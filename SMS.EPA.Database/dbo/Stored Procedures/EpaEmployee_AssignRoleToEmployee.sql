
CREATE PROCEDURE [dbo].[EpaEmployee_AssignRoleToEmployee] @EmployeeId INT
	,@RoleIds NVARCHAR(500) = ''
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-11@Josh.Phan
Task: Assign role to user.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DELETE
	FROM [dbo].[EmployeeRole]
	WHERE [EmployeeId] = @EmployeeId

	IF ISNULL(@RoleIds, '') <> ''
	BEGIN
		INSERT INTO [dbo].[EmployeeRole] (
			[EmployeeId]
			,[RoleId]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT DISTINCT @EmployeeId
			,val
			,@ModifiedBy
			,GETDATE()
		FROM dbo.fnc_strSplit(@RoleIds, ',')
	END
END