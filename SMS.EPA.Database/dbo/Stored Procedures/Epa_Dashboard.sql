
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

	CREATE TABLE #tblDisciplineScores (
		EPAId INT
		,SystemDisciplineScore FLOAT
		,ApproverSubtractScore FLOAT
		,OtherDisciplines FLOAT
		)

	CREATE TABLE #tblOtherViolation (
		EPAId INT
		,NumberOtherViolation INT
		)

	CREATE TABLE #tblEPAStatus (
		EPAId INT
		,StatusId TINYINT
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
	WHERE CAST(selfpc.StartDate AS DATE) <= @ToDay

	INSERT INTO #tblOtherViolation (
		EPAId
		,NumberOtherViolation
		)
	SELECT epa.EPAId
		,COUNT(1) AS NumberOtherViolation
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN @TblEPAIds AS epat ON epa.EPAId = epat.Id
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.EmployeeDiscipline AS dis WITH (NOLOCK) ON epa.EmployeeId = dis.EmpId
		AND CAST(dis.WorkDate AS DATE) BETWEEN CAST(period.AttendanceStartDate AS DATE)
			AND CAST(period.AttendanceEndDate AS DATE)
		INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON dis.DisciplineTypeId = dtype.DisciplineTypeId
		AND dtype.DisciplineTypeId < 0
	GROUP BY epa.EPAId

	INSERT INTO #tblEPAStatus (
		EPAId
		,StatusId
		)
	SELECT epa.EPAId
		,(CASE 
			WHEN tt.TargetTypeId IN (2, 3) THEN (CASE 
					WHEN CAST(pc.EndDate AS DATE) >= CAST(period.EndDate AS DATE) THEN (CASE 
							WHEN CAST(pc.EndDate AS DATE) < @ToDay THEN 1
							ELSE 0 END
							)
					ELSE (CASE 
							WHEN CAST(period.EndDate AS DATE) <= @ToDay THEN 1
							ELSE 0 END
							) END
					)
			ELSE (CASE 
					WHEN CAST(pc.EndDate AS DATE) < @ToDay THEN 1
					ELSE 0 END
					) END
			)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN @TblEPAIds AS epaId ON epa.EPAId = epaId.Id
		INNER JOIN (
		SELECT pc.EPAId
			,MAX(pc.EndDate) AS EndDate
		FROM dbo.EPAProcess AS pc WITH (NOLOCK)
			INNER JOIN @TblEPAIds AS epaId1 ON pc.EPAId = epaId1.Id
		GROUP BY pc.EPAId
		) AS pc ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId

	SELECT @EPAIds = COALESCE(@EPAIds + ',', '') + CAST(Id AS NVARCHAR(100))
	FROM @TblEPAIds

	INSERT INTO #tblDisciplineScores (
		EPAId
		,SystemDisciplineScore
		,ApproverSubtractScore
		,OtherDisciplines
		)
	EXECUTE dbo.EmployeeDisciplineSummary_GetByEPA @EPAIds

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
				WHEN tblstatus.StatusId = 1 THEN epa.DisciplineScore
				ELSE (CASE 
						WHEN ds.ApproverSubtractScore IS NULL THEN 0
						ELSE ds.ApproverSubtractScore END
						) END
				) AS DisciplineScore
			,ds.OtherDisciplines
			,srt.Attendance AS MaxDisciplineScore
			,(CASE 
				WHEN tbl.NumberOtherViolation > 0 THEN 1
				ELSE 0 END
				) AS HasOtherViolation
			,selfpc.IsEndProcess AS IsEndProcessBySelf
			,tblstatus.StatusId
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
			,tt.TargetTypeId
			,tt.[Name] AS TargetTypeName
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN @TblEPAIds AS epat ON epa.EPAId = epat.Id
			INNER JOIN (
			SELECT pc.EPAId
				,MIN(pc.StartDate) AS StartDate
				,MAX(pc.EndDate) AS EndDate
			FROM dbo.EPAProcess AS pc WITH (NOLOCK)
				INNER JOIN @TblEPAIds AS epaId1 ON pc.EPAId = epaId1.Id
			GROUP BY pc.EPAId
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
			INNER JOIN dbo.#tblEPAStatus AS tblstatus WITH (NOLOCK) ON epa.EPAId = tblstatus.EPAId
			LEFT JOIN #tblDisciplineScores AS ds ON epa.EPAId = ds.EPAId
			LEFT JOIN #tblOtherViolation AS tbl ON epa.EPAId = tbl.EPAId
		
		UNION ALL
		SELECT DISTINCT - epa.Id AS EPAId
			,epa.FinalScore
			,0 AS HasMonthlyEPA
			,0 AS HasDisciplineScore
			,0 AS DisciplineScore
			,0 AS OtherDisciplines
			,0 AS MaxDisciplineScore
			,0 AS HasOtherViolation
			,1 AS IsEndProcessBySelf
			,1 AS StatusId
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
			,epa.PeriodId AS PeriodId
			,epa.PeriodName AS [Name]
			,epa.PeriodDate AS StartDate
			,DATEADD(DAY, 29, [PeriodDate]) AS EndDate
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

	DROP TABLE #tblDisciplineScores

	DROP TABLE #tblOtherViolation

	DROP TABLE #tblEPAStatus
END