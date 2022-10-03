
CREATE PROCEDURE [dbo].[Report_GetEmployees] @ActorId INT
	,@Query NVARCHAR(100) = NULL
	,@PeriodIds NVARCHAR(MAX) = NULL
	,@TeamIds VARCHAR(MAX) = NULL
	,@IsLocal BIT = NULL
AS
/*
Created: 20150924@Tim.Trinh
Task: get employee by ActorId.
DB: SMSEPA
Original: @SortDirection = 'asc'/'desc'

Revisions: 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	SET @query = '%' + ISNULL(@query, '') + '%'

	CREATE TABLE #tmpTeam (TeamId INT)

	CREATE TABLE #tmpPeriod (
		PeriodId INT
		,[Name] NVARCHAR(200)
		,StartDate DATE
		)

	CREATE TABLE #tmpPeriodTemp (
		PeriodId INT
		,[Name] NVARCHAR(200)
		)

	IF @TeamIds IS NOT NULL
	BEGIN
		INSERT INTO #tmpTeam (TeamId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
	END

	INSERT INTO #tmpPeriod (
		PeriodId
		,[Name]
		,StartDate
		)
	EXECUTE dbo.EpaPeriod_GetAllPeriodIncludeImport

	INSERT INTO #tmpPeriodTemp (
		PeriodId
		,[Name]
		)
	SELECT PeriodId
		,[Name]
	FROM #tmpPeriod WITH (NOLOCK)
	WHERE @PeriodIds IS NULL
		OR PeriodId IN (
			SELECT DISTINCT val
			FROM dbo.fnc_strSplit(@PeriodIds, ',')
			)

	SELECT DISTINCT tblData.EmployeeId
		,tblData.Code
		,tblData.NickName
		,tblData.FullName
	FROM (
		SELECT DISTINCT emp.EmployeeId
			,emp.Code
			,emp.NickName
			,emp.FullName
			,emp.IsLocal
			,gr.GroupId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS pcSelf WITH (NOLOCK) ON epa.EPAId = pcSelf.EPAId
			AND pcSelf.ProcessId = 2
			AND pcSelf.StartDate <= @Today
			INNER JOIN #tmpPeriodTemp AS period ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		WHERE epa.[Status] <> 2 -- rejected
		
		UNION ALL
		SELECT DISTINCT emp.EmployeeId
			,emp.Code
			,emp.NickName
			,emp.FullName
			,emp.IsLocal
			,gr.GroupId
		FROM dbo.EPASummaryImport AS epa WITH (NOLOCK)
			INNER JOIN #tmpPeriodTemp AS period ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.Code = emp.Code
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		) AS tblData
	WHERE (
			tblData.FullName LIKE @Query
			OR tblData.NickName LIKE @Query
			)
		AND (
			@TeamIds IS NULL
			OR EXISTS (
				SELECT 1
				FROM #tmpTeam AS tmp WITH (NOLOCK)
				WHERE tblData.GroupId = tmp.TeamId
				)
			)
		AND ISNULL(@IsLocal, tblData.IsLocal) = tblData.IsLocal
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS ro WITH (NOLOCK)
				WHERE ro.EmployeeId = @ActorId
					AND ro.RoleId IN (1, 2) -- SA, HR can view all member
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empRole WITH (NOLOCK)
					INNER JOIN dbo.Employee AS emp1 WITH (NOLOCK) ON empRole.EmployeeId = emp1.EmployeeId
				WHERE empRole.RoleId = 3 -- management
					AND empRole.EmployeeId = @ActorId
					AND EXISTS (
						SELECT 1
						FROM dbo.EPA AS epa1 WITH (NOLOCK)
							INNER JOIN dbo.EPAProcess AS pc1 WITH (NOLOCK) ON epa1.EPAId = pc1.EPAId
						WHERE epa1.EmployeeId = tblData.EmployeeId
							AND pc1.EvaluatorId = empRole.EmployeeId
							AND pc1.ProcessId = 5 -- management
						)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empRole WITH (NOLOCK)
					INNER JOIN dbo.Employee AS emp1 WITH (NOLOCK) ON empRole.EmployeeId = emp1.EmployeeId
				WHERE empRole.RoleId = 4 -- reviewer
					AND empRole.EmployeeId = @ActorId
					AND EXISTS (
						SELECT 1
						FROM dbo.EPA AS epa1 WITH (NOLOCK)
							INNER JOIN dbo.EPAProcess AS pc1 WITH (NOLOCK) ON epa1.EPAId = pc1.EPAId
						WHERE epa1.EmployeeId = tblData.EmployeeId
							AND pc1.EvaluatorId = empRole.EmployeeId
							AND pc1.ProcessId = 4 -- reviewer
						)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empRole WITH (NOLOCK)
					INNER JOIN dbo.Employee AS emp1 WITH (NOLOCK) ON empRole.EmployeeId = emp1.EmployeeId
				WHERE empRole.RoleId = 5 -- evaluator
					AND empRole.EmployeeId = @ActorId
					AND EXISTS (
						SELECT 1
						FROM dbo.EPA AS epa1 WITH (NOLOCK)
							INNER JOIN dbo.EPAProcess AS pc1 WITH (NOLOCK) ON epa1.EPAId = pc1.EPAId
						WHERE epa1.EmployeeId = tblData.EmployeeId
							AND pc1.EvaluatorId = empRole.EmployeeId
							AND pc1.ProcessId = 3 -- evaluator
						)
				)
			--OR EXISTS (
			--	SELECT TOP 1 1
			--	FROM dbo.HRMGroupSupervisor AS grS WITH (NOLOCK)
			--	WHERE tblData.GroupId = grS.GroupId
			--		AND grS.OwnerId = @ActorId
			--		AND grS.ActionType = 3 -- Approver
			--	)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empRole WITH (NOLOCK)
				WHERE RoleId = 6 -- Employee can view their EPA
					AND empRole.EmployeeId = @ActorId
					AND tblData.EmployeeId = @ActorId
				)
			)
	ORDER BY tblData.NickName ASC
		,tblData.FullName ASC

	DROP TABLE #tmpTeam

	DROP TABLE #tmpPeriod

	DROP TABLE #tmpPeriodTemp
END