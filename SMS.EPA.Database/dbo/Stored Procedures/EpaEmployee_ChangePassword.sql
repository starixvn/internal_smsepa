CREATE PROCEDURE [dbo].[EpaEmployee_ChangePassword] @EmployeeId INT
	,@NewPassword NVARCHAR(100)
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Change password of employee.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @OldData NVARCHAR(MAX)
		,@Data NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_Employee_ChangePassWord] @EmployeeId = @EmployeeId

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE [dbo].[Employee] WITH (ROWLOCK, UPDLOCK)
		SET [Password] = @NewPassword
			,[ModifiedBy] = @ModifiedBy
		WHERE [EmployeeId] = @EmployeeId

		EXEC @Data = [dbo].[fncGetDataForWriteLog_Employee_ChangePassWord] @EmployeeId = @EmployeeId

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'EpaEmployee_ChangePassword'
			,@ObjectId = @EmployeeId
			,@ObjectTypeId = 4
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH
END