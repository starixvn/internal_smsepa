/****** Object:  Stored Procedure dbo.EpaEmployee_AssignRoleToEmployee    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_AssignRoleToEmployee    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_AssignRoleToEmployee    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_AssignRoleToEmployee    Script Date: 4/6/2016 11:27:44 AM ******/

CREATE PROCEDURE [dbo].[EpaEmployee_AssignRoleToEmployee]
	@EmployeeId INT
	,@RoleIds NVARCHAR(500) = ''
	,@ModifiedBy INT=NULL

	,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-11@Josh.Phan
Task: Assign role to user.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;	

	DELETE FROM [dbo].[EmployeeRole] WHERE [EmployeeId] = @EmployeeId

	IF ISNULL(@RoleIds,'')<>''
	BEGIN
		INSERT INTO [dbo].[EmployeeRole] (
			[EmployeeId]
			,[RoleId]
			,[ModifiedBy]
			,[ModifiedDate] )
		SELECT DISTINCT 
			@EmployeeId
			,val
			,@ModifiedBy
			,GETDATE()
		FROM fnc_strSplit (@RoleIds, ',')
	END
END