/****** Object:  Stored Procedure dbo.SettingEmployeeFlow_GetById    Script Date: 5/16/2016 2:20:08 PM ******/
/****** Object:  Stored Procedure dbo.SettingEmployeeFlow_GetById    Script Date: 5/16/2016 2:15:30 PM ******/
/****** Object:  Stored Procedure dbo.SettingEmployeeFlow_GetById    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.SettingEmployeeFlow_GetById    Script Date: 4/6/2016 11:27:47 AM ******/

CREATE PROCEDURE [dbo].[SettingEmployeeFlow_GetById] @EmployeeId INT
	,@FlowTemplateId INT
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

	SELECT ef.ProcessId
		,ef.[Priority]
		,ef.Duration
		,ef.HasSendMail
		,ef.RemindDay
		,ft.FlowTemplateId
		,ft.NAME
		,ef.EvaluatorId AS EmployeeId
		,e.Code
		,e.NickName
		,e.FullName
		,e.Email
		,(CASE 
			WHEN e.HasMonthlyEPA = 1 THEN 1
			ELSE 0 END
			) AS HasMonthlyEPA
	FROM (
		SELECT (CASE 
				WHEN EvaluatorId = - 1
					AND ProcessId = 2 THEN @EmployeeId
				ELSE EvaluatorId END
				) AS EvaluatorId
			,ProcessId
			,FlowTemplateId
			,[Priority]
			,EmployeeId
			,Duration
			,HasSendMail
			,RemindDay
		FROM dbo.SettingEmployeeFlow WITH (NOLOCK)
		) AS ef
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON ef.EvaluatorId = e.EmployeeId
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON ef.FlowTemplateId = ft.FlowTemplateId
	WHERE ef.EmployeeId = @EmployeeId
		AND ef.FlowTemplateId = @FlowTemplateId
	ORDER BY ef.ProcessId
		,ef.[Priority]
END