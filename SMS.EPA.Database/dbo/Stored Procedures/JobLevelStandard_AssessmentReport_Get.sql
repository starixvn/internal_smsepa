
CREATE PROCEDURE [dbo].[JobLevelStandard_AssessmentReport_Get] @ActorId INT
	,@PeriodId INT
	,@SectionIds VARCHAR(MAX) = NULL
	,@IsLocal BIT = NULL
	,@StatusId TINYINT = NULL
AS
/*
Created: 20220901@Tim.Trinh
Task: get job level assessment report.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @PeriodStartDate DATE
		,@OldPeriodId INT
		,@EvaluatorProcessId TINYINT = 1

	CREATE TABLE #tmpSection (SectionId INT)

	SELECT @PeriodStartDate = FromDate
	FROM dbo.JobLevelStandard_PeriodSetting WITH (NOLOCK)
	WHERE PeriodId = @PeriodId
		AND ProcessId = 1

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
		,a.CurrentJobLevelId
		,a.NewJobLevelId
		,a.StatusId
		,a.Remark
	INTO #tmpAssessment
	FROM dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK) ON pes.PeriodId = pps.PeriodId
		AND pes.EmployeeId = pps.EmployeeId
		LEFT JOIN dbo.JobLevelStandard_Assessment AS a WITH (NOLOCK) ON pes.PeriodId = a.PeriodId
		AND pes.EmployeeId = a.EmployeeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pes.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.[Group] AS team WITH (NOLOCK) ON emp.GroupId = team.GroupId
		AND team.IsDeleted = 0
		INNER JOIN dbo.Section AS section WITH (NOLOCK) ON team.DepartmentId = section.SectionId
		AND section.IsActive = 1
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON section.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
	WHERE pes.PeriodId = @PeriodId
		AND pes.IsExcluded = 0
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS ro WITH (NOLOCK)
				WHERE ro.EmployeeId = @ActorId
					AND ro.RoleId IN (1, 2) -- SA, HR
				)
			OR pps.EvaluatorId = @ActorId
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
			ISNULL(@SectionIds, '') = ''
			OR EXISTS (
				SELECT 1
				FROM #tmpSection AS tmp WITH (NOLOCK)
				WHERE section.SectionId = tmp.SectionId
				)
			)
		AND (
			@IsLocal IS NULL
			OR emp.IsLocal = @IsLocal
			)
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
		,a.TotalWeight
		,a.CurrentJobLevelId
		,a.NewJobLevelId
		,a.StatusId
	INTO #tmpOldAssessment
	FROM dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK) ON pes.PeriodId = pps.PeriodId
		AND pes.EmployeeId = pps.EmployeeId
		LEFT JOIN dbo.JobLevelStandard_Assessment AS a WITH (NOLOCK) ON pes.PeriodId = a.PeriodId
		AND pes.EmployeeId = a.EmployeeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pes.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
	WHERE pes.PeriodId = @OldPeriodId
		AND EXISTS (
			SELECT 1
			FROM #tmpAssessment AS tmpa WITH (NOLOCK)
			WHERE pes.EmployeeId = tmpa.EmployeeId
				AND pes.TemplateId = tmpa.TemplateId
			)

	SELECT a.EmployeeId
		,t.TemplateId
		,t.Name Template
		,kpi.KPIId
		,akpi.[Level]
		,akpi.[Weight]
	INTO #tmpOldWeight
	FROM #tmpOldAssessment AS a WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_Template AS t WITH (NOLOCK) ON a.TemplateId = t.TemplateId
		INNER JOIN dbo.JobLevelStandard_TemplateKPI AS tkpi WITH (NOLOCK) ON t.TemplateId = tkpi.TemplateId
		INNER JOIN dbo.JobLevelStandard_KPI AS kpi WITH (NOLOCK) ON tkpi.KPIId = kpi.KPIId
		LEFT JOIN dbo.JobLevelStandard_AssessmentKPI AS akpi WITH (NOLOCK) ON a.AssessmentId = akpi.AssessmentId
		AND kpi.KPIId = akpi.KPIId
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
		,oa.TotalWeight OldTotalWeight
		,a.CurrentJobLevelId
		,cjl.[Name] AS CurrentJobLevel
		,a.StatusId
		,a.Remark
		,@PeriodStartDate AS PeriodStartDate
		,a.MilitaryFromDate
		,a.MilitaryToDate
	FROM #tmpAssessment AS a WITH (NOLOCK)
		LEFT JOIN dbo.JobLevel AS cjl WITH (NOLOCK) ON a.CurrentJobLevelId = cjl.JobLevelId
		LEFT JOIN #tmpOldAssessment AS oa WITH (NOLOCK) ON oa.EmployeeId = a.EmployeeId AND oa.TemplateId = a.TemplateId
		INNER JOIN dbo.JobLevelStandard_Template AS t WITH (NOLOCK) ON a.TemplateId = t.TemplateId
	ORDER BY a.Department
		,a.Section
		,a.Team

	SELECT a.EmployeeId
		,kpi.KPIId
		,CASE 
			WHEN akpi.[Name] IS NOT NULL THEN akpi.[Name]
			ELSE kpi.[Name]
			END AS KPIName
		,tkpi.Weight TemplateWeight
		,akpi.[Level]
		,akpi.[Weight]
		,ow.[Level] OldLevel
		,ow.[Weight] OldWeight
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
		LEFT JOIN #tmpOldWeight as ow ON ow.EmployeeId = a.EmployeeId 
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

	SELECT pps.EmployeeId
		,eva.EmployeeId AS EvaluatorId
		,eva.Nickname
		,eva.FullName
		,joblevel.[Name] AS JobLevel
	FROM dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK)
		INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON pps.EvaluatorId = eva.EmployeeId
			AND eva.[Enabled] = 1
			AND eva.IsDeleted = 0
		LEFT JOIN dbo.JobLevel AS joblevel WITH (NOLOCK) ON eva.JobLevelId = joblevel.JobLevelId
	WHERE pps.PeriodId = @PeriodId
		AND pps.ProcessId = 1
		AND EXISTS (
			SELECT 1
			FROM #tmpAssessment AS tmp WITH (NOLOCK)
			WHERE pps.PeriodId = tmp.PeriodId
				AND pps.EmployeeId = tmp.EmployeeId
			)
	ORDER BY pps.EmployeeId

	DROP TABLE #tmpSection

	DROP TABLE #tmpAssessment
END