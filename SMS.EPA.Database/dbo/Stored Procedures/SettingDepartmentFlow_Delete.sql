/****** Object:  Stored Procedure dbo.SettingDepartmentFlow_Delete    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.SettingDepartmentFlow_Delete    Script Date: 5/16/2016 2:15:32 PM ******/
/****** Object:  Stored Procedure dbo.SettingDepartmentFlow_Delete    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.SettingDepartmentFlow_Delete    Script Date: 4/6/2016 11:27:47 AM ******/
CREATE PROCEDURE [dbo].[SettingDepartmentFlow_Delete] 
     @DepartmentId INT
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
	SET NOCOUNT ON;
     
	DECLARE @Data NVARCHAR(MAX)
	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_Flow_DepartmentSetting] @DepartmentId = @DepartmentId, @FlowTemplateId = @FlowTemplateId, @IsGroup = @IsGroup

     BEGIN TRY
		BEGIN TRANSACTION;
		
	     DELETE
	     FROM dbo.SettingDepartmentFlow
	     WHERE DepartmentId = @DepartmentId
	          AND FlowTemplateId = @FlowTemplateId
	          AND IsGroup = @IsGroup
	     
	     COMMIT TRANSACTION;

		 EXEC @Data = [dbo].[fncGetDataForWriteLog_Flow_DepartmentSetting] @DepartmentId = @DepartmentId, @FlowTemplateId = @FlowTemplateId, @IsGroup = @IsGroup
		 EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy, @SpName = 'SettingDepartmentFlow_Delete', @ObjectId = @FlowTemplateId, @ObjectTypeId = 6, @Data = @Data, @OldData = @OldData, @ObjectIds = NULL


		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete department flow setting unsuccessfully.'
	END CATCH
END