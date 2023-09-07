
CREATE PROCEDURE [dbo].[JobLevelStandard_Assessment_GetByActorId] @ActorId INT
	,@PeriodId INT
	,@SectionIds VARCHAR(MAX) = NULL
	,@StatusId TINYINT = NULL
AS
/*
Created: 20220816@Tim.Trinh
Task: get job level assessment by actor.
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
		,@OldPeriodId INT

	SELECT @PeriodStartDate = FromDate
	FROM dbo.JobLevelStandard_PeriodSetting WITH (NOLOCK)
	WHERE PeriodId = @PeriodId
		AND ProcessId = @EvaluatorProcessId

	CREATE TABLE #tmpSection (SectionId INT)

	IF ISNULL(@SectionIds, '') != ''
	BEGIN
		INSERT INTO #tmpSection (SectionId)
		SELECT val
		FROM dbo.fnc_strSplit(@SectionIds, ',')
	END

	SELECT DISTINCT emp.EmployeeId
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,team.[Name] AS Team
		,section.SectionId
		,section.[Name] AS Section
		,dept.[Name] AS Department
		,emp.JoinedDate
		,emp.MilitaryFromDate
		,emp.MilitaryToDate
		,pes.PeriodId
		,pes.TemplateId
		,a.AssessmentId
		,a.TotalWeight
		,CASE 
			WHEN a.AssessmentId IS NULL THEN emp.JobLevelId
			WHEN pps.ProcessId = 1
				AND @Today BETWEEN pps.FromDate
					AND pps.ToDate THEN emp.JobLevelId
			ELSE a.CurrentJobLevelId
			END AS CurrentJobLevelId
		,a.NewJobLevelId
		,a.StatusId
		,a.Remark
	INTO #tmpAssessment
	FROM dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK) ON pes.PeriodId = pps.PeriodId
		AND pes.EmployeeId = pps.EmployeeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pes.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.[Group] AS team WITH (NOLOCK) ON emp.GroupId = team.GroupId
		AND team.IsDeleted = 0
		INNER JOIN dbo.Section AS section WITH (NOLOCK) ON team.DepartmentId = section.SectionId
		AND section.IsActive = 1
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON section.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
		LEFT JOIN dbo.JobLevelStandard_Assessment AS a WITH (NOLOCK) ON pes.PeriodId = a.PeriodId
		AND pes.EmployeeId = a.EmployeeId
	WHERE pes.PeriodId = @PeriodId
		AND (
			pps.EvaluatorId = @ActorId
			OR EXISTS (
				SELECT 1
				FROM dbo.Employee AS actor WITH (NOLOCK)
					INNER JOIN dbo.[Group] AS ateam WITH (NOLOCK) ON actor.GroupId = ateam.GroupId
				WHERE actor.EmployeeId = @ActorId
					AND (
						ateam.GroupId = team.GroupId
						OR ateam.DepartmentId = section.SectionId
						)
				)
			)
		AND (
			@SectionIds IS NULL
			OR EXISTS (
				SELECT 1
				FROM #tmpSection AS tmp WITH (NOLOCK)
				WHERE section.SectionId = tmp.SectionId
				)
			)
		AND pps.FromDate <= @Today
		AND pes.IsExcluded = 0
		AND (
			@StatusId IS NULL
			OR a.StatusId = @StatusId
			)

	SELECT TOP 1 @OldPeriodId = p.PeriodId
	FROM dbo.JobLevelStandard_Period AS p WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodSetting AS ps WITH (NOLOCK) ON p.PeriodId = ps.PeriodId
	WHERE ps.FromDate < @PeriodStartDate
		AND ps.ProcessId = @EvaluatorProcessId
		AND p.IsActive = 1
	ORDER BY FromDate DESC

	SELECT DISTINCT emp.EmployeeId
		,pes.TemplateId
		,a.AssessmentId
		,CASE 
			WHEN pps.ProcessId = 1
				AND @Today BETWEEN pps.FromDate
					AND pps.ToDate THEN emp.JobLevelId
			ELSE a.CurrentJobLevelId
			END AS CurrentJobLevelId
		,a.NewJobLevelId
		,a.StatusId
		,a.Remark
		,a.TotalWeight
	INTO #tmpOldAssessment
	FROM dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK) ON pes.PeriodId = pps.PeriodId
		AND pes.EmployeeId = pps.EmployeeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pes.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		LEFT JOIN dbo.JobLevelStandard_Assessment AS a WITH (NOLOCK) ON pes.PeriodId = a.PeriodId
		AND pes.EmployeeId = a.EmployeeId
	WHERE pes.PeriodId = @OldPeriodId
		AND EXISTS (
			SELECT 1
			FROM #tmpAssessment AS tmpa WITH (NOLOCK)
			WHERE pes.EmployeeId = tmpa.EmployeeId
				AND pes.TemplateId = tmpa.TemplateId
			)
		AND pps.FromDate <= @Today
		AND pes.IsExcluded = 0

	SELECT a.EmployeeId
		,t.TemplateId
		,t.[Name] AS Template
		,kpi.KPIId
		,akpi.[Level]
		,akpi.[Weight]
	INTO #tmpOldWeight
	FROM #tmpOldAssessment AS a WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_Template AS t WITH (NOLOCK) ON a.TemplateId = t.TemplateId
		INNER JOIN dbo.JobLevelStandard_TemplateKPI AS tkpi WITH (NOLOCK) ON t.TemplateId = tkpi.TemplateId
		INNER JOIN dbo.JobLevelStandard_KPI AS kpi WITH (NOLOCK) ON tkpi.KPIId = kpi.KPIId
		INNER JOIN dbo.JobLevelStandard_AssessmentKPI AS akpi WITH (NOLOCK) ON a.AssessmentId = akpi.AssessmentId
		AND kpi.KPIId = akpi.KPIId

	SELECT a.EmployeeId
		,a.Code
		,a.Nickname
		,a.FullName
		,a.Team
		,a.SectionId
		,a.Section
		,a.Department
		,a.JoinedDate
		,a.PeriodId
		,a.AssessmentId
		,t.Name AS Template
		,a.TotalWeight
		,oa.TotalWeight AS OldTotalWeight
		,a.CurrentJobLevelId
		,cjl.[Name] AS CurrentJobLevel
		,a.StatusId
		,a.Remark
		,@PeriodStartDate AS PeriodStartDate
		,a.MilitaryFromDate
		,a.MilitaryToDate
	FROM #tmpAssessment AS a WITH (NOLOCK)
		LEFT JOIN dbo.JobLevel AS cjl WITH (NOLOCK) ON a.CurrentJobLevelId = cjl.JobLevelId
		LEFT JOIN #tmpOldAssessment AS oa WITH (NOLOCK) ON oa.EmployeeId = a.EmployeeId
			AND oa.TemplateId = a.TemplateId
		INNER JOIN dbo.JobLevelStandard_Template AS t WITH (NOLOCK) ON a.TemplateId = t.TemplateId
	ORDER BY Department
		,Section
		,Team

	SELECT pps.ProcessId
		,pps.EmployeeId
		,pps.EvaluatorId
		,pps.FromDate
		,pps.ToDate
	FROM dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK)
	WHERE pps.PeriodId = @PeriodId
		AND EXISTS (
			SELECT 1
			FROM #tmpAssessment AS tmp WITH (NOLOCK)
			WHERE pps.EmployeeId = tmp.EmployeeId
			)
	ORDER BY pps.EmployeeId
		,pps.ProcessId

	SELECT a.EmployeeId
		,kpi.KPIId
		,CASE 
			WHEN akpi.[Name] IS NOT NULL THEN akpi.[Name]
			ELSE kpi.[Name]
			END AS KPIName
		,tkpi.[Weight] AS TemplateWeight
		,akpi.[Level]
		,akpi.[Weight]
		,ow.[Level] AS OldLevel
		,ow.[Weight] AS OldWeight
		,CASE 
			WHEN akpi.DisplayOrder IS NOT NULL THEN akpi.DisplayOrder
			ELSE tkpi.DisplayOrder
			END AS DisplayOrder
	FROM #tmpAssessment AS a WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_Template AS t WITH (NOLOCK) ON a.TemplateId = t.TemplateId
		INNER JOIN dbo.JobLevelStandard_TemplateKPI AS tkpi WITH (NOLOCK) ON t.TemplateId = tkpi.TemplateId
		INNER JOIN dbo.JobLevelStandard_KPI AS kpi WITH (NOLOCK) ON tkpi.KPIId = kpi.KPIId
		LEFT JOIN dbo.JobLevelStandard_AssessmentKPI AS akpi WITH (NOLOCK) ON a.AssessmentId = akpi.AssessmentId
		AND kpi.KPIId = akpi.KPIId
		LEFT JOIN #tmpOldWeight AS ow ON ow.EmployeeId = a.EmployeeId
		AND ow.TemplateId = t.TemplateId
		AND ow.KPIId = kpi.KPIId
	WHERE (
			akpi.AssessmentId IS NULL
			AND kpi.IsActive = 1
			)
		OR akpi.AssessmentId IS NOT NULL
	ORDER BY a.EmployeeId
		,CASE 
			WHEN akpi.DisplayOrder IS NOT NULL THEN akpi.DisplayOrder
			ELSE tkpi.DisplayOrder
			END

	DROP TABLE #tmpSection

	DROP TABLE #tmpAssessment
END