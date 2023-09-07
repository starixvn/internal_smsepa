
CREATE PROCEDURE [dbo].[SettingEmployeeFlow_Get] @FlowTemplateId INT
AS
/*
Created: 20150722@Tim.Trinh
Task: Get employee flow setting.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,dept.[Name] AS Department
		,sec.[Name] AS Section
		,gr.[Name] AS Team
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Section AS sec WITH (NOLOCK) ON gr.DepartmentId = sec.SectionId
		AND sec.IsActive = 1
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON sec.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
	WHERE emp.[Enabled] = 1
		AND ISNULL(emp.IsDeleted, 0) = 0
		AND EXISTS (
			SELECT 1
			FROM dbo.SettingEmployeeFlow AS efs WITH (NOLOCK)
				INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON efs.FlowTemplateId = ft.FlowTemplateId
			WHERE ft.FlowTemplateId = @FlowTemplateId
				AND emp.EmployeeId = efs.EmployeeId
			)

	SELECT ef.EmployeeId
		,ef.ProcessId
		,ef.EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM dbo.SettingEmployeeFlow AS ef WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON ef.EvaluatorId = emp.EmployeeId
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON ef.FlowTemplateId = ft.FlowTemplateId
		AND ft.FlowTemplateId = @FlowTemplateId
	WHERE ef.ProcessId <> 2 -- self
	ORDER BY ef.EmployeeId ASC
		,ef.ProcessId ASC
		,ef.[Priority] ASC
END
