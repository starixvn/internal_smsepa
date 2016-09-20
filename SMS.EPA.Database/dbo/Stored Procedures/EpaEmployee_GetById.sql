
CREATE PROCEDURE [dbo].[EpaEmployee_GetById] @EmployeeId INT
AS
/*
Created: 20150720@Tim.Trinh
Task: Get employee by id
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT emp.[EmployeeId]
		,emp.[Code]
		,emp.[DomainUsername]
		,emp.[NickName]
		,emp.[FullName]
		,emp.[Password]
		,emp.[IsSpecialUser]
		,emp.[Enabled]
		,emp.JoinedDate
		,emp.HasMonthlyEPA
		,emp.[JobTitle] AS JobTitleId
		,job.[Name] AS JobTitleName
		,dept.DepartmentId
		,dept.Code
		,dept.NAME
		,gr.[GroupId]
		,gr.[Name] AS GroupName
		,gr.[Code] AS GroupCode
	FROM [dbo].[Employee] AS emp WITH (NOLOCK)
		LEFT JOIN [dbo].[Group] AS gr WITH (NOLOCK) ON emp.[GroupId] = gr.[GroupId]
		LEFT JOIN [dbo].[Department] AS dept WITH (NOLOCK) ON gr.[DepartmentId] = dept.[DepartmentId]
		LEFT JOIN [dbo].[HRMJobTitle] AS job WITH (NOLOCK) ON emp.[JobTitle] = job.[Id]
	WHERE emp.[EmployeeId] = @EmployeeId

	SELECT emRo.[RoleId]
		,loDa.[Code] AS RoleCode
	FROM [dbo].[EmployeeRole] AS emRo WITH (NOLOCK)
		INNER JOIN [dbo].[LookupData] loDa WITH (NOLOCK) ON emRo.[RoleId] = loDa.[Value]
	WHERE emRO.[EmployeeId] = @EmployeeId
		AND loDa.[LookupId] = 2 -- System Role
END