
CREATE PROCEDURE [dbo].[SettingEmployeeFlow_Delete] @EmployeeId INT
	,@FlowTemplateId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150722@Tim.Trinh
Task: Delete employee flow setting.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_Flow_EmployeeSetting] @FlowTemplateId = @FlowTemplateId
		,@EmployeeIds = @EmployeeId

	DELETE
	FROM dbo.SettingEmployeeFlow
	WHERE EmployeeId = @EmployeeId
		AND FlowTemplateId = @FlowTemplateId

	EXEC @Data = [dbo].[fncGetDataForWriteLog_Flow_EmployeeSetting] @FlowTemplateId = @FlowTemplateId
		,@EmployeeIds = @EmployeeId

	EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
		,@SpName = 'SettingEmployeeFlow_Delete'
		,@ObjectId = @FlowTemplateId
		,@ObjectTypeId = 6
		,@Data = @Data
		,@OldData = @OldData
		,@ObjectIds = NULL
END