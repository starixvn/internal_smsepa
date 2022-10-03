
CREATE PROCEDURE [dbo].[Report_EPADetail] @ActorId INT
	,@PeriodId INT = NULL
	,@PhaseIds NVARCHAR(10) = NULL
	,@TeamIds VARCHAR(MAX) = NULL
	,@IsLocal BIT = NULL
	,@EmployeeId INT = NULL
	,@SortColumnName NVARCHAR(100) = NULL
	,@SortDirection NVARCHAR(10) = NULL
AS
/*
Created: 20151001@Tim.Trinh
Task: EPA Detail Report.
DB: SMSEPA
Original: @SortDirection = 'asc'/'desc'

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Phases IntId
		,@Today DATE = GETDATE()
		,@Ids NVARCHAR(MAX)
		,@SortCode NVARCHAR(10) = 'code'
		,@SortTeam NVARCHAR(10) = 'team'
	-- target type
	DECLARE @SemiTargetId INT = 3

	CREATE TABLE #tmpTeam (TeamId INT)

	CREATE TABLE #tblEPAs (
		EPAId INT
		,Code NVARCHAR(20)
		,GroupName NVARCHAR(150)
		,EndDate DATE
		)

	CREATE TABLE #tblEmployees (
		EmployeeId INT
		,Code NVARCHAR(20)
		,GroupName NVARCHAR(150)
		)

	CREATE TABLE #Periods (
		PeriodId INT
		,[Name] NVARCHAR(200)
		,StartDate DATE
		)

	CREATE TABLE #tblPeriods (
		PeriodId INT
		,[Name] NVARCHAR(200)
		)

	CREATE TABLE #tblDisciplineScores (
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

	IF @TeamIds IS NOT NULL
	BEGIN
		INSERT INTO #tmpTeam (TeamId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
	END
	
	IF ISNULL(@PhaseIds, '') <> ''
	BEGIN
		INSERT INTO @Phases (Id)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@PhaseIds, ',')
	END

	INSERT INTO #Periods (
		PeriodId
		,[Name]
		,StartDate
		)
	EXECUTE dbo.EpaPeriod_GetAllPeriodIncludeImport

	INSERT INTO #tblPeriods (
		PeriodId
		,[Name]
		)
	SELECT PeriodId
		,[Name]
	FROM #Periods WITH (NOLOCK)
	WHERE @PeriodId IS NULL
		OR PeriodId = @PeriodId

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
		INNER JOIN #tblPeriods AS tblperiod WITH (NOLOCK) ON period.PeriodId = tblperiod.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND emp.ResignedDate IS NULL
	GROUP BY epa.EmployeeId
		,period.TargetTypeId

	INSERT INTO #tblEmployees (
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
			,gr.GroupId
			,gr.[Name] AS GroupName
			,period1.TargetTypeId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			AND pc.ProcessId = 2
			AND pc.StartDate <= @Today
			INNER JOIN #tblPeriods AS period ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON period.PeriodId = period1.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			AND emp.ResignedDate IS NULL
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		WHERE ISNULL(epa.[Status], 0) IN (0, 1)
		
		UNION ALL
		SELECT DISTINCT emp.EmployeeId
			,emp.Code
			,emp.NickName
			,emp.IsLocal
			,gr.GroupId
			,gr.[Name] AS GroupName
			,@SemiTargetId AS TargetTypeId
		FROM dbo.EPASummaryImport AS epa WITH (NOLOCK)
			INNER JOIN #tblPeriods AS period ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.Code = emp.Code
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			AND emp.ResignedDate IS NULL
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		) AS tblData
	WHERE (
			@TeamIds IS NULL
			OR EXISTS (
				SELECT 1
				FROM #tmpTeam AS tmp WITH (NOLOCK)
				WHERE tblData.GroupId = tmp.TeamId
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
				WHERE RoleId = 6 -- Employee
					AND erole.EmployeeId = @ActorId
					AND tblData.EmployeeId = @ActorId
				)
			)
	ORDER BY tblData.GroupName ASC
		,tblData.Code ASC

	INSERT INTO #tblEPAs (
		EPAId
		,Code
		,GroupName
		,EndDate
		)
	SELECT DISTINCT tbl.EPAId
		,tbl.Code
		,tbl.GroupName
		,tbl.EndDate
	FROM (
		SELECT DISTINCT epa.EPAId
			,emp.Code
			,emp.GroupName
			,(CASE 
				WHEN tt.TargetTypeId = @SemiTargetId THEN CASE 
						WHEN MAX(pc.EndDate) > period.EndDate THEN MAX(pc.EndDate)
						ELSE period.EndDate
						END
				ELSE MAX(pc.EndDate) END
				) AS EndDate
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS pcSelf WITH (NOLOCK) ON epa.EPAId = pcSelf.EPAId
			AND pcSelf.ProcessId = 2
			AND CAST(pcSelf.StartDate AS DATE) <= @Today
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
			INNER JOIN #tblPeriods AS period1 WITH (NOLOCK) ON epa.PeriodId = period1.PeriodId
			INNER JOIN #tblEmployees AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		WHERE ISNULL(epa.[Status], 0) IN (0, 1)
		GROUP BY epa.EPAId
			,emp.Code
			,emp.GroupName
			,tt.TargetTypeId
			,period.EndDate
		
		UNION ALL
		SELECT DISTINCT - epa.Id AS EPAId
			,emp.Code
			,emp.GroupName
			,NULL
		FROM dbo.EPASummaryImport AS epa WITH (NOLOCK)
			INNER JOIN #tblPeriods AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN #tblEmployees AS emp WITH (NOLOCK) ON epa.Code = emp.Code
		) AS tbl
	ORDER BY tbl.GroupName ASC
		,tbl.Code ASC

	SELECT @Ids = COALESCE(@Ids + ',', '') + CAST(EPAId AS NVARCHAR(20))
	FROM #tblEPAs WITH (NOLOCK)

	INSERT INTO #tblDisciplineScores (
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
		INNER JOIN #tblEPAs AS tbl WITH (NOLOCK) ON epa.EPAId = tbl.EPAId
		INNER JOIN (
		SELECT pc.EPAId
			,MAX(pc.EndDate) AS EndDate
		FROM dbo.EPAProcess AS pc WITH (NOLOCK)
			INNER JOIN #tblEPAs AS tbl1 WITH (NOLOCK) ON pc.EPAId = tbl1.EPAId
		GROUP BY pc.EPAId
		) AS pc ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId

	-- Phase selected
	SELECT ResourceId AS PhaseId
		,Value AS PhaseName
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'EvaluationPhase'
		AND (
			@PhaseIds IS NULL
			OR ResourceId IN (
				SELECT Id
				FROM @Phases
				)
			)

	-- General Info
	SELECT tblData.*
	FROM (
		SELECT epa.EPAId
			,epa.FinalScore
			,epa.Grade
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
			,tblstatus.StatusId
			,emp.EmployeeId
			,emp.Code
			,emp.NickName
			,emp.FullName
			,emp.Email
			,emp.JoinedDate
			,(CASE 
				WHEN emp.IsLocal = 1 THEN 'Local'
				ELSE 'Expat' END
				) AS Category
			,dept.DepartmentId
			,dept.[Name] AS DepartmentName
			,gr.GroupId
			,gr.Code AS GroupCode
			,gr.[Name] AS GroupName
			,job.Id AS JobTitleId
			,job.[Name] AS JobTitleName
			,period.PeriodId
			,period.[Name]
			,ttype.TargetTypeId
			,ttype.[Name] AS TargetTypeName
			,ttem.Name AS TargetTemplate
			,srt.ScoreRatingTypeId
			,srt.SelfEvaluated
			,srt.Evaluator
			,srt.Reviewer
			,srt.Management
			,srt.Attendance
		FROM #tblEPAs AS tbl WITH (NOLOCK)
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.EPAId = epa.EPAId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
			INNER JOIN dbo.TargetType AS ttype WITH (NOLOCK) ON period.TargetTypeId = ttype.TargetTypeId
			INNER JOIN dbo.PeriodTargetSetting AS pertar WITH (NOLOCK) ON epa.PeriodId = pertar.PeriodId and epa.EmployeeId = pertar.EmployeeId
			INNER JOIN dbo.TargetTemplate AS ttem WITH (NOLOCK) ON ttem.TargetTemplateId = pertar.TargetTemplateId
			INNER JOIN #tblEPAStatus AS tblstatus WITH (NOLOCK) ON epa.EPAId = tblstatus.EPAId
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
			LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.[JobTitle] = job.[Id]
			LEFT JOIN #tblDisciplineScores AS ds WITH (NOLOCK) ON epa.EPAId = ds.EPAId
		
		UNION ALL
		SELECT - epa.Id AS EPAId
			,epa.FinalScore
			,NULL
			,0 AS HasMonthlyEPA
			,0 AS HasDisciplineScore
			,0 AS DisciplineScore
			,0 AS MaxDisciplineScore
			,1 AS StatusId
			,emp.EmployeeId
			,emp.Code
			,emp.NickName
			,emp.FullName
			,emp.Email
			,emp.JoinedDate
			,(CASE 
				WHEN emp.IsLocal = 1 THEN 'Local'
				ELSE 'Expat' END
				) AS Category
			,dept.DepartmentId
			,dept.[Name] AS DepartmentName
			,gr.GroupId
			,gr.Code AS GroupCode
			,gr.[Name] AS GroupName
			,job.Id AS JobTitleId
			,job.[Name] AS JobTitleName
			,epa.PeriodId
			,epa.PeriodName AS [Name]
			,NULL AS TargetTypeId
			,NULL AS TargetTypeName
			,NULL AS TargetTemplate
			,NULL AS ScoreRatingTypeId
			,NULL AS SelfEvaluated
			,NULL AS Evaluator
			,NULL AS Reviewer
			,NULL AS Management
			,NULL AS Attendance
		FROM #tblEPAs AS tbl WITH (NOLOCK)
			INNER JOIN dbo.EPASummaryImport AS epa WITH (NOLOCK) ON tbl.EPAId = - epa.Id
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.Code = emp.Code
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
			LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.[JobTitle] = job.[Id]
		) AS tblData
	ORDER BY CASE 
			WHEN ISNULL(@SortColumnName, '') <> ''
				AND ISNULL(@SortDirection, '') <> ''
				AND @SortDirection = 'desc' THEN CASE LOWER(@SortColumnName)
					WHEN @SortCode THEN tblData.Code
					WHEN @SortTeam THEN tblData.GroupName
					END
			END DESC
		,CASE 
			WHEN ISNULL(@SortColumnName, '') <> ''
				AND ISNULL(@SortDirection, '') <> ''
				AND @SortDirection = 'asc' THEN CASE LOWER(@SortColumnName)
					WHEN @SortCode THEN tblData.Code
					WHEN @SortTeam THEN tblData.GroupName
					END
			END ASC

	-- Process
	SELECT DISTINCT epa.EPAId
		,pc.ProcessId
		,pc.TotalScore
		,pc.StartDate
		,pc.EndDate
		,(CASE 
			WHEN pc.StartDate > @Today THEN 1
			WHEN @Today BETWEEN pc.StartDate
					AND pc.EndDate THEN 2
			WHEN @Today > pc.EndDate THEN 3 END
			) AS StatusId -- 1:Open; 2:In Progress; 3:Closed
		,emp.EmployeeId AS EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM #tblEPAs AS tbl WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.EPAId = epa.EPAId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId > 1 -- ignore HR phase
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pc.EvaluatorId = emp.EmployeeId
	ORDER BY epa.EPAId ASC
		,pc.ProcessId ASC

	DROP TABLE #tmpTeam
	
	DROP TABLE #tblEPAs

	DROP TABLE #tblEmployees

	DROP TABLE #Periods

	DROP TABLE #tblPeriods

	DROP TABLE #tblDisciplineScores

	DROP TABLE #tblEPAStatus
	
	DROP TABLE #tblLastedEPA
END