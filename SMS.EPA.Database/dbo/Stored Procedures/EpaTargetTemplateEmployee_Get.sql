
CREATE PROCEDURE [dbo].[EpaTargetTemplateEmployee_Get] @TeamIds VARCHAR(MAX) = NULL
	,@EmployeeId INT = NULL
	,@TargetTypeId INT = NULL
	,@ShowAll BIT = 0
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get users and their target template.
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
		SELECT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
	END

	SELECT DISTINCT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,gr.[Name] AS Section
		,dept.[Name] AS Department
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND (
			ISNULL(@TeamIds, '') = ''
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
					FROM dbo.TargetTemplateEmployee AS tte WITH (NOLOCK)
					WHERE tte.EmployeeId = emp.EmployeeId
					)
				)
			OR @ShowAll = 1
			)
		AND (
			@TargetTypeId IS NULL
			OR EXISTS (
				SELECT 1
				FROM dbo.TargetTemplateEmployee AS tte WITH (NOLOCK)
					INNER JOIN dbo.TargetTemplate AS tt WITH (NOLOCK) ON tte.TargetTemplateId = tt.TargetTemplateId
					INNER JOIN dbo.TargetType AS ttype WITH (NOLOCK) ON tt.TargetTypeId = ttype.TargetTypeId
					AND ttype.TargetTypeId = @TargetTypeId
				WHERE tte.EmployeeId = emp.EmployeeId
				)
			)
	ORDER BY emp.EmployeeId ASC

	SELECT tte.EmployeeId
		,tte.TargetTemplateId
		,tt.[Name]
		,tt.TargetTypeId
		,ttype.[Name] AS TargetTypeName
	FROM dbo.TargetTemplateEmployee AS tte WITH (NOLOCK)
		INNER JOIN dbo.TargetTemplate AS tt WITH (NOLOCK) ON tte.TargetTemplateId = tt.TargetTemplateId
		INNER JOIN dbo.TargetType AS ttype WITH (NOLOCK) ON tt.TargetTypeId = ttype.TargetTypeId
	WHERE (
			@EmployeeId IS NULL
			OR tte.EmployeeId = @EmployeeId
			)
	ORDER BY tte.EmployeeId ASC
		,tt.TargetTypeId ASC

	DROP TABLE #tmpTeam
END
