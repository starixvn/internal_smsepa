
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
		,dept.[Name]
		,gr.[GroupId]
		,gr.[Name] AS GroupName
		,gr.[Code] AS GroupCode
	FROM [dbo].[Employee] AS emp WITH (NOLOCK)
		LEFT JOIN [dbo].[Group] AS gr WITH (NOLOCK) ON emp.[GroupId] = gr.[GroupId]
		LEFT JOIN [dbo].[Department] AS dept WITH (NOLOCK) ON gr.[DepartmentId] = dept.[DepartmentId]
		LEFT JOIN [dbo].[HRMJobTitle] AS job WITH (NOLOCK) ON emp.[JobTitle] = job.[Id]
	WHERE emp.[EmployeeId] = @EmployeeId

	SELECT ro.RoleId
		,ref.Value AS RoleCode
	FROM dbo.EmployeeRole AS ro WITH (NOLOCK)
		INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON ro.RoleId = ref.ResourceId
		AND ref.RefName = 'SystemRole'
	WHERE ro.EmployeeId = @EmployeeId
END