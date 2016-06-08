/****** Object:  Stored Procedure dbo.FlowTemplate_UpdateStatus    Script Date: 5/16/2016 2:20:09 PM ******/
/****** Object:  Stored Procedure dbo.FlowTemplate_UpdateStatus    Script Date: 5/16/2016 2:15:32 PM ******/
/****** Object:  Stored Procedure dbo.FlowTemplate_UpdateStatus    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.FlowTemplate_UpdateStatus    Script Date: 4/6/2016 11:27:46 AM ******/

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
	SET NOCOUNT ON;
	
	DECLARE @Data NVARCHAR(MAX)
	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_UpdateStatus] @ObjectId = @FlowTemplateId, @ObjectType = 6

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE dbo.FlowTemplate WITH (ROWLOCK, UPDLOCK)
		SET IsActive = @IsActive
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		WHERE FlowTemplateId = @FlowTemplateId

		COMMIT TRANSACTION;

		EXEC @Data = [dbo].[fncGetDataForWriteLog_UpdateStatus] @ObjectId = @FlowTemplateId, @ObjectType = 6
		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy, @SpName = 'FlowTemplate_UpdateStatus', @ObjectId = @FlowTemplateId, @ObjectTypeId = 6, @Data = @Data, @OldData = @OldData, @ObjectIds = NULL

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Status Flow Tempalte unsuccessfully.'
	END CATCH
END