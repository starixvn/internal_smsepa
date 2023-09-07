
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
		,ParentId INT
		)

	INSERT INTO #tmpTeam (
		TeamId
		,Code
		,[Name]
		,SectionId
		)
	SELECT t.GroupId
		,t.Code
		,t.[Name]
		,CASE 
			WHEN t.DepartmentId IS NOT NULL THEN t.DepartmentId
			ELSE 0
			END
	FROM dbo.[Group] AS t WITH (NOLOCK)
		LEFT JOIN dbo.Section AS s WITH (NOLOCK) ON t.DepartmentId = s.SectionId
		AND s.IsActive = 1
	WHERE t.IsDeleted = 0

	INSERT INTO #tmpSection (
		SectionId
		,Code
		,[Name]
		,DepartmentId
		)
	SELECT s.SectionId
		,s.Code
		,s.[Name]
		,CASE 
			WHEN s.DepartmentId IS NOT NULL THEN s.DepartmentId
			ELSE 0
			END
	FROM dbo.Section AS s WITH (NOLOCK)
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON s.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
	WHERE s.IsActive = 1
		AND EXISTS (
			SELECT 1
			FROM #tmpTeam AS t WITH (NOLOCK)
			WHERE s.SectionId = t.SectionId
			)

	IF EXISTS (
			SELECT 1
			FROM #tmpTeam WITH (NOLOCK)
			WHERE SectionId = 0
			)
	BEGIN
		INSERT INTO #tmpSection (
			SectionId
			,Code
			,[Name]
			,DepartmentId
			)
		VALUES (
			0
			,''
			,''
			,0
			)
	END

	INSERT INTO #tmpDepartment (
		DepartmentId
		,Code
		,[Name]
		,ParentId
		)
	SELECT dept.DepartmentId
		,dept.Code
		,dept.[Name]
		,CASE 
			WHEN u.UnitId IS NOT NULL THEN u.UnitId
			ELSE 0
			END
	FROM dbo.Department_New AS dept WITH (NOLOCK)
		LEFT JOIN dbo.Unit AS u WITH (NOLOCK) ON dept.UnitId = u.UnitId
		AND u.IsActive = 1
	WHERE dept.IsActive = 1
		AND EXISTS (
			SELECT 1
			FROM #tmpSection AS s WITH (NOLOCK)
			WHERE dept.DepartmentId = s.DepartmentId
			)

	IF EXISTS (
			SELECT 1
			FROM #tmpSection WITH (NOLOCK)
			WHERE DepartmentId = 0
			)
	BEGIN
		INSERT INTO #tmpDepartment (
			DepartmentId
			,Code
			,[Name]
			,ParentId
			)
		VALUES (
			0
			,''
			,''
			,0
			)
	END

	SELECT DepartmentId
		,Code
		,[Name]
		,ParentId
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