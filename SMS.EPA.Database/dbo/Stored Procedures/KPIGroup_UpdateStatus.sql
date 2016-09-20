
CREATE PROCEDURE [dbo].[KPIGroup_UpdateStatus] @KPIGroupId INT
	,@IsActive BIT = 0
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150710@Tim.Trinh
Task: Update status of KPI Group.
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
			FROM dbo.KPIGroup WITH (NOLOCK)
			WHERE KPIGroupId = @KPIGroupId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'KPI Group does not exists.'

		RETURN
	END

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_UpdateStatus] @ObjectId = @KPIGroupId
		,@ObjectType = 2

	BEGIN TRY
		BEGIN TRANSACTION;

		DECLARE @LogNo VARCHAR(20)
			,@ModifiedDate DATETIME

		SET @LogNo = CONVERT(VARCHAR(8), GETDATE(), 112) + REPLACE(CONVERT(VARCHAR(12), GETDATE(), 114), ':', '')
		SET @ModifiedDate = GETDATE()

		UPDATE dbo.KPIGroup WITH (ROWLOCK, UPDLOCK)
		SET IsActive = @IsActive
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = @ModifiedDate
		WHERE KPIGroupId = @KPIGroupId

		INSERT INTO dbo.KPIGroupHistory (
			[KPIGroupId]
			,[LogNo]
			,[Name]
			,[Description]
			,[IsActive]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT @KPIGroupId
			,@LogNo
			,[Name]
			,[Description]
			,[IsActive]
			,@ModifiedBy
			,@ModifiedDate
		FROM dbo.KPIGroup WITH (NOLOCK)
		WHERE KPIGroupId = @KPIGroupId

		EXEC @Data = [dbo].[fncGetDataForWriteLog_UpdateStatus] @ObjectId = @KPIGroupId
			,@ObjectType = 2

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'KPIGroup_UpdateStatus'
			,@ObjectId = @KPIGroupId
			,@ObjectTypeId = 2
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update KPI Group unsuccessfully.'
	END CATCH
END