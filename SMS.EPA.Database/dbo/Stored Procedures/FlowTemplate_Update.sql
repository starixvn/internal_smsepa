/****** Object:  Stored Procedure dbo.FlowTemplate_Update    Script Date: 5/16/2016 2:20:09 PM ******/
/****** Object:  Stored Procedure dbo.FlowTemplate_Update    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.FlowTemplate_Update    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.FlowTemplate_Update    Script Date: 4/6/2016 11:27:46 AM ******/

CREATE PROCEDURE [dbo].[FlowTemplate_Update] @FlowTemplateId INT
	,@Name NVARCHAR(200)
	,@Description NVARCHAR(500) = NULL
	,@IsActive BIT
	,@TargetTypeId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150805@Tim.Trinh
Task: Update Flow Template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.FlowTemplate WITH (NOLOCK)
			WHERE [Name] = @Name
				AND [FlowTemplateId] != @FlowTemplateId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Flow Template ''' + @Name + ''' has already existed.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.FlowTemplate WITH (NOLOCK)
			WHERE TargetTypeId = @TargetTypeId
				AND [FlowTemplateId] != @FlowTemplateId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'This Target Type has already existed setting.'

		RETURN
	END

	DECLARE @Data NVARCHAR(MAX)
	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog] @ObjectId = @FlowTemplateId, @ObjectType = 6

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE dbo.FlowTemplate WITH (ROWLOCK, UPDLOCK)
		SET [Name] = @Name
			,[TargetTypeId] = @TargetTypeId
			,[Description] = @Description
			,[IsActive] = @IsActive
			,[ModifiedBy] = @ModifiedBy
			,[ModifiedDate] = GETDATE()
		WHERE [FlowTemplateId] = @FlowTemplateId

		COMMIT TRANSACTION;

		EXEC @Data = [dbo].[fncGetDataForWriteLog] @ObjectId = @FlowTemplateId, @ObjectType = 6
		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy, @SpName = 'FlowTemplate_Update', @ObjectId = @FlowTemplateId, @ObjectTypeId = 6, @Data = @Data, @OldData = @OldData, @ObjectIds = NULL

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Flow Template unsuccessfully.'
	END CATCH
END