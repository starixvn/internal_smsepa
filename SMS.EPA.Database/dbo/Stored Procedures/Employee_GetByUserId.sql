

CREATE PROCEDURE [dbo].[Employee_GetByUserId] @UserId INT
AS
/*
Created: 20220111@Tim.Trinh
Task: get employee by userid
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT emp.EmployeeId
		,emp.DomainUsername
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.[Password]
		,emp.IsSpecialUser
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE emp.UserId = @UserId
END