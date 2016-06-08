/****** Object:  Stored Procedure dbo.EpaEmployee_ChangePassword    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_ChangePassword    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_ChangePassword    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_ChangePassword    Script Date: 4/6/2016 11:27:44 AM ******/

CREATE PROCEDURE [dbo].[EpaEmployee_ChangePassword] @EmployeeId INT
	,@NewPassword NVARCHAR(100)
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Change password of employee.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @OldData NVARCHAR(MAX)
	DECLARE @Data NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_Employee_ChangePassWord] @EmployeeId = @EmployeeId

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE [dbo].[Employee] WITH (ROWLOCK, UPDLOCK)
		SET [Password] = @NewPassword
			,[ModifiedBy] = @ModifiedBy
		WHERE [EmployeeId] = @EmployeeId

		COMMIT TRANSACTION

		EXEC @Data = [dbo].[fncGetDataForWriteLog_Employee_ChangePassWord] @EmployeeId = @EmployeeId

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'EpaEmployee_ChangePassword'
			,@ObjectId = @EmployeeId
			,@ObjectTypeId = 4
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH
END