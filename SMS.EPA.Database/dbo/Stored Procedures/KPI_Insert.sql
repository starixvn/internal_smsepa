CREATE PROCEDURE [dbo].[KPI_Insert] @KPIId INT OUTPUT
	,@Name NVARCHAR(500)
	,@Description NVARCHAR(MAX) = NULL
	,@KPIGroupIds VARCHAR(500) = ''
	,@HasComment BIT
	,@IsRequiredComment BIT
	,@IsRequiredEvaluate BIT
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150708@Tim.Trinh
Task: Insert KPI.
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
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'KPI ''' + @Name + ''' has already existed.'

		RETURN
	END

	IF OBJECT_ID('tempdb..#KPIGroup') IS NOT NULL
	BEGIN
		DROP TABLE #KPIGroup
	END

	CREATE TABLE #KPIGroup (KPIGroupId INT)

	IF ISNULL(@KPIGroupIds, '') <> ''
	BEGIN
		INSERT INTO #KPIGroup (KPIGroupId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@KPIGroupIds, ',')
	END

	BEGIN TRY
		BEGIN TRANSACTION;

		DECLARE @LogNo VARCHAR(20)
			,@CreatedDate DATETIME

		SET @LogNo = CONVERT(VARCHAR(8), GETDATE(), 112) + REPLACE(CONVERT(VARCHAR(12), GETDATE(), 114), ':', '')
		SET @CreatedDate = GETDATE()

		INSERT INTO dbo.KPI (
			[Name]
			,[LogNo]
			,[Description]
			,[HasComment]
			,[IsRequiredComment]
			,[IsRequiredEvaluate]
			,[IsActive]
			,[CreatedBy]
			,[CreatedDate]
			)
		VALUES (
			@Name
			,@LogNo
			,@Description
			,@HasComment
			,@IsRequiredComment
			,@IsRequiredEvaluate
			,1
			,@CreatedBy
			,@CreatedDate
			)

		SELECT @KPIId = SCOPE_IDENTITY();

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
			,1
			,@CreatedBy
			,@CreatedDate
			)

		DECLARE @Data NVARCHAR(MAX)

		EXEC @Data = [dbo].[fncGetDataForWriteLog] @ObjectId = @KPIId
			,@ObjectType = 1

		EXEC [dbo].[LogDetail_Insert] @ActorId = @CreatedBy
			,@SpName = 'KPI_Insert'
			,@ObjectId = @KPIId
			,@ObjectTypeId = 1
			,@Data = @Data
			,@OldData = NULL
			,@ObjectIds = NULL

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert KPI unsuccessfully.'
	END CATCH

	DROP TABLE #KPIGroup
END