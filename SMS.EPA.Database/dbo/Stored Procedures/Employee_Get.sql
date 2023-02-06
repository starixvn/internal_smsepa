
CREATE PROCEDURE [dbo].[Employee_Get] @Query NVARCHAR(100) = NULL
	,@GroupIds VARCHAR(MAX) = NULL
	,@IsLocal BIT = NULL
	,@IsSpecialUser BIT = NULL
	,@IsDisabledUser BIT = NULL
	,@HasMonthlyEPA BIT = NULL
	,@JobLevelId INT = NULL
AS
/*
Created: 2022-09-14@Tim.Trinh
Task: Get employee
DB: SMS EPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET @query = '%' + ISNULL(@query, '') + '%'

	CREATE TABLE #tmpGroup (GroupId INT PRIMARY KEY)

	CREATE TABLE #tmpEmployee (EmployeeId INT PRIMARY KEY)

	IF @GroupIds IS NOT NULL
	BEGIN
		INSERT INTO #tmpGroup (GroupId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')
	END

	INSERT INTO #tmpEmployee (EmployeeId)
	SELECT DISTINCT emp.EmployeeId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE emp.IsDeleted = 0
		AND (
			@GroupIds IS NULL
			OR EXISTS (
				SELECT 1
				FROM #tmpGroup AS tmp WITH (NOLOCK)
				WHERE gr.GroupId = tmp.GroupId
				)
			)
		AND (
			emp.FullName LIKE @query
			OR emp.NickName LIKE @query
			)
		AND (
			@IsLocal IS NULL
			OR emp.IsLocal = emp.IsLocal
			)
		AND (
			@IsSpecialUser IS NULL
			OR emp.IsSpecialUser = @IsSpecialUser
			)
		AND (
			(
				@IsDisabledUser IS NULL
				AND emp.[Enabled] = 1
				)
			OR (
				@IsDisabledUser IS NOT NULL
				AND emp.[Enabled] = ~ @IsDisabledUser
				)
			)
		AND (
			@HasMonthlyEPA IS NULL
			OR emp.HasMonthlyEPA = @HasMonthlyEPA
			)
		AND (
			@JobLevelId IS NULL
			OR emp.JobLevelId = @JobLevelId
			)

	SELECT DISTINCT emp.EmployeeId
		,emp.Code
		,emp.DomainUsername
		,emp.NickName
		,emp.FullName
		,section.[Name] AS Section
		,emp.IsSpecialUser
		,emp.[Password]
		,emp.[Enabled]
		,emp.IsLocal
		,(CASE 
			WHEN emp.IsLocal = 1 THEN 'Local'
			ELSE 'Expat' END
			) AS Category
		,emp.[Email]
		,emp.HasMonthlyEPA
		,emp.JoinedDate
		,emp.JobTitle AS JobTitleId
		,job.[Name] AS JobTitleName
		,emp.JobLevelId AS JobLevelId
		,jl.[Name] AS JobLevelName
		,gr.GroupId
		,gr.[Name] AS GroupName
		,gr.Code AS GroupCode
		,dept.DepartmentId
		,dept.Code
		,dept.[Name]
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tmpEmployee AS tbl WITH (NOLOCK) ON emp.EmployeeId = tbl.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS section WITH (NOLOCK) ON gr.DepartmentId = section.DepartmentId
		AND section.IsActive = 1
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON section.ParentId = dept.DepartmentId
		AND dept.IsActive = 1
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
		LEFT JOIN dbo.JobLevel AS jl WITH (NOLOCK) ON emp.JobLevelId = jl.JobLevelId
		AND jl.IsDeleted = 0

	SELECT ro.EmployeeId
		,ro.RoleId
		,ref.Value AS RoleCode
	FROM dbo.EmployeeRole AS ro WITH (NOLOCK)
		INNER JOIN #tmpEmployee AS emp WITH (NOLOCK) ON ro.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON ro.RoleId = ref.ResourceId
		AND ref.RefName = 'SystemRole'

	DROP TABLE #tmpGroup

	DROP TABLE #tmpEmployee
END