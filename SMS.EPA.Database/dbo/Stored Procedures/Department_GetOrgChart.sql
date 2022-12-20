

CREATE PROCEDURE [dbo].[Department_GetOrgChart]
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

	CREATE TABLE #tmpTeam (
		TeamId INT
		,Code VARCHAR(200)
		,[Name] NVARCHAR(200)
		,SectionId INT
		)

	CREATE TABLE #tmpSection (
		SectionId INT
		,Code VARCHAR(200)
		,[Name] NVARCHAR(200)
		,DepartmentId INT
		)

	CREATE TABLE #tmpDepartment (
		DepartmentId INT
		,Code VARCHAR(200)
		,[Name] NVARCHAR(200)
		)

	INSERT INTO #tmpTeam (
		TeamId
		,Code
		,[Name]
		,SectionId
		)
	SELECT GroupId
		,Code
		,[Name]
		,DepartmentId AS SectionId
	FROM dbo.[Group] WITH (NOLOCK)
	WHERE IsDeleted = 0

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