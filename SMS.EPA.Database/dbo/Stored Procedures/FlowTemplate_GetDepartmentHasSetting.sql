/****** Object:  Stored Procedure dbo.FlowTemplate_GetDepartmentHasSetting    Script Date: 5/16/2016 2:20:07 PM ******/
/****** Object:  Stored Procedure dbo.FlowTemplate_GetDepartmentHasSetting    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  Stored Procedure dbo.FlowTemplate_GetDepartmentHasSetting    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  Stored Procedure dbo.FlowTemplate_GetDepartmentHasSetting    Script Date: 4/6/2016 11:27:46 AM ******/

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
	SET NOCOUNT ON;

	SELECT deptF.[DepartmentId]
		,gr.[Name]
		,deptF.[IsGroup]
	FROM [dbo].[SettingDepartmentFlow] AS deptF WITH (NOLOCK)
		INNER JOIN [dbo].[Group] AS gr WITH (NOLOCK) ON deptF.[DepartmentId] = gr.[GroupId]
		AND deptF.[IsGroup] = 1
		INNER JOIN [dbo].[FlowTemplate] AS ft WITH (NOLOCK) ON deptF.[FlowTemplateId] = ft.[FlowTemplateId]
	WHERE ft.[FlowTemplateId] = @FlowTemplateId
	
	UNION
	SELECT deptF.[DepartmentId]
		,dept.[Name]
		,deptF.[IsGroup]
	FROM [dbo].[SettingDepartmentFlow] AS deptF WITH (NOLOCK)
		INNER JOIN [dbo].[Department] AS dept WITH (NOLOCK) ON deptF.[DepartmentId] = dept.[DepartmentId]
		AND deptF.[IsGroup] = 0
		INNER JOIN [dbo].[FlowTemplate] AS ft WITH (NOLOCK) ON deptF.[FlowTemplateId] = ft.[FlowTemplateId]
	WHERE ft.[FlowTemplateId] = @FlowTemplateId
END