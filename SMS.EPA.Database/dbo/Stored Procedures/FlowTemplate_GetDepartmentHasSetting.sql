
CREATE PROCEDURE [dbo].[FlowTemplate_GetDepartmentHasSetting] @FlowTemplateId INT
AS
/*
Created: 20151022@Josh.Phan
Task: Get deparment, Section has flow setting.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	SELECT deptF.DepartmentId
		,gr.[Name]
		,deptF.IsGroup
	FROM dbo.SettingDepartmentFlow AS deptF WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON deptF.DepartmentId = gr.GroupId
		AND deptF.IsGroup = 1
		AND gr.IsDeleted = 0
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON deptF.FlowTemplateId = ft.FlowTemplateId
	WHERE ft.FlowTemplateId = @FlowTemplateId
	
	UNION
	SELECT deptF.DepartmentId
		,dept.[Name]
		,deptF.IsGroup
	FROM dbo.SettingDepartmentFlow AS deptF WITH (NOLOCK)
		INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON deptF.DepartmentId = dept.DepartmentId
		AND deptF.IsGroup = 0
		AND dept.IsActive = 1
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON deptF.[FlowTemplateId] = ft.FlowTemplateId
	WHERE ft.FlowTemplateId = @FlowTemplateId
END