/****** Object:  Stored Procedure dbo.TargetTemplate_Delete    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_Delete    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_Delete    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_Delete    Script Date: 4/6/2016 11:27:49 AM ******/

CREATE PROCEDURE [dbo].[TargetTemplate_Delete] 
	@TargetTemplateId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140404@Quinn.Vo
Task: Delete Target template.
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
				FROM dbo.TargetTemplate WITH (NOLOCK)
				WHERE TargetTemplateId = @TargetTemplateId
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The Target Template does not exists.'

		RETURN
	END

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.[TargetTemplateDepartment] WITH (NOLOCK)
				WHERE [TargetTemplateId] = @TargetTemplateId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.[TargetTemplateEmployee] WITH (NOLOCK)
				WHERE [TargetTemplateId] = @TargetTemplateId
				)
	)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete Target Template unsuccessfully. It has already in target employee setting.'

		RETURN
	END

	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog] @ObjectId = @TargetTemplateId, @ObjectType = 3

	DELETE
	FROM dbo.TargetTemplateKPIGroup
	WHERE TargetTemplateId = @TargetTemplateId

	DELETE
	FROM dbo.TargetTemplate
	WHERE TargetTemplateId = @TargetTemplateId

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete Target Template failed.'
	END
	ELSE
	BEGIN
		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy, @SpName = 'TargetTemplate_Delete', @ObjectId = @TargetTemplateId, @ObjectTypeId = 3, @Data = NULL, @OldData = @OldData, @ObjectIds = NULL

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END