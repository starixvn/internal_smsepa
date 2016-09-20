
CREATE PROCEDURE [dbo].[TargetTemplate_SetKPIGroup] @TargetTemplateId INT
	,@KPIGroups VARCHAR(5000) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140426@Quinn.Vo
Task: Add KPI group into Target template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
• @KPIGroups=N'<ROOT>
		 <KPIGroup
			KPIGroupId="1" 
			Weight="3"
			DisplayOrder = "1"
			IsSpreadKPI='1'>
		  </KPIGroup>
		  </ROOT>'
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #KPIGroups (
		KPIGroupId INT
		,[Weight] FLOAT
		,IsSpreadKPI BIT
		,DisplayOrder INT
		)

	-- Read data from KPIGroups
	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@KPIGroups

		INSERT INTO #KPIGroups (
			KPIGroupId
			,[Weight]
			,IsSpreadKPI
			,DisplayOrder
			)
		SELECT KPIGroupId
			,[Weight]
			,IsSpreadKPI
			,DisplayOrder
		FROM OPENXML(@DocHandler, '/ROOT/KPIGroup', 1) WITH (KPIGroupId INT, [Weight] FLOAT, IsSpreadKPI BIT, DisplayOrder INT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Add KPI Group failed. Paramter invalid.'

		RETURN
	END CATCH

	-- End read xml
	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_TargetTemplate_SetKPIGroup] @ObjectId = @TargetTemplateId

	BEGIN TRY
		BEGIN TRANSACTION;

		DELETE
		FROM dbo.TargetTemplateKPIGroup
		WHERE TargetTemplateId = @TargetTemplateId
			AND KPIGroupId NOT IN (
				SELECT KPIGroupId
				FROM #KPIGroups
				)

		DELETE
		FROM dbo.TargetTemplateKPI
		WHERE TargetTemplateId = @TargetTemplateId
			AND KPIGroupId NOT IN (
				SELECT KPIGroupId
				FROM #KPIGroups
				)

		UPDATE t
		SET t.[Weight] = k.[Weight]
			,t.IsSpreadKPI = k.IsSpreadKPI
			,t.DisplayOrder = k.DisplayOrder
			,t.ModifiedBy = @ModifiedBy
			,t.ModifiedDate = GETDATE()
		FROM dbo.TargetTemplateKPIGroup AS t WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #KPIGroups AS k ON t.TargetTemplateId = @TargetTemplateId
			AND t.KPIGroupId = k.KPIGroupId

		DELETE
		FROM #KPIGroups
		WHERE KPIGroupId IN (
				SELECT KPIGroupId
				FROM dbo.TargetTemplateKPIGroup WITH (NOLOCK)
				WHERE TargetTemplateId = @TargetTemplateId
				)

		INSERT dbo.TargetTemplateKPIGroup (
			TargetTemplateId
			,KPIGroupId
			,[Weight]
			,IsSpreadKPI
			,DisplayOrder
			,AddedBy
			,AddedDate
			)
		SELECT @TargetTemplateId
			,KPIGroupId
			,[Weight]
			,IsSpreadKPI
			,DisplayOrder
			,@ModifiedBy
			,GETDATE()
		FROM #KPIGroups

		UPDATE dbo.TargetTemplate WITH (ROWLOCK, UPDLOCK)
		SET ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()

		EXEC @Data = [dbo].[fncGetDataForWriteLog_TargetTemplate_SetKPIGroup] @ObjectId = @TargetTemplateId

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'TargetTemplate_SetKPIGroup'
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
			,@ErrMsg = N'Add KPI Group failed';
	END CATCH
END