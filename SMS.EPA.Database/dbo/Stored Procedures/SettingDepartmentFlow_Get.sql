/****** Object:  Stored Procedure dbo.SettingDepartmentFlow_Get    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.SettingDepartmentFlow_Get    Script Date: 5/16/2016 2:15:32 PM ******/
/****** Object:  Stored Procedure dbo.SettingDepartmentFlow_Get    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.SettingDepartmentFlow_Get    Script Date: 4/6/2016 11:27:47 AM ******/

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

	SELECT dept.DepartmentId
		,dept.Code
		,dept.[Name]
	FROM dbo.Department AS dept WITH (NOLOCK)
	WHERE dept.IsActive = 1
	
	UNION ALL
	SELECT (- g.GroupId) AS DepartmentId
		,g.Code
		,g.[Name]
	FROM dbo.[Group] AS g WITH (NOLOCK)
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
		AND df.ProcessId <> 2 -- self
	ORDER BY DepartmentId DESC
		,df.ProcessId
		,df.[Priority]
END