/****** Object:  Stored Procedure dbo.EpaTargetTemplateEmployee_Delete    Script Date: 5/16/2016 2:20:09 PM ******/
/****** Object:  Stored Procedure dbo.EpaTargetTemplateEmployee_Delete    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.EpaTargetTemplateEmployee_Delete    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaTargetTemplateEmployee_Delete    Script Date: 4/6/2016 11:27:46 AM ******/

CREATE PROCEDURE [dbo].[EpaTargetTemplateEmployee_Delete] @TargetTemplateId INT
	,@EmployeeId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Delete target template setting of employee.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Data NVARCHAR(MAX)
	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_TargetTemplateSetting_Employee] @EmployeeId = @EmployeeId

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE
		FROM [dbo].[TargetTemplateEmployee]
		WHERE [TargetTemplateId] = @TargetTemplateId
			AND [EmployeeId] = @EmployeeId

		COMMIT TRANSACTION

		EXEC @Data = [dbo].[fncGetDataForWriteLog_TargetTemplateSetting_Employee] @EmployeeId = @EmployeeId

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'EpaTargetTemplateEmployee_Delete'
			,@ObjectId = NULL
			,@ObjectTypeId = 7
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete target template of employee unsuccessfully.'
	END CATCH
END