
CREATE PROCEDURE [dbo].[JobLevelStandard_GetDashboard] @EmployeeId INT
AS
/*
Created: 20221215@Frankie.Nguyen
Task: get jls dashboard
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@ProcessByEvaluator TINYINT = 1
		,@ProcessByManageMent TINYINT = 3
		,@ApprovedStatus TINYINT = 3

	SELECT pes.PeriodId
		,emp.EmployeeId
		,assessment.StatusId
	INTO #tmpEmployeeSetting
	FROM dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pes.EmployeeId = emp.EmployeeId
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1
		LEFT JOIN dbo.JobLevelStandard_Assessment AS assessment WITH (NOLOCK) ON pes.PeriodId = assessment.PeriodId
		AND pes.EmployeeId = assessment.EmployeeId

	SELECT tes.PeriodId
		,pe.[Name]
		,SUM(CASE 
				WHEN pps.ProcessId = @ProcessByEvaluator
					AND tes.StatusId IS NULL
					AND pps.FromDate <= @Today THEN 1
				ELSE 0
				END) AS UnfinishedByEvaluator
		,SUM(CASE 
				WHEN pps.ProcessId = @ProcessByEvaluator
					AND tes.StatusId IS NOT NULL
					AND pps.FromDate <= @Today THEN 1
				ELSE 0
				END) AS FinishedByEvaluator
		,SUM(CASE 
				WHEN pps.ProcessId = @ProcessByManageMent
					AND tes.StatusId <> @ApprovedStatus
					AND pps.FromDate <= @Today THEN 1
				ELSE 0
				END) AS UnfinishedByManagement
		,SUM(CASE 
				WHEN pps.ProcessId = @ProcessByManageMent
					AND tes.StatusId = @ApprovedStatus
					AND pps.FromDate <= @Today THEN 1
				ELSE 0
				END) AS FinishedByManagement
	FROM dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK)
		INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON pps.EvaluatorId = eva.EmployeeId
		AND eva.[Enabled] = 1
		AND eva.IsDeleted = 0
		INNER JOIN #tmpEmployeeSetting AS tes WITH (NOLOCK) ON tes.EmployeeId = pps.EmployeeId
		AND tes.PeriodId = pps.PeriodId
		INNER JOIN dbo.JobLevelStandard_Period AS pe WITH (NOLOCK) ON tes.PeriodId = pe.PeriodId
		AND pe.IsActive = 1
	WHERE EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_PeriodProcessSetting AS ppsm WITH (NOLOCK)
			WHERE ppsm.PeriodId = pps.PeriodId
				AND ppsm.EmployeeId = pps.EmployeeId
				AND ppsm.ProcessId = @ApprovedStatus
				AND @Today <= ppsm.ToDate
			)
		AND eva.EmployeeId = @EmployeeId
	GROUP BY tes.PeriodId
		,pe.[Name]

	DROP TABLE #tmpEmployeeSetting
END