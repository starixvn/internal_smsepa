
CREATE PROCEDURE [dbo].[SettingDepartmentFlow_Get] @FlowTemplateId INT
AS
/*
Created: 20151026@Tim.Trinh
Task: Get department flow setting.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	--SELECT dept.DepartmentId
	--	,dept.[Name] AS Department
	--	,NULL AS Section
	--	,NULL AS Team
	--FROM dbo.Department_New AS dept WITH (NOLOCK)
	--WHERE dept.IsActive = 1

	--UNION ALL
	--SELECT sec.SectionId AS DepartmentId
	--	,dept.[Name] AS Department
	--	,sec.[Name] AS Section
	--	,NULL AS Team
	--FROM dbo.Section AS sec WITH (NOLOCK)
	--	LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON sec.DepartmentId = dept.DepartmentId
	--	AND dept.IsActive = 1
	--WHERE sec.IsActive = 1

	--UNION ALL
	SELECT (- g.GroupId) AS DepartmentId
		,dept.[Name] AS Department
		,sec.[Name] AS Section
		,g.[Name] AS Team
	FROM dbo.[Group] AS g WITH (NOLOCK)
		LEFT JOIN dbo.Section AS sec WITH (NOLOCK) ON g.DepartmentId = sec.SectionId
		AND sec.IsActive = 1
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON sec.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
	WHERE g.IsDeleted = 0

	SELECT (CASE 
			WHEN df.IsGroup = 1 THEN - df.DepartmentId
			ELSE df.DepartmentId END
			) AS DepartmentId
		,df.ProcessId
		,df.EvaluatorId
		,e.Code
		,e.NickName
		,e.FullName
	FROM dbo.SettingDepartmentFlow AS df WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON df.EvaluatorId = e.EmployeeId
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON df.FlowTemplateId = ft.FlowTemplateId
	WHERE df.FlowTemplateId = @FlowTemplateId
		AND df.ProcessId != 2 -- self
		AND df.IsGroup = 1
	ORDER BY DepartmentId DESC
		,df.ProcessId ASC
		,df.[Priority] ASC
END
