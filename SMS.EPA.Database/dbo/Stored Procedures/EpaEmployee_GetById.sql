
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

	SELECT emp.EmployeeId
		,emp.UserId
		,emp.Code
		,emp.DomainUsername
		,emp.NickName
		,emp.FullName
		,emp.[Password]
		,emp.IsSpecialUser
		,emp.[Enabled]
		,emp.JoinedDate
		,emp.HasMonthlyEPA
		,sec.[Name] AS Section
		,emp.[JobTitle] AS JobTitleId
		,job.[Name] AS JobTitleName
		,dept.DepartmentId
		,dept.Code
		,dept.[Name]
		,gr.GroupId
		,gr.[Name] AS GroupName
		,gr.[Code] AS GroupCode
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
		LEFT JOIN dbo.[Section] AS sec WITH (NOLOCK) ON gr.DepartmentId = sec.SectionId
		AND sec.IsActive = 1
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON sec.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.[JobTitle] = job.[Id]
	WHERE emp.EmployeeId = @EmployeeId

	SELECT ro.RoleId
		,ref.Value AS RoleCode
	FROM dbo.EmployeeRole AS ro WITH (NOLOCK)
		INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON ro.RoleId = ref.ResourceId
		AND ref.RefName = 'SystemRole'
	WHERE ro.EmployeeId = @EmployeeId
END