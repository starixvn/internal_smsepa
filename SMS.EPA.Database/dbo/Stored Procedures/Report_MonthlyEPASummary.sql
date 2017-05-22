
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

	DECLARE @Ids NVARCHAR(MAX)
		,@ToDay DATE = GETDATE()
		,@SortCode NVARCHAR(10) = 'code'
		,@SortSection NVARCHAR(10) = 'section'
	-- target type
	DECLARE @MonthlyId INT = 2

	CREATE TABLE #tblEPA (
		EPAId INT
		,EndDate DATE
		)

	CREATE TABLE #tblEmployee (
		EmployeeId INT
		,Code NVARCHAR(20)
		,GroupName NVARCHAR(150)
		)

	CREATE TABLE #tblPeriod (
		PeriodId INT
		,[Name] NVARCHAR(200)
		,StartDate DATE
		)

	CREATE TABLE #tblPeriodTemp (
		PeriodId INT
		,[Name] NVARCHAR(200)
		)

	CREATE TABLE #tblDisciplineScore (
		EPAId INT
		,SystemDisciplineScore FLOAT
		,ApproverSubtractScore FLOAT
		,OtherDisciplines FLOAT
		)

	CREATE TABLE #tblEPAStatus (
		EPAId INT
		,StatusId TINYINT
		)

	INSERT INTO #tblPeriod (
		PeriodId
		,[Name]
		,StartDate
		)
	SELECT PeriodId
		,[Name]
		,StartDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE IsActive = 1
		AND TargetTypeId = @MonthlyId
	ORDER BY StartDate DESC

	INSERT INTO #tblPeriodTemp (
		PeriodId
		,[Name]
		)
	SELECT PeriodId
		,[Name]
	FROM #tblPeriod
	WHERE @PeriodIds IS NULL
		OR PeriodId IN (
			SELECT DISTINCT val
			FROM fnc_strSplit(@PeriodIds, ',')
			)

	INSERT INTO #tblEmployee (
		EmployeeId
		,Code
		,GroupName
		)
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
			INNER JOIN #tblPeriodTemp AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
			LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
			INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa.PeriodId = period1.PeriodId
			INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period1.TargetTypeId = tt.TargetTypeId
		WHERE ISNULL(epa.[Status], 0) IN (0, 1)
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

	INSERT INTO #tblEPA (
		EPAId
		,EndDate
		)
	SELECT DISTINCT epa.EPAId
		,MAX(pc.EndDate) AS EndDate
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN #tblEmployee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN #tblPeriodTemp AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa.PeriodId = period1.PeriodId
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period1.TargetTypeId = tt.TargetTypeId
	WHERE ISNULL(epa.[Status], 0) IN (0, 1)
	GROUP BY epa.EPAId
		,period1.EndDate
		,tt.TargetTypeId

	SELECT @Ids = COALESCE(@Ids + ',', '') + CAST(EPAId AS NVARCHAR)
	FROM #tblEPA WITH (NOLOCK)

	INSERT INTO #tblDisciplineScore (
		EPAId
		,SystemDisciplineScore
		,ApproverSubtractScore
		,OtherDisciplines
		)
	EXECUTE dbo.EmployeeDisciplineSummary_GetByEPA @Ids

	SELECT period.PeriodId
		,period.[Name]
	FROM #tblPeriodTemp AS tbl WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON tbl.PeriodId = period.PeriodId
	ORDER BY period.StartDate DESC

	INSERT INTO #tblEPAStatus (
		EPAId
		,StatusId
		)
	SELECT epa.EPAId
		,(CASE 
			WHEN pc.EndDate >= CAST(period.EndDate AS DATE) THEN (CASE 
					WHEN pc.EndDate < @ToDay THEN 1
					ELSE 0 END
					)
			ELSE (CASE 
					WHEN CAST(period.EndDate AS DATE) <= @ToDay THEN 1
					ELSE 0 END
					) END
			)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN #tblEPA AS tbl WITH (NOLOCK) ON epa.EPAId = tbl.EPAId
		INNER JOIN (
		SELECT pc.EPAId
			,CAST(MAX(pc.EndDate) AS DATE) AS EndDate
		FROM dbo.EPAProcess AS pc WITH (NOLOCK)
			INNER JOIN #tblEPA AS tbl1 WITH (NOLOCK) ON pc.EPAId = tbl1.EPAId
		GROUP BY pc.EPAId
		) AS pc ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
		AND tt.TargetTypeId = @MonthlyId

	-- get data
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
		INNER JOIN #tblEmployee AS tblEmp WITH (NOLOCK) ON emp.EmployeeId = tblEmp.EmployeeId
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
				WHEN tblstatus.StatusId = 1 THEN epa.DisciplineScore
				ELSE (CASE 
						WHEN ds.ApproverSubtractScore IS NULL THEN 0
						ELSE ds.ApproverSubtractScore END
						) END
				) AS DisciplineScore
			,srt.Attendance AS MaxDisciplineScore
			,epa.FinalScore
			,epa.Grade
			,tblstatus.StatusId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN #tblEPA AS epa1 ON epa.EPAId = epa1.EPAId
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			INNER JOIN #tblEmployee AS tblEmp WITH (NOLOCK) ON emp.EmployeeId = tblEmp.EmployeeId
			INNER JOIN #tblPeriod AS period1 WITH (NOLOCK) ON epa.PeriodId = period1.PeriodId
			INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
			INNER JOIN #tblEPAStatus AS tblstatus WITH (NOLOCK) ON epa.EPAId = tblstatus.EPAId
			LEFT JOIN #tblDisciplineScore AS ds WITH (NOLOCK) ON epa.EPAId = ds.EPAId
		) AS tbl

	DROP TABLE #tblEPA

	DROP TABLE #tblEmployee

	DROP TABLE #tblPeriod

	DROP TABLE #tblPeriodTemp

	DROP TABLE #tblDisciplineScore

	DROP TABLE #tblEPAStatus
END