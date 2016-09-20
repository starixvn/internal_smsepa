
CREATE PROCEDURE [dbo].[FlowTemplate_UpdateStatus] @FlowTemplateId INT
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150807@Tim.Trinh
Task: Update Status Flow Template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = dbo.fncGetDataForWriteLog_UpdateStatus @ObjectId = @FlowTemplateId
		,@ObjectType = 6

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE dbo.FlowTemplate WITH (ROWLOCK, UPDLOCK)
		SET IsActive = @IsActive
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		WHERE FlowTemplateId = @FlowTemplateId

		EXEC @Data = dbo.fncGetDataForWriteLog_UpdateStatus @ObjectId = @FlowTemplateId
			,@ObjectType = 6

		EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
			,@SpName = 'FlowTemplate_UpdateStatus'
			,@ObjectId = @FlowTemplateId
			,@ObjectTypeId = 6
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully.'
	END CATCH
END