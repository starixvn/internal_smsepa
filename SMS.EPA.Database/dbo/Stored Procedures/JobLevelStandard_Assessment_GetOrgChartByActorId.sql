
CREATE PROCEDURE [dbo].[JobLevelStandard_Assessment_GetOrgChartByActorId] @ActorId INT
	,@PeriodId INT = NULL
AS
/*
Created: 20220816@Tim.Trinh
Task: get org-chart.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	CREATE TABLE #tmpTeam (
		TeamId INT
		,Code VARCHAR(20)
		,[Name] NVARCHAR(150)
		,SectionId INT
		)

	CREATE TABLE #tmpSection (
		SectionId INT
		,Code VARCHAR(50)
		,[Name] NVARCHAR(200)
		,DepartmentId INT
		)

	CREATE TABLE #tmpDepartment (
		DepartmentId INT
		,Code VARCHAR(50)
		,[Name] NVARCHAR(200)
		)

	INSERT INTO #tmpTeam (
		TeamId
		,Code
		,[Name]
		,SectionId
		)
	SELECT DISTINCT team.GroupId
		,team.Code
		,team.[Name]
		,team.DepartmentId AS SectionId
	FROM dbo.[Group] AS team WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON team.GroupId = emp.GroupId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK) ON emp.EmployeeId = pes.EmployeeId
		AND pes.IsExcluded = 0
	WHERE team.IsDeleted = 0
		AND (@PeriodId IS NULL OR pes.PeriodId = @PeriodId)
		AND EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK)
			WHERE pes.PeriodId = pps.PeriodId
				AND pes.EmployeeId = pps.EmployeeId
				AND pps.EvaluatorId = @ActorId
				AND pps.FromDate <= @Today
			)

	INSERT INTO #tmpSection (
		SectionId
		,Code
		,[Name]
		,DepartmentId
		)
	SELECT s.DepartmentId
		,s.Code
		,s.[Name]
		,s.ParentId
	FROM dbo.Department AS s WITH (NOLOCK)
	WHERE s.IsActive = 1
		AND EXISTS (
			SELECT 1
			FROM #tmpTeam AS t WITH (NOLOCK)
			WHERE s.DepartmentId = t.SectionId
			)

	INSERT INTO #tmpDepartment (
		DepartmentId
		,Code
		,[Name]
		)
	SELECT dept.DepartmentId
		,dept.Code
		,dept.[Name]
	FROM dbo.Department AS dept WITH (NOLOCK)
	WHERE dept.IsActive = 1
		AND EXISTS (
			SELECT 1
			FROM #tmpSection AS s WITH (NOLOCK)
			WHERE dept.DepartmentId = s.DepartmentId
			)

	SELECT DepartmentId
		,Code
		,[Name]
	FROM #tmpDepartment WITH (NOLOCK)
	ORDER BY [Name]

	SELECT SectionId
		,Code
		,[Name]
		,DepartmentId
	FROM #tmpSection WITH (NOLOCK)
	ORDER BY [Name]

	SELECT TeamId
		,Code
		,[Name]
		,SectionId
	FROM #tmpTeam WITH (NOLOCK)
	ORDER BY [Name]

	DROP TABLE #tmpTeam

	DROP TABLE #tmpSection

	DROP TABLE #tmpDepartment
END