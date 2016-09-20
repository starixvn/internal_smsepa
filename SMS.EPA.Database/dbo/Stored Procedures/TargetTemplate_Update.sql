
CREATE PROCEDURE [dbo].[TargetTemplate_Update] @TargetTemplateId INT
	,@KPIs VARCHAR(MAX) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2010404@Quinn.Vo
Task: Update Target Template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
• @KPIs=N'<ROOT>
		 <KPI
			KPIId="1"
			KPIGroupId="1"
			IsSpreadKPI="0"
			Weight="3"
			DisplayOrder="1"
			IsRequiredEvaluate="true"
			HasComment="true"
			IsRequiredComment="true">
		  </KPI>
		  </ROOT>'
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.TargetTemplate WITH (NOLOCK)
			WHERE TargetTemplateId = @TargetTemplateId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Target template does not exists.'

		RETURN
	END

	CREATE TABLE #KPIs (
		KPIId INT
		,KPIGroupId INT
		,IsSpreadKPI BIT
		,[Weight] FLOAT
		,DisplayOrder INT
		,IsRequiredEvaluate BIT
		,HasComment BIT
		,IsRequiredComment BIT
		)

	-- Read data from KPIGroups
	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@KPIs

		INSERT INTO #KPIs (
			KPIId
			,KPIGroupId
			,IsSpreadKPI
			,[Weight]
			,DisplayOrder
			,IsRequiredEvaluate
			,HasComment
			,IsRequiredComment
			)
		SELECT KPIId
			,KPIGroupId
			,IsSpreadKPI
			,[Weight]
			,DisplayOrder
			,IsRequiredEvaluate
			,HasComment
			,IsRequiredComment
		FROM OPENXML(@DocHandler, '/ROOT/KPI', 1) WITH (KPIId INT, KPIGroupId INT, IsSpreadKPI BIT, [Weight] FLOAT, DisplayOrder INT, IsRequiredEvaluate BIT, HasComment BIT, IsRequiredComment BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Target template unsuccessfully. Paramter invalid.'

		RETURN
	END CATCH

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_TargetTemplate_UpdateKPI] @ObjectId = @TargetTemplateId

	-- End read xml
	BEGIN TRY
		BEGIN TRANSACTION;

		DELETE tp
		FROM dbo.TargetTemplateKPI AS tp
		WHERE tp.TargetTemplateId = @TargetTemplateId
			AND NOT EXISTS (
				SELECT 1
				FROM #KPIs AS k
				WHERE k.KPIGroupId = tp.KPIGroupId
					AND k.KPIId = tp.KPIId
				)

		UPDATE tp
		SET tp.[Weight] = k.[Weight]
			,tp.DisplayOrder = k.DisplayOrder
			,tp.IsRequiredEvaluate = k.IsRequiredEvaluate
			,tp.HasComment = k.HasComment
			,tp.IsRequiredComment = k.IsRequiredComment
			,tp.ModifiedBy = @ModifiedBy
			,tp.ModifiedDate = GETDATE()
		FROM dbo.TargetTemplateKPI AS tp WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #KPIs AS k ON tp.TargetTemplateId = @TargetTemplateId
			AND tp.KPIGroupId = k.KPIGroupId
			AND tp.KPIId = k.KPIId

		UPDATE ttkg
		SET ttkg.IsSpreadKPI = k.IsSpreadKPI
			,ttkg.ModifiedBy = @ModifiedBy
			,ttkg.ModifiedDate = GETDATE()
		FROM dbo.TargetTemplateKPIGroup AS ttkg WITH (ROWLOCK, UPDLOCK)
			INNER JOIN (
			SELECT DISTINCT KPIGroupId
				,IsSpreadKPI
			FROM #KPIs
			) AS k ON ttkg.TargetTemplateId = @TargetTemplateId
			AND ttkg.KPIGroupId = k.KPIGroupId

		DELETE k
		FROM #KPIs AS k
		WHERE EXISTS (
				SELECT 1
				FROM dbo.TargetTemplateKPI AS tp
				WHERE tp.TargetTemplateId = @TargetTemplateId
					AND k.KPIGroupId = tp.KPIGroupId
					AND k.KPIId = tp.KPIId
				)

		INSERT dbo.TargetTemplateKPI (
			TargetTemplateId
			,KPIGroupId
			,KPIId
			,[Weight]
			,DisplayOrder
			,IsRequiredEvaluate
			,HasComment
			,IsRequiredComment
			,AddedBy
			,AddedDate
			)
		SELECT @TargetTemplateId
			,KPIGroupId
			,KPIId
			,[Weight]
			,DisplayOrder
			,IsRequiredEvaluate
			,HasComment
			,IsRequiredComment
			,@ModifiedBy
			,GETDATE()
		FROM #KPIs

		EXEC @Data = [dbo].[fncGetDataForWriteLog_TargetTemplate_UpdateKPI] @ObjectId = @TargetTemplateId

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'TargetTemplate_Update'
			,@ObjectId = @TargetTemplateId
			,@ObjectTypeId = 3
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Target template unsuccessfully.'
	END CATCH
END