CREATE PROCEDURE [dbo].[KPI_UpdateStatus] @KPIId INT
	,@IsActive BIT = 0
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150708@Tim.Trinh
Task: Update status of KPI.
DB: SMSEPA
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
			,@ErrMsg = N'KPI does not exists.'

		RETURN
	END

	DECLARE @OldData NVARCHAR(MAX)
	DECLARE @Data NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_UpdateStatus] @ObjectId = @KPIId
		,@ObjectType = 1

	BEGIN TRY
		BEGIN TRANSACTION;

		DECLARE @LogNo VARCHAR(20)
			,@ModifiedDate DATETIME

		SET @LogNo = CONVERT(VARCHAR(8), GETDATE(), 112) + REPLACE(CONVERT(VARCHAR(12), GETDATE(), 114), ':', '')
		SET @ModifiedDate = GETDATE()

		UPDATE dbo.KPI WITH (ROWLOCK, UPDLOCK)
		SET LogNo = @LogNo
			,IsActive = @IsActive
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = @ModifiedDate
		WHERE KPIId = @KPIId

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
			,@IsActive
			,@ModifiedBy
			,@ModifiedDate
		FROM dbo.KPI WITH (NOLOCK)
		WHERE KPIId = @KPIId

		EXEC @Data = [dbo].[fncGetDataForWriteLog_UpdateStatus] @ObjectId = @KPIId
			,@ObjectType = 1

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'KPI_UpdateStatus'
			,@ObjectId = @KPIId
			,@ObjectTypeId = 1
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		SELECT @ErrCode = 0
			,@ErrMsg = ''

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update KPI unsuccessfully.'
	END CATCH
END