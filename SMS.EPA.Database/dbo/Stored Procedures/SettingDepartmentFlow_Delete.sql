
CREATE PROCEDURE [dbo].[SettingDepartmentFlow_Delete] @DepartmentId INT
	,@FlowTemplateId INT
	,@IsGroup BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150721@Tim.Trinh
Task: Delete department flow setting.
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
	SET NOCOUNT ON

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_Flow_DepartmentSetting] @DepartmentId = @DepartmentId
		,@FlowTemplateId = @FlowTemplateId
		,@IsGroup = @IsGroup

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE
		FROM dbo.SettingDepartmentFlow
		WHERE DepartmentId = @DepartmentId
			AND FlowTemplateId = @FlowTemplateId
			AND IsGroup = @IsGroup

		EXEC @Data = [dbo].[fncGetDataForWriteLog_Flow_DepartmentSetting] @DepartmentId = @DepartmentId
			,@FlowTemplateId = @FlowTemplateId
			,@IsGroup = @IsGroup

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'SettingDepartmentFlow_Delete'
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
			,@ErrMsg = N'Delete department flow setting unsuccessfully.'
	END CATCH
END