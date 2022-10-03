
CREATE PROCEDURE [dbo].[Auto_JobLevelStandard_GetEmployeeSettingToSendMail] @PeriodId INT = NULL
AS
/*
Created: 2022-09-027@Tim.Trinh
Task: get employee setting to send mail.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	SELECT DISTINCT period.PeriodId
		,period.[Name] AS PeriodName
		,pps.ProcessId
		,pps.FromDate
		,pps.ToDate
		,pps.EvaluatorId
		,eva.NickName
		,eva.FullName
		,eva.Email
	FROM dbo.JobLevelStandard_Period AS period WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK) ON period.PeriodId = pes.PeriodId
		AND pes.IsExcluded = 0
		INNER JOIN dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK) ON period.PeriodId = pes.PeriodId
		AND pes.EmployeeId = pps.EmployeeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pes.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON pps.EvaluatorId = eva.EmployeeId
		AND eva.[Enabled] = 1
		AND eva.IsDeleted = 0
	WHERE (
			@PeriodId IS NULL
			OR period.PeriodId = @PeriodId
			)
		AND pps.FromDate = @Today
	ORDER BY period.PeriodId
		,pps.ProcessId
END