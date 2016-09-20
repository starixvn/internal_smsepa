
CREATE PROCEDURE [dbo].[SettingDepartmentFlow_GetById] @DepartmentId INT
	,@FlowTemplateId INT
	,@IsGroup BIT
AS
/*
Created: 20150717@Tim.Trinh
Task: Get department flow setting.
DB: SMSEPA
Original:

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	SELECT df.ProcessId
		,df.[Priority]
		,df.Duration
		,df.HasSendMail
		,df.RemindDay
		,ft.FlowTemplateId
		,ft.[Name]
		,df.EvaluatorId AS EmployeeId
		,e.Code
		,e.NickName
		,e.FullName
		,e.Email
	FROM dbo.SettingDepartmentFlow AS df WITH (NOLOCK)
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON df.EvaluatorId = e.EmployeeId
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON df.FlowTemplateId = ft.FlowTemplateId
	WHERE df.DepartmentId = @DepartmentId
		AND df.FlowTemplateId = @FlowTemplateId
		AND df.IsGroup = @IsGroup
	ORDER BY df.ProcessId
		,df.[Priority]
END