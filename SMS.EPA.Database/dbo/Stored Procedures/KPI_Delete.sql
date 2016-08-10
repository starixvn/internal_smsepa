CREATE PROCEDURE [dbo].[KPI_Delete] @KPIId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150709@Tim.Trinh
Task: Delete KPI.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.KPI WITH (NOLOCK)
			WHERE KPIId = @KPIId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The KPI does not exists.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.TargetTemplateKPI WITH (NOLOCK)
			WHERE KPIId = @KPIId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete KPI unsuccessfully. It has been already existed in target template.'

		RETURN
	END

	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog] @ObjectId = @KPIId
		,@ObjectType = 1

	BEGIN TRY
		BEGIN TRANSACTION;

		DECLARE @LogNo VARCHAR(20)

		SET @LogNo = CONVERT(VARCHAR(8), GETDATE(), 112) + REPLACE(CONVERT(VARCHAR(12), GETDATE(), 114), ':', '')

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
		SELECT @KPIId
			,@LogNo
			,[Name]
			,[Description]
			,[HasComment]
			,[IsRequiredComment]
			,[IsRequiredEvaluate]
			,[IsActive]
			,@ModifiedBy
			,GETDATE()
		FROM dbo.KPI WITH (NOLOCK)
		WHERE KPIId = @KPIId

		DELETE
		FROM dbo.KPI
		WHERE KPIId = @KPIId

		DELETE
		FROM dbo.KPIDetail
		WHERE KPIId = @KPIId

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'KPI_Delete'
			,@ObjectId = @KPIId
			,@ObjectTypeId = 1
			,@Data = NULL
			,@OldData = @OldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete KPI unsuccessfully.'
	END CATCH
END