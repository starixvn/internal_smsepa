
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
		,@EvaluatorProcessId TINYINT = 1
		,@EmpProcessId TINYINT = 2
		,@ManagementProcessId TINYINT = 3
		,@VerifiedStatusId TINYINT = 2
		,@ApprovedStatusId TINYINT = 3

	CREATE TABLE #tmpEmployeeSetting (
		PeriodId INT
		,EmployeeId INT
		,ProcessId TINYINT
		,StatusId TINYINT
		)

	INSERT INTO #tmpEmployeeSetting (
		PeriodId
		,EmployeeId
		,ProcessId
		,StatusId
		)
	SELECT pes.PeriodId
		,emp.EmployeeId
		,pps.ProcessId
		,assessment.StatusId
	FROM dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pes.EmployeeId = emp.EmployeeId
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
		LEFT JOIN dbo.JobLevelStandard_Assessment AS assessment WITH (NOLOCK) ON pes.PeriodId = assessment.PeriodId
		AND pes.EmployeeId = assessment.EmployeeId
		INNER JOIN dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK) ON emp.EmployeeId = pps.EmployeeId
		AND pes.PeriodId = pps.PeriodId
	WHERE pes.IsExcluded = 0
		AND pps.ProcessId != @EmpProcessId
		AND pps.EvaluatorId = @EmployeeId

	SELECT tes.PeriodId
		,pe.[Name]
		,SUM(CASE 
				WHEN pps.ProcessId = @EvaluatorProcessId
					AND tes.StatusId IS NULL
					AND pps.FromDate <= @Today THEN 1
				ELSE 0
				END) AS UnfinishedByEvaluator
		,SUM(CASE 
				WHEN pps.ProcessId = @EvaluatorProcessId
					AND tes.StatusId IS NOT NULL
					AND pps.FromDate <= @Today THEN 1
				ELSE 0
				END) AS FinishedByEvaluator
		,SUM(CASE 
				WHEN pps.ProcessId = @ManagementProcessId
					AND tes.StatusId = @VerifiedStatusId
					AND pps.FromDate <= @Today THEN 1
				ELSE 0
				END) AS UnfinishedByManagement
		,SUM(CASE 
				WHEN pps.ProcessId = @ManagementProcessId
					AND tes.StatusId = @ApprovedStatusId
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
				AND ppsm.ProcessId = @ManagementProcessId
				AND ppsm.ToDate >= @Today
			)
		AND eva.EmployeeId = @EmployeeId
	GROUP BY tes.PeriodId
		,pe.[Name]
	HAVING SUM(CASE 
				WHEN (
						pps.ProcessId = @EvaluatorProcessId
						OR (
							pps.ProcessId = @ManagementProcessId
							AND (
								tes.StatusId = @VerifiedStatusId
								OR tes.StatusId = @ApprovedStatusId
								)
							)
						)
					AND pps.FromDate <= @Today THEN 1
				ELSE 0
				END) > 0

	DROP TABLE #tmpEmployeeSetting
END