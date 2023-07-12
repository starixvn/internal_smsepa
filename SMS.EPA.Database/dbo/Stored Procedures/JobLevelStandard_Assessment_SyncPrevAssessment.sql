
CREATE PROCEDURE [dbo].[JobLevelStandard_Assessment_SyncPrevAssessment] @PeriodId INT
AS
/*
Created: 20230704@Tim.Trinh
Task: sync prev assessment.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@PeriodStartDate DATE
		,@EvaluatorProcessId TINYINT = 1
		,@PendingStatusId TINYINT = 1
		,@ApprovedStatusId TINYINT = 3
		,@OldPeriodId INT

	CREATE TABLE #tmpDisciplineScore (EmployeeId INT PRIMARY KEY, Score1Year FLOAT, Score2Year FLOAT, Score3Year FLOAT, Score4Year FLOAT)
	CREATE TABLE #tmpInsertedAssessment (AssessmentId INT, EmployeeId INT)
	CREATE TABLE #tmpTotalWeight (AssessmentId INT, TotalWeight DECIMAL(7, 2))

	SELECT @PeriodStartDate = FromDate
	FROM dbo.JobLevelStandard_PeriodSetting WITH (NOLOCK)
	WHERE PeriodId = @PeriodId
		AND ProcessId = @EvaluatorProcessId

	SELECT DISTINCT emp.EmployeeId
		,emp.JoinedDate
		,emp.JobLevelId
		,pes.TemplateId
		,kpi.KPIId
		,kpi.[Name]
		,tkpi.[Weight] AS TemplateWeight
		,0 AS [Level]
		,0 AS [Weight]
		,tkpi.DisplayOrder
	INTO #tmpAssessment
	FROM dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK) ON pes.PeriodId = pps.PeriodId
		AND pes.EmployeeId = pps.EmployeeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pes.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.JobLevelStandard_Template AS t WITH (NOLOCK) ON pes.TemplateId = t.TemplateId
		INNER JOIN dbo.JobLevelStandard_TemplateKPI AS tkpi WITH (NOLOCK) ON t.TemplateId = tkpi.TemplateId
		INNER JOIN dbo.JobLevelStandard_KPI AS kpi WITH (NOLOCK) ON tkpi.KPIId = kpi.KPIId
	WHERE pes.PeriodId = @PeriodId
		AND pes.IsExcluded = 0
		AND pps.ProcessId = @EvaluatorProcessId
		AND pps.FromDate < @Today
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_Assessment AS a WITH (NOLOCK)
			WHERE pes.PeriodId = a.PeriodId
				AND pes.EmployeeId = a.EmployeeId
			)

	SELECT TOP 1 @OldPeriodId = p.PeriodId
	FROM dbo.JobLevelStandard_Period AS p WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodSetting AS ps WITH (NOLOCK) ON p.PeriodId = ps.PeriodId
	WHERE ps.FromDate < @PeriodStartDate
		AND ps.ProcessId = @EvaluatorProcessId
		AND p.IsActive = 1
	ORDER BY ps.FromDate DESC

	UPDATE tmpa
	SET tmpa.[Level] = akpi.[Level]
		,tmpa.[Weight] = akpi.[Weight]
	FROM #tmpAssessment AS tmpa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK) ON pes.EmployeeId = tmpa.EmployeeId
		AND pes.TemplateId = tmpa.TemplateId
		INNER JOIN dbo.JobLevelStandard_Assessment AS a WITH (NOLOCK) ON pes.PeriodId = a.PeriodId
		AND pes.EmployeeId = a.EmployeeId
		INNER JOIN dbo.JobLevelStandard_AssessmentKPI AS akpi WITH (NOLOCK) ON a.AssessmentId = akpi.AssessmentId
	WHERE a.PeriodId = @OldPeriodId
		AND a.StatusId = @ApprovedStatusId
		AND pes.IsExcluded = 0

	-- discipline score
	DECLARE @From1Date DATE
		,@From2Date DATE
		,@From3Date DATE
		,@From4Date DATE
		,@ToDate DATE

	SET @ToDate = @PeriodStartDate
	SET @From1Date = CAST(YEAR(@ToDate) AS NVARCHAR(10)) + '-01-01'
	SET @From2Date = DATEADD(YEAR, -1, @From1Date)
	SET @From3Date = DATEADD(YEAR, -2, @From1Date)
	SET @From4Date = DATEADD(YEAR, -3, @From1Date)

	INSERT INTO #tmpDisciplineScore (EmployeeId, Score1Year, Score2Year, Score3Year, Score4Year)
	SELECT tmp.EmployeeId
		,SUM(CASE WHEN dis.WorkDate BETWEEN @From1Date AND @ToDate THEN dis.SystemSubtractScore ELSE 0 END)
		,SUM(CASE WHEN dis.WorkDate BETWEEN @From2Date AND @ToDate THEN dis.SystemSubtractScore ELSE 0 END)
		,SUM(CASE WHEN dis.WorkDate BETWEEN @From3Date AND @ToDate THEN dis.SystemSubtractScore ELSE 0 END)
		,SUM(CASE WHEN dis.WorkDate BETWEEN @From4Date AND @ToDate THEN dis.SystemSubtractScore ELSE 0 END)
	FROM #tmpAssessment AS tmp WITH (NOLOCK)
		LEFT JOIN dbo.DisciplineSummary AS dis WITH (NOLOCK) ON tmp.EmployeeId = dis.EmployeeId
		AND dis.WorkDate BETWEEN @From4Date AND @ToDate
	GROUP BY tmp.EmployeeId

	UPDATE akpi
	SET akpi.[Level] = CASE WHEN YEAR(akpi.JoinedDate) <= YEAR(@From4Date) AND dis.Score4Year = 0 THEN 5
			WHEN YEAR(akpi.JoinedDate) = YEAR(@From3Date) AND dis.Score3Year = 0 THEN 4
			WHEN YEAR(akpi.JoinedDate) = YEAR(@From2Date) AND dis.Score2Year = 0 THEN 3
			WHEN YEAR(akpi.JoinedDate) = YEAR(@From1Date) AND dis.Score1Year <= 3 AND dis.Score1Year >= 0 THEN 2
			WHEN YEAR(akpi.JoinedDate) = YEAR(@From1Date) AND dis.Score1Year <= 7 AND dis.Score1Year > 3 THEN 1
			WHEN YEAR(akpi.JoinedDate) = YEAR(@From1Date) AND dis.Score1Year > 7 THEN 0
			ELSE 0 END
	FROM #tmpAssessment AS akpi WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpDisciplineScore AS dis WITH (NOLOCK) ON akpi.EmployeeId = dis.EmployeeId
	WHERE akpi.[Name] = 'Discipline Score'

	UPDATE akpi
	SET akpi.[Weight] = ROUND(akpi.[Level] / 5 * akpi.TemplateWeight, 4, 1)
	FROM #tmpAssessment AS akpi WITH (ROWLOCK, UPDLOCK)
	WHERE akpi.[Name] = 'Discipline Score'

	-- insert data
	DELETE a
	FROM #tmpAssessment AS a WITH (ROWLOCK, UPDLOCK)
	WHERE NOT EXISTS (
		SELECT 1
		FROM #tmpAssessment AS tmp WITH (NOLOCK)
		WHERE a.EmployeeId = tmp.EmployeeId
			AND tmp.[Level] > 0
		)

	INSERT INTO dbo.JobLevelStandard_Assessment (EmployeeId, PeriodId, StatusId, CurrentJobLevelId)
	OUTPUT INSERTED.AssessmentId, INSERTED.EmployeeId
	INTO #tmpInsertedAssessment (AssessmentId, EmployeeId)
	SELECT DISTINCT a.EmployeeId, @PeriodId, @PendingStatusId, a.JobLevelId
	FROM #tmpAssessment AS a WITH (NOLOCK)

	INSERT INTO dbo.JobLevelStandard_AssessmentKPI (AssessmentId, KPIId, [Name], [Level], [Weight], DisplayOrder)
	SELECT DISTINCT ia.AssessmentId, akpi.KPIId, akpi.[Name], akpi.[Level], akpi.[Weight], akpi.DisplayOrder
	FROM #tmpInsertedAssessment AS ia WITH (NOLOCK)
		INNER JOIN #tmpAssessment AS akpi WITH (NOLOCK) ON ia.EmployeeId = akpi.EmployeeId

	INSERT INTO #tmpTotalWeight (AssessmentId, TotalWeight)
	SELECT a.AssessmentId, TotalWeight = ROUND(SUM(akpi.[Weight]), 2)
	FROM dbo.JobLevelStandard_Assessment AS a WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpInsertedAssessment AS tmpa WITH (NOLOCK) ON a.AssessmentId = tmpa.AssessmentId
		INNER JOIN dbo.JobLevelStandard_AssessmentKPI AS akpi WITH (NOLOCK) ON a.AssessmentId = akpi.AssessmentId
	GROUP BY a.AssessmentId

	UPDATE a
	SET a.TotalWeight = tmp.TotalWeight
	FROM dbo.JobLevelStandard_Assessment AS a WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpTotalWeight AS tmp WITH (NOLOCK) ON a.AssessmentId = tmp.AssessmentId

	DROP TABLE #tmpAssessment

	DROP TABLE #tmpDisciplineScore

	DROP TABLE #tmpInsertedAssessment

	DROP TABLE #tmpTotalWeight
END