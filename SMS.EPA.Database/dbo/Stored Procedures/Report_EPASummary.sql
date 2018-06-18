

CREATE PROCEDURE [dbo].[Report_EPASummary] @ActorId INT
	,@PeriodIds NVARCHAR(MAX) = NULL
	,@DepartmentId INT = NULL
	,@IsLocal BIT = NULL
	,@EmployeeId INT = NULL
	,@SortColumnName NVARCHAR(100) = NULL
	,@SortDirection NVARCHAR(10) = NULL
AS
/*
Created: 20150924@Tim.Trinh
Task: EPA Summary Report.
DB: SMSEPA
Original: @SortDirection = 'asc'/'desc'

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Ids NVARCHAR(MAX)
		,@Today DATE = GETDATE()
		,@SortCode NVARCHAR(10) = 'code'
		,@SortSection NVARCHAR(10) = 'section'
	-- target type
	DECLARE @SemiTargetId INT = 3

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
		,DisciplineScore FLOAT
		,OtherDisciplines FLOAT
		)

	CREATE TABLE #tblEPAStatus (
		EPAId INT
		,StatusId TINYINT
		)
	
	CREATE TABLE #tblLastedEPA (
		EmployeeId INT
		,TargetTypeId TINYINT
		,StartDate DATE
		)

	INSERT INTO #tblPeriod (
		PeriodId
		,[Name]
		,StartDate
		)
	EXECUTE dbo.EpaPeriod_GetAllPeriodIncludeImport

	INSERT INTO #tblPeriodTemp (
		PeriodId
		,[Name]
		)
	SELECT PeriodId
		,[Name]
	FROM #tblPeriod WITH (NOLOCK)
	WHERE @PeriodIds IS NULL
		OR PeriodId IN (
			SELECT DISTINCT val
			FROM fnc_strSplit(@PeriodIds, ',')
			)
	
	INSERT INTO #tblLastedEPA (
		EmployeeId
		,TargetTypeId
		,StartDate
		)
	SELECT epa.EmployeeId
		,period.TargetTypeId
		,MAX(process.StartDate)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS process WITH (NOLOCK) ON epa.EPAId = process.EPAId
		AND process.ProcessId = 1 -- HR
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN #tblPeriodTemp AS tblperiod WITH (NOLOCK) ON period.PeriodId = tblperiod.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND emp.ResignedDate IS NULL
	GROUP BY epa.EmployeeId
		,period.TargetTypeId

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
			,period1.TargetTypeId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS pcSelf WITH (NOLOCK) ON epa.EPAId = pcSelf.EPAId
			AND pcSelf.ProcessId = 2
			AND pcSelf.StartDate <= @Today
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			INNER JOIN #tblPeriodTemp AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			AND emp.ResignedDate IS NULL
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
			LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
			INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa.PeriodId = period1.PeriodId
			INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period1.TargetTypeId = tt.TargetTypeId
		WHERE ISNULL(epa.[Status], 0) IN (0, 1)
		
		UNION ALL
		SELECT DISTINCT emp.EmployeeId
			,emp.Code
			,emp.NickName
			,emp.IsLocal
			,dept.DepartmentId
			,gr.GroupId
			,gr.[Name] AS GroupName
			,@SemiTargetId AS TargetTypeId
		FROM dbo.EPASummaryImport AS epa WITH (NOLOCK)
			INNER JOIN #tblPeriodTemp AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.Code = emp.Code
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			AND emp.ResignedDate IS NULL
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
		AND ISNULL(@IsLocal, tblData.IsLocal) = tblData.IsLocal
		AND ISNULL(@EmployeeId, tblData.EmployeeId) = tblData.EmployeeId
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS erole WITH (NOLOCK)
				WHERE erole.EmployeeId = @ActorId
					AND erole.RoleId IN (1, 2) -- SA, HR
				)
			OR (
				EXISTS (
					SELECT 1
					FROM dbo.EmployeeRole AS erole WITH (NOLOCK)
						INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON erole.EmployeeId = emp.EmployeeId
						AND erole.RoleId IN (3, 4, 5) -- MGT, Reviewer, Evaluator
						AND emp.EmployeeId = @ActorId
					)
				AND EXISTS (
					SELECT 1
					FROM dbo.EPA AS epa WITH (NOLOCK)
						INNER JOIN dbo.EPAProcess AS process WITH (NOLOCK) ON epa.EPAId = process.EPAId
						AND process.ProcessId > 2 -- self
						AND process.EvaluatorId = @ActorId
						AND epa.EmployeeId = tblData.EmployeeId
						INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
						AND period.TargetTypeId = tblData.TargetTypeId
					WHERE epa.EPAId IN (
							SELECT DISTINCT epa1.EPAId
							FROM dbo.EPA AS epa1 WITH (NOLOCK)
								INNER JOIN dbo.EPAProcess AS process1 WITH (NOLOCK) ON epa1.EPAId = process1.EPAId
								AND process1.ProcessId = 1 -- HR
								INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa1.PeriodId = period1.PeriodId
								AND period1.TargetTypeId = tblData.TargetTypeId
								INNER JOIN #tblLastedEPA AS lastedepa WITH (NOLOCK) ON epa1.EmployeeId = lastedepa.EmployeeId
								AND period1.TargetTypeId = lastedepa.TargetTypeId
								AND process1.StartDate = lastedepa.StartDate
							)
					)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS erole WITH (NOLOCK)
				WHERE erole.RoleId = 6 -- Employee
					AND erole.EmployeeId = @ActorId
					AND tblData.EmployeeId = @ActorId
				)
			)
	ORDER BY tblData.GroupName
		,tblData.Code

	INSERT INTO #tblEPA (
		EPAId
		,EndDate
		)
	SELECT DISTINCT epa.EPAId
		,(CASE 
			WHEN tt.TargetTypeId = @SemiTargetId THEN CASE 
					WHEN MAX(pc.EndDate) > period1.EndDate THEN MAX(pc.EndDate)
					ELSE period1.EndDate
					END
			ELSE MAX(pc.EndDate) END
			) AS EndDate
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
		,DisciplineScore
		,OtherDisciplines
		)
	EXECUTE dbo.DisciplineSummary_GetByEPA @Ids

	INSERT INTO #tblEPAStatus (
		EPAId
		,StatusId
		)
	SELECT epa.EPAId
		,(CASE 
			WHEN tt.TargetTypeId = @SemiTargetId THEN (CASE 
					WHEN pc.EndDate >= period.EndDate THEN (CASE 
							WHEN pc.EndDate < @Today THEN 1
							ELSE 0 END
							)
					ELSE (CASE 
							WHEN period.EndDate <= @Today THEN 1
							ELSE 0 END
							) END
					)
			ELSE (CASE 
					WHEN pc.EndDate < @Today THEN 1
					ELSE 0 END
					) END
			)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN #tblEPA AS tbl WITH (NOLOCK) ON epa.EPAId = tbl.EPAId
		INNER JOIN (
		SELECT pc.EPAId
			,MAX(pc.EndDate) AS EndDate
		FROM dbo.EPAProcess AS pc WITH (NOLOCK)
			INNER JOIN #tblEPA AS tbl1 WITH (NOLOCK) ON pc.EPAId = tbl1.EPAId
		GROUP BY pc.EPAId
		) AS pc ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId

	-- get data
	SELECT PeriodId
		,[Name]
	FROM #tblPeriodTemp WITH (NOLOCK)

	SELECT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.JoinedDate
		,(CASE 
			WHEN emp.IsLocal = 1 THEN 'Local'
			ELSE 'Expat' END
			) AS Category
		,gr.GroupId
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,emp.JobTitle AS JobTitleId
		,job.[Name] AS JobTitleName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblEmployee AS tblemp WITH (NOLOCK) ON emp.EmployeeId = tblemp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
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
			,period.TargetTypeId
			,epa.EPAId
			,epa.HasMonthlyEPA
			,(CASE srt.ScoreRatingTypeId
				WHEN 1 THEN 1
				ELSE 0 END
				) AS HasDisciplineScore
			,(CASE 
				WHEN tblstatus.StatusId = 1 THEN epa.DisciplineScore
				ELSE (CASE 
						WHEN ds.DisciplineScore IS NULL
							OR ds.DisciplineScore < 0 THEN 0
						ELSE ds.DisciplineScore END
						) END
				) AS DisciplineScore
			,srt.Attendance AS MaxDisciplineScore
			,epa.FinalScore
			,epa.Grade
			,tblstatus.StatusId
			,srt.ScoreRatingTypeId
			,srt.[Name]
			,srt.SelfEvaluated
			,srt.Evaluator
			,srt.Reviewer
			,srt.Management
			,srt.Attendance
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN #tblEPA AS epa1 WITH (NOLOCK) ON epa.EPAId = epa1.EPAId
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			INNER JOIN #tblEmployee AS tblemp WITH (NOLOCK) ON emp.EmployeeId = tblemp.EmployeeId
			INNER JOIN #tblPeriodTemp AS period1 WITH (NOLOCK) ON epa.PeriodId = period1.PeriodId
			INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
			INNER JOIN #tblEPAStatus AS tblstatus WITH (NOLOCK) ON epa.EPAId = tblstatus.EPAId
			LEFT JOIN #tblDisciplineScore AS ds WITH (NOLOCK) ON epa.EPAId = ds.EPAId
		
		UNION ALL
		SELECT DISTINCT emp.EmployeeId
			,period.PeriodId
			,NULL AS TargetTypeId
			,- epa.Id AS EPAId
			,0 AS HasMonthlyEPA
			,0 AS HasDisciplineScore
			,0 AS DisciplineScore
			,0 AS MaxDisciplineScore
			,epa.FinalScore
			,NULL AS Grade
			,1 AS StatusId
			,NULL AS ScoreRatingTypeId
			,NULL AS [Name]
			,NULL AS SelfEvaluated
			,NULL AS Evaluator
			,NULL AS Reviewer
			,NULL AS Management
			,NULL AS Attendance
		FROM dbo.EPASummaryImport AS epa WITH (NOLOCK)
			INNER JOIN #tblPeriodTemp AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.Code = emp.Code
			INNER JOIN #tblEmployee AS tblemp WITH (NOLOCK) ON emp.EmployeeId = tblemp.EmployeeId
		) AS tbl

	SELECT pc.EPAId
		,epa.PeriodId
		,epa.EmployeeId
		,pc.EvaluatorId
		,pc.ProcessId
		,(CASE 
			WHEN pc.StartDate > @Today THEN 1
			WHEN @Today BETWEEN pc.StartDate
					AND pc.EndDate THEN 2
			WHEN @Today > pc.EndDate THEN 3 END
			) AS StatusId -- 1:Open; 2:In Progress; 3:Closed
		,pc.TotalScore
		,pc.AdjustScore
	FROM dbo.EPAProcess AS pc WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON pc.EPAId = epa.EPAId
		INNER JOIN #tblEPA AS tbl WITH (NOLOCK) ON epa.EPAId = tbl.EPAId
	WHERE pc.ProcessId > 1

	DROP TABLE #tblEPA

	DROP TABLE #tblEmployee

	DROP TABLE #tblDisciplineScore

	DROP TABLE #tblPeriod

	DROP TABLE #tblPeriodTemp

	DROP TABLE #tblEPAStatus
	
	DROP TABLE #tblLastedEPA
END