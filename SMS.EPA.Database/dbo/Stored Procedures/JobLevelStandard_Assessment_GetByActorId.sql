
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

	SELECT @PeriodStartDate = FromDate
	FROM dbo.JobLevelStandard_PeriodSetting WITH (NOLOCK)
	WHERE PeriodId = @PeriodId
		AND ProcessId = 1

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
		,section.DepartmentId AS SectionId
		,section.[Name] AS Section
		,dept.[Name] AS Department
		,emp.JoinedDate
		,pes.PeriodId
		,pes.TemplateId
		,a.AssessmentId
		,a.TotalWeight
		,CASE 
			WHEN pps.ProcessId = 1 AND @Today BETWEEN pps.FromDate AND pps.ToDate THEN emp.JobLevelId
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
		INNER JOIN dbo.Department AS section WITH (NOLOCK) ON team.DepartmentId = section.DepartmentId
		AND section.IsActive = 1
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON section.ParentId = dept.DepartmentId
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
						OR ateam.DepartmentId = section.DepartmentId
						)
				)
			)
		AND (
			@SectionIds IS NULL
			OR EXISTS (
				SELECT 1
				FROM #tmpSection AS tmp WITH (NOLOCK)
				WHERE section.DepartmentId = tmp.SectionId
				)
			)
		AND pps.FromDate <= @Today
		AND pes.IsExcluded = 0
		AND (
			@StatusId IS NULL
			OR a.StatusId = @StatusId
			)

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
		,a.TotalWeight
		,a.CurrentJobLevelId
		,cjl.[Name] AS CurrentJobLevel
		,a.StatusId
		,a.Remark
		,@PeriodStartDate AS PeriodStartDate
	FROM #tmpAssessment AS a WITH (NOLOCK)
		LEFT JOIN dbo.JobLevel AS cjl WITH (NOLOCK) ON a.CurrentJobLevelId = cjl.JobLevelId
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
			END AS [Name]
		,akpi.[Level]
		,akpi.[Weight]
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