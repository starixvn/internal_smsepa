
CREATE PROCEDURE [dbo].[Report_MonthlyEPADetail] @EmployeeId INT
	,@PeriodId INT = NULL
	,@PhaseIds NVARCHAR(10) = NULL
	,@DepartmentId INT = NULL
	,@IsLocal BIT = NULL
	,@SortColumnName NVARCHAR(100) = NULL
	,@SortDirection NVARCHAR(10) = NULL
	,@PageIndex INT = NULL
	,@ItemsPerPage INT = NULL
	,@TotalItem INT = NULL OUT
AS
/*
Created: 20160520@Razor.Nguyen
Task: Monthly EPA Detail Report.
DB: SMSEPA
Original: @SortDirection = 'asc'/'desc'

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Phases IntId
		,@ToDay DATE
		,@Ids NVARCHAR(MAX)
		,@SortCode NVARCHAR(10)
		,@SortSection NVARCHAR(10)

	CREATE TABLE #EPAs (
		EPAId INT
		,Code NVARCHAR(10)
		,GroupName NVARCHAR(150)
		,EndDate DATE
		)

	CREATE TABLE #TblEPAs (
		EPAId INT
		,EndDate DATE
		)

	CREATE TABLE #Employees (
		EmployeeId INT
		,Code NVARCHAR(10)
		,GroupName NVARCHAR(150)
		)

	CREATE TABLE #Periods (
		PeriodId INT
		,[Name] NVARCHAR(200)
		,StartDate DATETIME
		)

	CREATE TABLE #TblPeriods (
		PeriodId INT
		,[Name] NVARCHAR(200)
		)

	CREATE TABLE #DisciplineScores (
		EPAId INT
		,SystemDisciplineScore FLOAT
		,ApproverSubtractScore FLOAT
		,OtherDisciplines FLOAT
		)

	SET @ToDay = CAST(GETDATE() AS DATE)
	SET @SortCode = 'code'
	SET @SortSection = 'section'

	IF ISNULL(@PhaseIds, '') <> ''
	BEGIN
		INSERT INTO @Phases (Id)
		SELECT DISTINCT val
		FROM fnc_strSplit(@PhaseIds, ',')
	END

	INSERT INTO #Periods
	SELECT [PeriodId]
		,[Name]
		,[StartDate]
	FROM [dbo].[Period] WITH (NOLOCK)
	WHERE [IsActive] = 1
		AND TargetTypeId = 2 --Monthly
	ORDER BY [StartDate] DESC

	INSERT INTO #TblPeriods
	SELECT PeriodId
		,[Name]
	FROM #Periods
	WHERE @PeriodId IS NULL
		OR PeriodId = @PeriodId

	INSERT INTO #Employees
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
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			AND pc.ProcessId = 2
			AND CAST(pc.StartDate AS DATE) <= @ToDay
			INNER JOIN #TblPeriods AS period ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
			LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
		WHERE epa.[Status] <> 2 -- rejected
		
		UNION ALL
		SELECT DISTINCT emp.EmployeeId
			,emp.Code
			,emp.NickName
			,emp.IsLocal
			,dept.DepartmentId
			,gr.GroupId
			,gr.[Name] AS GroupName
		FROM dbo.EPASummaryImport AS epa WITH (NOLOCK)
			INNER JOIN #TblPeriods AS period ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.Code = emp.Code
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
			LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
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
					INNER JOIN dbo.LookupData AS lo WITH (NOLOCK) ON ro.RoleId = lo.Value
					AND lo.LookupId = 2 -- role
				WHERE ro.EmployeeId = @EmployeeId
					AND ro.RoleId IN (8) -- Section Admin can view all member
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empRole WITH (NOLOCK)
					INNER JOIN dbo.Employee AS emp1 ON empRole.EmployeeId = emp1.EmployeeId
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

	INSERT INTO #EPAs
	SELECT DISTINCT tbl.EPAId
		,tbl.Code
		,tbl.GroupName
		,tbl.EndDate
	FROM (
		SELECT DISTINCT epa.EPAId
			,emp.Code
			,emp.GroupName
			,MAX(CAST(pc.EndDate AS DATE)) AS EndDate
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS pcSelf WITH (NOLOCK) ON epa.EPAId = pcSelf.EPAId
			AND pcSelf.ProcessId = 2
			AND CAST(pcSelf.StartDate AS DATE) <= @ToDay
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
			INNER JOIN #TblPeriods AS period1 ON epa.PeriodId = period1.PeriodId
			INNER JOIN #Employees AS emp ON epa.EmployeeId = emp.EmployeeId
		GROUP BY epa.EPAId
			,emp.Code
			,emp.GroupName
			,tt.TargetTypeId
			,period.EndDate
		) AS tbl
	ORDER BY tbl.GroupName
		,tbl.Code

	IF @PageIndex IS NOT NULL
		AND @ItemsPerPage IS NOT NULL
	BEGIN
		INSERT INTO #TblEPAs
		SELECT EPAId
			,EndDate
		FROM #EPAs
		ORDER BY CASE 
				WHEN ISNULL(@SortColumnName, '') <> ''
					AND ISNULL(@SortDirection, '') <> ''
					AND @SortDirection = 'desc' THEN CASE LOWER(@SortColumnName)
						WHEN @SortCode THEN Code
						WHEN @SortSection THEN GroupName
						END
				END DESC
			,CASE 
				WHEN ISNULL(@SortColumnName, '') <> ''
					AND ISNULL(@SortDirection, '') <> ''
					AND @SortDirection = 'asc' THEN CASE LOWER(@SortColumnName)
						WHEN @SortCode THEN Code
						WHEN @SortSection THEN GroupName
						END
				END OFFSET(@PageIndex * @ItemsPerPage) ROWS

		FETCH NEXT @ItemsPerPage ROWS ONLY;
	END
	ELSE
	BEGIN
		INSERT INTO #TblEPAs
		SELECT EPAId
			,EndDate
		FROM #EPAs
		ORDER BY CASE 
				WHEN ISNULL(@SortColumnName, '') <> ''
					AND ISNULL(@SortDirection, '') <> ''
					AND @SortDirection = 'desc' THEN CASE LOWER(@SortColumnName)
						WHEN @SortCode THEN Code
						WHEN @SortSection THEN GroupName
						END
				END DESC
			,CASE 
				WHEN ISNULL(@SortColumnName, '') <> ''
					AND ISNULL(@SortDirection, '') <> ''
					AND @SortDirection = 'asc' THEN CASE LOWER(@SortColumnName)
						WHEN @SortCode THEN Code
						WHEN @SortSection THEN GroupName
						END
				END
	END

	SELECT @Ids = COALESCE(@Ids + ',', '') + CAST(EPAId AS NVARCHAR)
	FROM #TblEPAs

	INSERT INTO #DisciplineScores
	EXECUTE dbo.EmployeeDisciplineSummary_GetByEPA @Ids

	-- Phase selected
	SELECT CAST(lkd.Value AS TINYINT) AS PhaseId
		,lkd.[Name] AS PhaseName
	FROM dbo.[Lookup] AS lk WITH (NOLOCK)
		INNER JOIN dbo.LookupData AS lkd WITH (NOLOCK) ON lk.Id = lkd.LookupId
		AND lk.Id = 5 -- evaluation phase
	WHERE @PhaseIds IS NULL
		OR CAST(lkd.Value AS TINYINT) IN (
			SELECT Id
			FROM @Phases
			)

	-- General Info
	SELECT tblData.*
	FROM (
		SELECT DISTINCT epa.EPAId
			,epa.FinalScore
			,epa.Grade
			,epa.HasMonthlyEPA
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
			,emp.EmployeeId
			,emp.Code
			,emp.NickName
			,emp.FullName
			,emp.Email
			,emp.JoinedDate
			,lkd.[Name] AS Category
			,dept.DepartmentId
			,dept.[Name] AS DepartmentName
			,gr.GroupId
			,gr.Code AS GroupCode
			,gr.[Name] AS GroupName
			,job.Id AS JobTitleId
			,job.[Name] AS JobTitleName
			,epa.[Status] AS StatusId
			,period.PeriodId
			,period.[Name]
			,(CASE 
				WHEN CAST(pc.EndDate AS DATE) >= CAST(period.EndDate AS DATE) THEN (CASE 
						WHEN CAST(pc.EndDate AS DATE) < @ToDay THEN 1
						ELSE 0 END
						)
				ELSE (CASE 
						WHEN CAST(period.EndDate AS DATE) <= @ToDay THEN 1
						ELSE 0 END
						) END
				) AS IsEndPeriod
			,ttype.TargetTypeId
			,ttype.[Name] AS TargetTypeName
		FROM #TblEPAs AS tbl
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.EPAId = epa.EPAId
			INNER JOIN (
			SELECT pc1.EPAId
				,MIN(pc1.StartDate) AS StartDate
				,MAX(pc1.EndDate) AS EndDate
			FROM dbo.EPAProcess AS pc1 WITH (NOLOCK)
				INNER JOIN #TblEPAs AS tbl1 ON pc1.EPAId = tbl1.EPAId
			GROUP BY pc1.EPAId
			) AS pc ON epa.EPAId = pc.EPAId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
			LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.[JobTitle] = job.[Id]
			LEFT JOIN dbo.LookupData AS lkd WITH (NOLOCK) ON emp.IsLocal = CAST(lkd.Value AS BIT)
			AND lkd.LookupId = 11 -- employee category
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
			INNER JOIN dbo.TargetType AS ttype WITH (NOLOCK) ON period.TargetTypeId = ttype.TargetTypeId
			LEFT JOIN #DisciplineScores AS ds ON epa.EPAId = ds.EPAId
		) AS tblData
	ORDER BY CASE 
			WHEN ISNULL(@SortColumnName, '') <> ''
				AND ISNULL(@SortDirection, '') <> ''
				AND @SortDirection = 'desc' THEN CASE LOWER(@SortColumnName)
					WHEN @SortCode THEN tblData.Code
					WHEN @SortSection THEN tblData.GroupName
					END
			END DESC
		,CASE 
			WHEN ISNULL(@SortColumnName, '') <> ''
				AND ISNULL(@SortDirection, '') <> ''
				AND @SortDirection = 'asc' THEN CASE LOWER(@SortColumnName)
					WHEN @SortCode THEN tblData.Code
					WHEN @SortSection THEN tblData.GroupName
					END
			END

	-- Process
	SELECT DISTINCT epa.EPAId
		,pc.ProcessId
		,pc.TotalScore
		,pc.IsEndProcess
		,(CASE 
			WHEN ISNULL(epa.EPAId, '') <> '' THEN CASE 
					WHEN pc.IsEndProcess = 0 THEN CASE 
							WHEN CAST(pc.StartDate AS DATE) > CAST(GETDATE() AS DATE) THEN 1 -- Open
							ELSE 2 -- In Progress
							END
					ELSE 3 -- Closed
					END
			ELSE 0 END
			) AS StatusEndProcess
		,emp.EmployeeId AS EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM #TblEPAs AS tbl
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.EPAId = epa.EPAId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId > 1 -- ignore HR phase
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pc.EvaluatorId = emp.EmployeeId
	ORDER BY epa.EPAId
		,pc.ProcessId

	SELECT @TotalItem = COUNT(*)
	FROM #EPAs

	DROP TABLE #EPAs

	DROP TABLE #TblEPAs

	DROP TABLE #Employees

	DROP TABLE #Periods

	DROP TABLE #TblPeriods

	DROP TABLE #DisciplineScores
END