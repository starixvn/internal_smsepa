/****** Object:  Stored Procedure dbo.TargetTemplate_UpdateStatus    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_UpdateStatus    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_UpdateStatus    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_UpdateStatus    Script Date: 4/6/2016 11:27:49 AM ******/

CREATE PROCEDURE [dbo].[TargetTemplate_UpdateStatus] 
	@TargetTemplateId INT
	,@IsActive BIT = 0
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140317@Quinn.Vo
Task: Update status of Target template.
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

	IF (
			NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole WITH (NOLOCK)
				WHERE EmployeeId = @ModifiedBy
					AND RoleId = 1
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'You are not authorized to update status target template'

		RETURN
	END

	IF (
			NOT EXISTS (
				SELECT 1
				FROM dbo.TargetTemplate WITH (NOLOCK)
				WHERE TargetTemplateId = @TargetTemplateId
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Target template does not exists '

		RETURN
	END

	DECLARE @Data NVARCHAR(MAX)
	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_UpdateStatus] @ObjectId = @TargetTemplateId, @ObjectType = 3

	UPDATE dbo.TargetTemplate WITH (ROWLOCK, UPDLOCK)
	SET IsActive = @IsActive
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE TargetTemplateId = @TargetTemplateId

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Target template failed.'
	END
	ELSE
	BEGIN
		EXEC @Data = [dbo].[fncGetDataForWriteLog_UpdateStatus] @ObjectId = @TargetTemplateId, @ObjectType = 3
		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy, @SpName = 'TargetTemplate_UpdateStatus', @ObjectId = @TargetTemplateId, @ObjectTypeId = 3, @Data = @Data, @OldData = @OldData, @ObjectIds = NULL

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END