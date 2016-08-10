CREATE PROCEDURE [dbo].[KPI_Update] @KPIId INT
	,@Name NVARCHAR(500)
	,@Description NVARCHAR(MAX) = NULL
	,@KPIGroupIds VARCHAR(500) = ''
	,@HasComment BIT
	,@IsActive BIT
	,@IsRequiredComment BIT
	,@IsRequiredEvaluate BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150708@Tim.Trinh
Task: Update KPI.
DB: SMSEPA
Original:

Revisions:  
Param's Explanation:
• @KPIGroupIds: List KPI groups id.
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.KPI WITH (NOLOCK)
			WHERE [Name] = @Name
				AND KPIId <> @KPIId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'KPI ''' + @Name + ''' has already existed.'

		RETURN
	END

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	IF OBJECT_ID('tempdb..#KPIGroup') IS NOT NULL
	BEGIN
		DROP TABLE #KPIGroup
	END

	CREATE TABLE #KPIGroup (KPIGroupId INT)

	IF ISNULL(@KPIGroupIds, '') <> ''
	BEGIN
		INSERT INTO #KPIGroup (KPIGroupId)
		SELECT DISTINCT val
		FROM fnc_strSplit(@KPIGroupIds, ',')
	END

	EXEC @OldData = [dbo].[fncGetDataForWriteLog] @ObjectId = @KPIId
		,@ObjectType = 1

	BEGIN TRY
		BEGIN TRANSACTION;

		DECLARE @LogNo VARCHAR(20)
			,@ModifiedDate DATETIME

		SET @LogNo = CONVERT(VARCHAR(8), GETDATE(), 112) + REPLACE(CONVERT(VARCHAR(12), GETDATE(), 114), ':', '')
		SET @ModifiedDate = GETDATE()

		UPDATE dbo.KPI WITH (ROWLOCK, UPDLOCK)
		SET [Name] = @Name
			,[LogNo] = @LogNo
			,[Description] = @Description
			,[HasComment] = @HasComment
			,[IsActive] = @IsActive
			,[IsRequiredComment] = @IsRequiredComment
			,[IsRequiredEvaluate] = @IsRequiredEvaluate
			,[ModifiedBy] = @ModifiedBy
			,[ModifiedDate] = @ModifiedDate
		WHERE KPIId = @KPIId

		DELETE
		FROM dbo.KPIDetail
		WHERE KPIId = @KPIId
			AND KPIGroupId NOT IN (
				SELECT KPIGroupId
				FROM #KPIGroup
				)

		DELETE
		FROM #KPIGroup
		WHERE KPIGroupId IN (
				SELECT KPIGroupId
				FROM dbo.KPIDetail WITH (NOLOCK)
				WHERE KPIId = @KPIId
				)

		INSERT INTO dbo.KPIDetail (
			KPIGroupId
			,KPIId
			)
		SELECT KPIGroupId
			,@KPIId
		FROM #KPIGroup

		INSERT INTO dbo.KPIHistory (
			[KPIId]
			,[LogNo]
			,[Name]
			,[Description]
			,[HasComment]
			,[IsRequiredComment]
			,[IsRequiredEvaluate]
			,[IsActive]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		VALUES (
			@KPIId
			,@LogNo
			,@Name
			,@Description
			,@HasComment
			,@IsRequiredComment
			,@IsRequiredEvaluate
			,@IsActive
			,@ModifiedBy
			,@ModifiedDate
			)

		EXEC @Data = [dbo].[fncGetDataForWriteLog] @ObjectId = @KPIId
			,@ObjectType = 1

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'KPI_Update'
			,@ObjectId = @KPIId
			,@ObjectTypeId = 1
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update KPI unsuccessfully.'
	END CATCH

	DROP TABLE #KPIGroup
END