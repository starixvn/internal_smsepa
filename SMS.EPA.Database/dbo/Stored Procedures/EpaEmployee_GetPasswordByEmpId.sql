
CREATE PROCEDURE [dbo].[EpaEmployee_GetPasswordByEmpId] @EmployeeId INT
AS
/*
Created: 2016-08-22@Tim.Trinh
Task: Get password of employee.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT emp.[Password]
	FROM [dbo].[Employee] AS emp WITH (NOLOCK)
	WHERE emp.EmployeeId = @EmployeeId
END