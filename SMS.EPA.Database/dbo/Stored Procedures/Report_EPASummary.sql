
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
		,@ToDay DATE = GETDATE()
		,@SortCode NVARCHAR(10) = 'code'
		,@SortSection NVARCHAR(10) = 'section'
	-- target type
	DECLARE @SemiId INT = 3

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
		
		UNION ALL
		SELECT DISTINCT emp.EmployeeId
			,emp.Code
			,emp.NickName
			,emp.IsLocal
			,dept.DepartmentId
			,gr.GroupId
			,gr.[Name] AS GroupName
		FROM dbo.EPASummaryImport AS epa WITH (NOLOCK)
			INNER JOIN #tblPeriodTemp AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
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
			@EmployeeId IS NULL
			OR tblData.EmployeeId = @EmployeeId
			)
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
	ORDER BY tblData.GroupName
		,tblData.Code

	INSERT INTO #tblEPA (
		EPAId
		,EndDate
		)
	SELECT DISTINCT epa.EPAId
		,(CASE 
			WHEN tt.TargetTypeId = @SemiId THEN CASE 
					WHEN MAX(CAST(pc.EndDate AS DATE)) > CAST(period1.EndDate AS DATE) THEN MAX(pc.EndDate)
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
		,ApproverSubtractScore
		,OtherDisciplines
		)
	EXECUTE dbo.EmployeeDisciplineSummary_GetByEPA @Ids

	INSERT INTO #tblEPAStatus (
		EPAId
		,StatusId
		)
	SELECT epa.EPAId
		,(CASE 
			WHEN tt.TargetTypeId = @SemiId THEN (CASE 
					WHEN pc.EndDate >= CAST(period.EndDate AS DATE) THEN (CASE 
							WHEN pc.EndDate < @ToDay THEN 1
							ELSE 0 END
							)
					ELSE (CASE 
							WHEN CAST(period.EndDate AS DATE) <= @ToDay THEN 1
							ELSE 0 END
							) END
					)
			ELSE (CASE 
					WHEN pc.EndDate < @ToDay THEN 1
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

	-- get data
	SELECT PeriodId
		,[Name]
	FROM #tblPeriodTemp WITH (NOLOCK)

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
		INNER JOIN #tblEmployee AS tblemp WITH (NOLOCK) ON emp.EmployeeId = tblemp.EmployeeId
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
			INNER JOIN #tblEPA AS epa1 WITH (NOLOCK) ON epa.EPAId = epa1.EPAId
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			INNER JOIN #tblEmployee AS tblemp WITH (NOLOCK) ON emp.EmployeeId = tblemp.EmployeeId
			INNER JOIN #tblPeriod AS period1 WITH (NOLOCK) ON epa.PeriodId = period1.PeriodId
			INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
			INNER JOIN #tblEPAStatus AS tblstatus WITH (NOLOCK) ON epa.EPAId = tblstatus.EPAId
			LEFT JOIN #tblDisciplineScore AS ds WITH (NOLOCK) ON epa.EPAId = ds.EPAId
		
		UNION ALL
		SELECT DISTINCT emp.EmployeeId
			,period.PeriodId
			,0 AS HasDisciplineScore
			,0 AS DisciplineScore
			,0 AS MaxDisciplineScore
			,epa.FinalScore
			,NULL AS Grade
			,1 AS StatusId
		FROM dbo.EPASummaryImport AS epa WITH (NOLOCK)
			INNER JOIN #tblPeriod AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.Code = emp.Code
			INNER JOIN #tblEmployee AS tblemp WITH (NOLOCK) ON emp.EmployeeId = tblemp.EmployeeId
		) AS tbl

	SELECT pc.EPAId
		,epa.PeriodId
		,epa.EmployeeId
		,pc.EvaluatorId
		,pc.ProcessId
		,(CASE 
			WHEN CAST(pc.StartDate AS DATE) > @Today THEN 1
			WHEN @Today BETWEEN CAST(pc.StartDate AS DATE)
					AND CAST(pc.EndDate AS DATE) THEN 2
			WHEN @Today > CAST(pc.EndDate AS DATE) THEN 3 END
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
END