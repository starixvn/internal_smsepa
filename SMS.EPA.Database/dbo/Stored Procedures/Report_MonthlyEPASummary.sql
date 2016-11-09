
CREATE PROCEDURE [dbo].[Report_MonthlyEPASummary] @EmployeeId INT
	,@PeriodIds NVARCHAR(MAX) = NULL
	,@DepartmentId INT = NULL
	,@IsLocal BIT = NULL
	,@SortColumnName NVARCHAR(100) = NULL
	,@SortDirection NVARCHAR(10) = NULL
AS
/*
Created: 20160523@Razor.Nguyen
Task: Monthly EPA Summary Report.
DB: SMSEPA
Original: @SortDirection = 'asc'/'desc'

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #EPA (
		EPAId INT
		,EndDate DATE
		)

	CREATE TABLE #Employee (
		EmployeeId INT
		,Code NVARCHAR(10)
		,GroupName NVARCHAR(150)
		)

	CREATE TABLE #Period (
		PeriodId INT
		,[Name] NVARCHAR(200)
		,StartDate DATETIME
		)

	CREATE TABLE #PeriodTemp (
		PeriodId INT
		,[Name] NVARCHAR(200)
		)

	CREATE TABLE #DisciplineScore (
		EPAId INT
		,SystemDisciplineScore FLOAT
		,ApproverSubtractScore FLOAT
		,OtherDisciplines FLOAT
		)

	DECLARE @Ids NVARCHAR(MAX)
		,@ToDay DATE
		,@SortCode NVARCHAR(10)
		,@SortSection NVARCHAR(10)

	SET @ToDay = CAST(GETDATE() AS DATE)
	SET @SortCode = 'code'
	SET @SortSection = 'section'

	INSERT INTO #Period
	SELECT [PeriodId]
		,[Name]
		,[StartDate]
	FROM [dbo].[Period] WITH (NOLOCK)
	WHERE [IsActive] = 1
		AND TargetTypeId = 2 --Monthly
	ORDER BY [StartDate] DESC

	INSERT INTO #PeriodTemp
	SELECT PeriodId
		,[Name]
	FROM #Period
	WHERE @PeriodIds IS NULL
		OR PeriodId IN (
			SELECT DISTINCT val
			FROM fnc_strSplit(@PeriodIds, ',')
			)

	INSERT INTO #Employee
	SELECT DISTINCT tblData.EmployeeId
		,tblData.Code
		,tblData.GroupName
	FROM (
		SELECT DISTINCT emp.EmployeeId
			,emp.Code
			,emp.NickName
			,emp.IsLocal
			,dept.DepartmentId
			,gr.GroupId
			,gr.[Name] AS GroupName
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS pcSelf WITH (NOLOCK) ON epa.EPAId = pcSelf.EPAId
			AND pcSelf.ProcessId = 2
			AND CAST(pcSelf.StartDate AS DATE) <= @ToDay
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			INNER JOIN #PeriodTemp AS period ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
			LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
			INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa.PeriodId = period1.PeriodId
			INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period1.TargetTypeId = tt.TargetTypeId
		WHERE epa.[Status] <> 2 -- rejected
		) AS tblData
	WHERE (
			@DepartmentId IS NULL
			OR (
				@DepartmentId < 0
				AND tblData.GroupId = ABS(@DepartmentId)
				)
			OR (
				@DepartmentId > 0
				AND (
					tblData.DepartmentId = @DepartmentId
					OR tblData.DepartmentId IN (
						SELECT DepartmentId
						FROM dbo.RelatedDepartment(@DepartmentId)
						
						UNION
						SELECT @DepartmentId
						)
					)
				)
			)
		AND (
			@IsLocal IS NULL
			OR tblData.IsLocal = @IsLocal
			)
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS ro WITH (NOLOCK)
				WHERE ro.EmployeeId = @EmployeeId
					AND ro.RoleId IN (8) -- Section Admin can view all member
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empRole WITH (NOLOCK)
					INNER JOIN dbo.Employee AS emp1 WITH (NOLOCK) ON empRole.EmployeeId = emp1.EmployeeId
				WHERE empRole.RoleId = 4 -- reviewer
					AND empRole.EmployeeId = @EmployeeId
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
					AND empRole.EmployeeId = @EmployeeId
					AND EXISTS (
						SELECT 1
						FROM dbo.EPA AS epa1 WITH (NOLOCK)
							INNER JOIN dbo.EPAProcess AS pc1 WITH (NOLOCK) ON epa1.EPAId = pc1.EPAId
						WHERE epa1.EmployeeId = tblData.EmployeeId
							AND pc1.EvaluatorId = empRole.EmployeeId
							AND pc1.ProcessId = 3 -- evaluator
						)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empRole WITH (NOLOCK)
				WHERE RoleId = 6 -- Employee can view their EPA
					AND empRole.EmployeeId = @EmployeeId
					AND tblData.EmployeeId = @EmployeeId
				)
			)
	ORDER BY tblData.GroupName
		,tblData.Code

	INSERT INTO #EPA
	SELECT DISTINCT epa.EPAId
		,MAX(CAST(pc.EndDate AS DATE)) AS EndDate
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN #Employee AS emp ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN #PeriodTemp AS period ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa.PeriodId = period1.PeriodId
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period1.TargetTypeId = tt.TargetTypeId
	GROUP BY epa.EPAId
		,period1.EndDate
		,tt.TargetTypeId

	SELECT @Ids = COALESCE(@Ids + ',', '') + CAST(EPAId AS NVARCHAR)
	FROM #EPA

	INSERT INTO #DisciplineScore
	EXECUTE dbo.EmployeeDisciplineSummary_GetByEPA @Ids

	SELECT period.PeriodId
		,period.[Name]
	FROM #PeriodTemp AS TEMP
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON TEMP.PeriodId = period.PeriodId
	ORDER BY period.StartDate DESC

	SELECT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.JoinedDate
		,lkd.[Name] AS Category
		,gr.GroupId
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,emp.JobTitle AS JobTitleId
		,job.[Name] AS JobTitleName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #Employee AS tblEmp ON emp.EmployeeId = tblEmp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
		LEFT JOIN dbo.LookupData AS lkd WITH (NOLOCK) ON emp.IsLocal = CAST(lkd.Value AS BIT)
		AND lkd.LookupId = 11 -- employee category
	ORDER BY CASE 
			WHEN ISNULL(@SortColumnName, '') <> ''
				AND ISNULL(@SortDirection, '') <> ''
				AND @SortDirection = 'desc' THEN CASE LOWER(@SortColumnName)
					WHEN @SortCode THEN emp.Code
					WHEN @SortSection THEN gr.[Name]
					END
			END DESC
		,CASE 
			WHEN ISNULL(@SortColumnName, '') <> ''
				AND ISNULL(@SortDirection, '') <> ''
				AND @SortDirection = 'asc' THEN CASE LOWER(@SortColumnName)
					WHEN @SortCode THEN emp.Code
					WHEN @SortSection THEN gr.[Name]
					END
			END

	SELECT DISTINCT tbl.*
	FROM (
		SELECT DISTINCT emp.EmployeeId
			,period.PeriodId
			,(CASE srt.ScoreRatingTypeId
				WHEN 1 THEN 1
				ELSE 0 END
				) AS HasDisciplineScore
			,(CASE 
				WHEN epa.[Status] = 1 THEN epa.DisciplineScore
				ELSE (CASE 
						WHEN ds.ApproverSubtractScore IS NULL THEN 0
						ELSE ds.ApproverSubtractScore END
						) END
				) AS DisciplineScore
			,srt.Attendance AS MaxDisciplineScore
			,(CASE 
				WHEN CAST(epa1.EndDate AS DATE) < @ToDay THEN 1
				ELSE 0 END
				) AS IsEndPeriod
			,epa.FinalScore
			,epa.Grade
			,epa.[Status] AS StatusId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN #EPA AS epa1 ON epa.EPAId = epa1.EPAId
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			INNER JOIN #Employee AS tblEmp ON emp.EmployeeId = tblEmp.EmployeeId
			INNER JOIN #Period AS period1 ON epa.PeriodId = period1.PeriodId
			INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
			LEFT JOIN #DisciplineScore AS ds ON epa.EPAId = ds.EPAId
		WHERE epa.[Status] <> 2 -- rejected
		) AS tbl

	DROP TABLE #EPA

	DROP TABLE #Employee

	DROP TABLE #Period

	DROP TABLE #PeriodTemp

	DROP TABLE #DisciplineScore
END