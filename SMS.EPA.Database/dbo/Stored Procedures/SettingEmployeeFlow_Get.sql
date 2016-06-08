/****** Object:  Stored Procedure dbo.SettingEmployeeFlow_Get    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.SettingEmployeeFlow_Get    Script Date: 5/16/2016 2:15:32 PM ******/
/****** Object:  Stored Procedure dbo.SettingEmployeeFlow_Get    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.SettingEmployeeFlow_Get    Script Date: 4/6/2016 11:27:47 AM ******/

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

	SELECT DISTINCT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,gr.GroupId
		,gr.Code AS GroupCode
		,gr.NAME AS GroupName
	FROM dbo.SettingEmployeeFlow AS ef WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON ef.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON ef.FlowTemplateId = ft.FlowTemplateId
		AND ft.FlowTemplateId = @FlowTemplateId

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
	ORDER BY ef.EmployeeId
		,ef.ProcessId
		,ef.[Priority]
END