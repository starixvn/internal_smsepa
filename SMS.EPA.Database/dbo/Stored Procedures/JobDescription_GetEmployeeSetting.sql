
CREATE PROCEDURE [dbo].[JobDescription_GetEmployeeSetting] @TeamIds VARCHAR(MAX) = NULL
	,@EmployeeId INT = NULL
	,@ShowAll BIT = 0
AS
/*
Created: 20210426@Tim.Trinh
Task: job description - get employee setting
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tmpTeam (TeamId INT)

	IF @TeamIds IS NOT NULL
	BEGIN
		INSERT INTO #tmpTeam (TeamId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
	END

	SELECT emp.EmployeeId
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,emp.[Enabled]
		,dept.[Name] AS Department
		,sec.[Name] AS Section
		,gr.[Name] AS GroupName
		,gr.Code AS GroupCode
		,job.[Name] AS JobLevel
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.JobLevel AS job WITH (NOLOCK) ON emp.JobLevelId = job.JobLevelId
		LEFT JOIN dbo.Section AS sec WITH (NOLOCK) ON gr.DepartmentId = sec.SectionId
		AND sec.IsActive = 1
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON sec.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND (
			@TeamIds IS NULL
			OR EXISTS (
				SELECT 1
				FROM #tmpTeam AS tmp WITH (NOLOCK)
				WHERE gr.GroupId = tmp.TeamId
				)
			)
		AND (
			@EmployeeId IS NULL
			OR emp.EmployeeId = @EmployeeId
			)
		AND (
			(
				@ShowAll = 0
				AND EXISTS (
					SELECT 1
					FROM dbo.JobDescription_EmployeeSetting AS setting WITH (NOLOCK)
					WHERE emp.EmployeeId = setting.EmployeeId
					)
				)
			OR @ShowAll = 1
			)
	ORDER BY gr.[Name]
		,emp.Code
		,emp.NickName
END