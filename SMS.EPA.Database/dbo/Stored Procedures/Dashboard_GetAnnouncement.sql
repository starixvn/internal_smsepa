

CREATE PROCEDURE [dbo].[Dashboard_GetAnnouncement] @EmployeeId INT
AS
/*
Created: 2022-09-028@Tim.Trinh
Task: get announcement.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	SELECT period.PeriodId
		,period.[Name] AS PeriodName
		,pps.ProcessId
		,MIN(pps.FromDate) AS FromDate
		,MAX(pps.ToDate) AS ToDate
	FROM dbo.JobLevelStandard_Period AS period WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK) ON period.PeriodId = pes.PeriodId
		AND pes.IsExcluded = 0
		INNER JOIN dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK) ON period.PeriodId = pps.PeriodId
		AND pes.EmployeeId = pps.EmployeeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pes.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON pps.EvaluatorId = eva.EmployeeId
		AND eva.[Enabled] = 1
		AND eva.IsDeleted = 0
	WHERE eva.EmployeeId = @EmployeeId
		AND @Today BETWEEN pps.FromDate AND pps.ToDate
	GROUP BY period.PeriodId
		,period.[Name]
		,pps.ProcessId
	ORDER BY period.PeriodId
		,pps.ProcessId
END