CREATE PROCEDURE [dbo].[Epa_Dashboard] @EmployeeId INT
AS
/*
Created: 20150904@Tim.Trinh
Task: Get data for Dashboard.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TblEPAIds IntId
		,@EPAIds NVARCHAR(MAX)
		,@ToDay DATE

	CREATE TABLE #TblDisciplineScores (
		EPAId INT
		,SystemDisciplineScore FLOAT
		,ApproverSubtractScore FLOAT
		)

	SET @ToDay = CAST(GETDATE() AS DATE)

	INSERT INTO @TblEPAIds (Id)
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS selfpc WITH (NOLOCK) ON epa.EPAId = selfpc.EPAId
		AND selfpc.ProcessId = 2
		AND selfpc.EvaluatorId = @EmployeeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.EmployeeId = @EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
	WHERE (
			epa.[Status] = 0 -- inprogress
			AND CAST(selfpc.StartDate AS DATE) <= @ToDay
			)
		OR epa.[Status] = 1 -- closed

	SELECT @EPAIds = COALESCE(@EPAIds + ',', '') + CAST(Id AS NVARCHAR)
	FROM @TblEPAIds

	INSERT INTO #TblDisciplineScores
	EXECUTE dbo.EpaAttendanceSummary_GetByEPA @EPAIds

	SELECT tblData.*
	FROM (
		SELECT DISTINCT epa.EPAId
			,epa.FinalScore
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
			,(CASE 
				WHEN tbl.NumberOtherViolation > 0 THEN 1
				ELSE 0 END
				) AS HasOtherViolation
			,selfpc.IsEndProcess AS IsEndProcessBySelf
			,emp.EmployeeId
			,emp.Code
			,emp.NickName
			,emp.FullName
			,emp.Email
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
			,CAST(pc.StartDate AS DATE) AS StartDate
			,(CASE 
				WHEN tt.TargetTypeId IN (2, 3) THEN CASE 
						WHEN CAST(pc.EndDate AS DATE) >= CAST(period.EndDate AS DATE) THEN CAST(pc.EndDate AS DATE)
						ELSE period.EndDate
						END
				ELSE CAST(pc.EndDate AS DATE) END
				) AS EndDate
			,(CASE 
				WHEN tt.TargetTypeId IN (2, 3) THEN (CASE 
						WHEN CAST(pc.EndDate AS DATE) >= CAST(period.EndDate AS DATE) THEN (CASE 
								WHEN CAST(pc.EndDate AS DATE) < @ToDay
									AND epa.[Status] = 1 THEN 1
								ELSE 0 END
								)
						ELSE (CASE 
								WHEN CAST(period.EndDate AS DATE) <= @ToDay
									AND epa.[Status] = 1 THEN 1
								ELSE 0 END
								) END
						)
				ELSE (CASE 
						WHEN CAST(pc.EndDate AS DATE) < @ToDay
							AND epa.[Status] = 1 THEN 1
						ELSE 0 END
						) END
				) AS IsEndPeriod
			,tt.TargetTypeId
			,tt.[Name] AS TargetTypeName
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN @TblEPAIds AS epat ON epa.EPAId = epat.Id
			INNER JOIN (
			SELECT pc1.EPAId
				,MIN(pc1.StartDate) AS StartDate
				,MAX(pc1.EndDate) AS EndDate
			FROM dbo.EPAProcess AS pc1 WITH (NOLOCK)
			GROUP BY pc1.EPAId
			) AS pc ON epa.EPAId = pc.EPAId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
			LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.[JobTitle] = job.[Id]
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.IsActive = 1
			INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
			INNER JOIN dbo.EPAProcess AS selfpc WITH (NOLOCK) ON epa.EPAId = selfpc.EPAId
			AND selfpc.ProcessId = 2 -- self
			INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
			LEFT JOIN #TblDisciplineScores AS ds ON epa.EPAId = ds.EPAId
			LEFT JOIN (
			SELECT epa1.EPAId
				,COUNT(*) AS NumberOtherViolation
			FROM dbo.EPA AS epa1 WITH (NOLOCK)
				INNER JOIN @TblEPAIds AS epat1 ON epa1.EPAId = epat1.Id
				INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa1.PeriodId = period1.PeriodId
				AND period1.IsActive = 1
				INNER JOIN dbo.EmployeeOtherViolation AS ov WITH (NOLOCK) ON epa1.EmployeeId = ov.EmpId
				AND CAST(ov.WorkDate AS DATE) BETWEEN CAST(period1.AttendanceStartDate AS DATE)
					AND CAST(period1.AttendanceEndDate AS DATE)
			GROUP BY epa1.EPAId
			) AS tbl ON epa.EPAId = tbl.EPAId
		
		UNION ALL
		SELECT DISTINCT - epa.Id AS EPAId
			,epa.FinalScore
			,0 AS HasMonthlyEPA
			,0 AS HasDisciplineScore
			,0 AS DisciplineScore
			,0 AS MaxDisciplineScore
			,0 AS HasOtherViolation
			,1 AS IsEndProcessBySelf
			,emp.EmployeeId
			,emp.Code
			,emp.NickName
			,emp.FullName
			,emp.Email
			,dept.DepartmentId
			,dept.[Name] AS DepartmentName
			,gr.GroupId
			,gr.Code AS GroupCode
			,gr.[Name] AS GroupName
			,job.Id AS JobTitleId
			,job.[Name] AS JobTitleName
			,1 AS StatusId
			,epa.PeriodId AS PeriodId
			,epa.PeriodName AS [Name]
			,epa.PeriodDate AS StartDate
			,DATEADD(DAY, 29, [PeriodDate]) AS EndDate
			,(1) AS IsEndPeriod
			,NULL AS TargetTypeId
			,NULL AS TargetTypeName
		FROM dbo.EPASummaryImport AS epa WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.Code = emp.Code
			AND emp.EmployeeId = @EmployeeId
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
			LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.[JobTitle] = job.[Id]
		) AS tblData
	ORDER BY tblData.StartDate DESC

	EXECUTE dbo.EPA_GetProcess @TblEPAIds

	DROP TABLE #TblDisciplineScores
END