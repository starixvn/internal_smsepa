
CREATE PROCEDURE [dbo].[JobLevelStandard_PeriodEmployeeSetting_Get] @PeriodId INT
	,@TeamIds VARCHAR(MAX) = NULL
	,@IsLocal BIT = NULL
	,@IsExcluded BIT = NULL
	,@StatusId TINYINT = NULL
AS
/*
Created: 20220816@Tim.Trinh
Task: get period employee setting
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tmpTeam (TeamId INT)

	IF ISNULL(@TeamIds, '') != ''
	BEGIN
		INSERT INTO #tmpTeam (TeamId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
	END
	ELSE
	BEGIN
		INSERT INTO #tmpTeam (TeamId)
		SELECT GroupId
		FROM dbo.[Group] WITH (NOLOCK)
		WHERE IsDeleted = 0
	END

	SELECT pes.PeriodId
		,emp.EmployeeId
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,emp.JoinedDate
		,(CASE 
			WHEN emp.IsLocal = 1 THEN 'Local'
			ELSE 'Expat' END
			) AS Category
		,team.[Name] AS Team
		,section.[Name] AS Section
		,dept.[Name] AS Department
		,pes.IsExcluded
		,assessment.AssessmentId
		,assessment.StatusId
		--,assessment.CurrentJobLevelId
		,CASE 
			WHEN assessment.CurrentJobLevelId IS NOT NULL
				AND assessment.CurrentJobLevelId != 0 THEN assessment.CurrentJobLevelId
			ELSE emp.JobLevelId
			END AS CurrentJobLevelId
		,t.TemplateId
		,t.[Name] AS Template
	INTO #tmpEmployeeSetting
	FROM dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pes.EmployeeId = emp.EmployeeId
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
		INNER JOIN dbo.[Group] AS team WITH (NOLOCK) ON emp.GroupId = team.GroupId
			AND team.IsDeleted = 0
		LEFT JOIN dbo.JobLevel AS joblevel WITH (NOLOCK) ON emp.JobLevelId = joblevel.JobLevelId
		LEFT JOIN dbo.Section AS section WITH (NOLOCK) ON team.DepartmentId = section.SectionId
			AND section.IsActive = 1
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON section.DepartmentId = dept.DepartmentId
			AND dept.IsActive = 1
		LEFT JOIN dbo.JobLevelStandard_Assessment AS assessment WITH (NOLOCK) ON pes.PeriodId = assessment.PeriodId
			AND pes.EmployeeId = assessment.EmployeeId
		LEFT JOIN dbo.JobLevelStandard_Template AS t WITH (NOLOCK) ON t.TemplateId = pes.TemplateId 
	WHERE pes.PeriodId = @PeriodId
		AND EXISTS (
			SELECT 1
			FROM #tmpTeam AS tmp WITH (NOLOCK)
			WHERE team.GroupId = tmp.TeamId
			)
		AND (
			@IsExcluded IS NULL
			OR pes.IsExcluded = @IsExcluded
			)
		AND (
			@IsLocal IS NULL
			OR emp.IsLocal = @IsLocal
			)
		AND (
			@StatusId IS NULL
			OR assessment.StatusId = @StatusId
			)

	SELECT a.PeriodId
		,a.EmployeeId
		,a.Code
		,a.Nickname
		,a.FullName
		,jl.[Name] AS JobLevel
		,a.JoinedDate
		,a.Category
		,a.Team
		,a.Section
		,a.Department
		,a.IsExcluded
		,a.AssessmentId
		,a.StatusId
		,a.TemplateId
		,a.Template
	FROM #tmpEmployeeSetting AS a WITH (NOLOCK)
		LEFT JOIN dbo.JobLevel AS jl WITH (NOLOCK) ON a.CurrentJobLevelId = jl.JobLevelId
	ORDER BY a.Department
		,a.Section
		,a.Team
		,a.Nickname

	SELECT pps.EmployeeId
		,pps.ProcessId
		,pps.FromDate
		,pps.ToDate
		,eva.EmployeeId AS EvaluatorId
		,eva.Code
		,eva.Nickname
		,eva.FullName
		,joblevel.NAME AS JobLevel
	FROM dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK)
		INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON pps.EvaluatorId = eva.EmployeeId
		AND eva.[Enabled] = 1
		AND eva.IsDeleted = 0
		LEFT JOIN dbo.JobLevel AS joblevel WITH (NOLOCK) ON eva.JobLevelId = joblevel.JobLevelId
	WHERE pps.PeriodId = @PeriodId
		AND EXISTS (
			SELECT 1
			FROM #tmpEmployeeSetting AS tmp WITH (NOLOCK)
			WHERE pps.EmployeeId = tmp.EmployeeId
			)

	DROP TABLE #tmpEmployeeSetting

	DROP TABLE #tmpTeam
END