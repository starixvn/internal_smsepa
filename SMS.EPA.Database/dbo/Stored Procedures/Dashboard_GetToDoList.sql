
CREATE PROCEDURE [dbo].[Dashboard_GetToDoList] @EmployeeId INT
AS
/*
Created: 20160928@Tim.Trinh
Task: Get to do list.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @PeriodIds IntId
		,@ToDay DATE

	SET @ToDay = CAST(GETDATE() AS DATE)

	INSERT INTO @PeriodIds (Id)
	SELECT DISTINCT period.PeriodId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId IN (3, 4, 5) -- evaluator, reviewer, management
		AND @ToDay >= CAST(pc.StartDate AS DATE)
			--AND CAST(pc.EndDate AS DATE)
	WHERE pc.EvaluatorId = @EmployeeId
		AND epa.[Status] = 0 -- inprogress

	SELECT evaluator1.NumberOfEmployeesFinished AS NumberOfEmployeesFinishedByEvaluator
		,evaluator2.NumberOfEmployeesUnfinished AS NumberOfEmployeesUnfinishedByEvaluator
		,reviewer1.NumberOfEmployeesFinished AS NumberOfEmployeesFinishedByReviewer
		,reviewer2.NumberOfEmployeesUnfinished AS NumberOfEmployeesUnfinishedByReviewer
		,management1.NumberOfEmployeesUnfinished AS NumberOfEmployeesFinishedByManagement
		,management2.NumberOfEmployeesUnfinished AS NumberOfEmployeesUnfinishedByManagement
		,period.PeriodId
		,period.[Name]
		,ttype.TargetTypeId
		,ttype.[Name]
	FROM @PeriodIds AS tbl
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON tbl.Id = period.PeriodId
		INNER JOIN dbo.TargetType AS ttype WITH (NOLOCK) ON period.TargetTypeId = ttype.TargetTypeId
		LEFT JOIN (
		SELECT DISTINCT period.PeriodId
			,COUNT(*) AS NumberOfEmployeesFinished
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.IsActive = 1
			INNER JOIN dbo.EPAProcess AS pc ON epa.EPAId = pc.EPAId
			AND pc.ProcessId = 3 -- evaluator
		WHERE epa.[Status] = 0 -- inprogress
			AND pc.EvaluatorId = @EmployeeId
			AND @ToDay > CAST(pc.EndDate AS DATE)
		GROUP BY period.PeriodId
		) AS evaluator1 ON period.PeriodId = evaluator1.PeriodId
		LEFT JOIN (
		SELECT DISTINCT period.PeriodId
			,COUNT(*) AS NumberOfEmployeesUnfinished
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.IsActive = 1
			INNER JOIN dbo.EPAProcess AS pc ON epa.EPAId = pc.EPAId
			AND pc.ProcessId = 3 -- evaluator
		WHERE epa.[Status] = 0 -- inprogress
			AND pc.EvaluatorId = @EmployeeId
			AND epa.HasMonthlyEPA = 0
			AND @ToDay BETWEEN CAST(pc.StartDate AS DATE)
				AND CAST(pc.EndDate AS DATE)
		GROUP BY period.PeriodId
		) AS evaluator2 ON period.PeriodId = evaluator2.PeriodId
		LEFT JOIN (
		SELECT DISTINCT period.PeriodId
			,COUNT(*) AS NumberOfEmployeesFinished
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.IsActive = 1
			INNER JOIN dbo.EPAProcess AS pc ON epa.EPAId = pc.EPAId
			AND pc.ProcessId = 4 -- reviewer
		WHERE epa.[Status] = 0 -- inprogress
			AND pc.EvaluatorId = @EmployeeId
			AND @ToDay > CAST(pc.EndDate AS DATE)
		GROUP BY period.PeriodId
		) AS reviewer1 ON period.PeriodId = reviewer1.PeriodId
		LEFT JOIN (
		SELECT DISTINCT period.PeriodId
			,COUNT(*) AS NumberOfEmployeesUnfinished
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.IsActive = 1
			INNER JOIN dbo.EPAProcess AS pc ON epa.EPAId = pc.EPAId
			AND pc.ProcessId = 4 -- reviewer
		WHERE epa.[Status] = 0 -- inprogress
			AND pc.EvaluatorId = @EmployeeId
			AND @ToDay BETWEEN CAST(pc.StartDate AS DATE)
				AND CAST(pc.EndDate AS DATE)
		GROUP BY period.PeriodId
		) AS reviewer2 ON period.PeriodId = reviewer2.PeriodId
		LEFT JOIN (
		SELECT DISTINCT period.PeriodId
			,COUNT(*) AS NumberOfEmployeesUnfinished
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.IsActive = 1
			INNER JOIN dbo.EPAProcess AS pc ON epa.EPAId = pc.EPAId
			AND pc.ProcessId = 5 -- management
		WHERE epa.[Status] = 0 -- inprogress
			AND pc.EvaluatorId = @EmployeeId
			AND @ToDay > CAST(pc.EndDate AS DATE)
		GROUP BY period.PeriodId
		) AS management1 ON period.PeriodId = management1.PeriodId
		LEFT JOIN (
		SELECT DISTINCT period.PeriodId
			,COUNT(*) AS NumberOfEmployeesUnfinished
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.IsActive = 1
			INNER JOIN dbo.EPAProcess AS pc ON epa.EPAId = pc.EPAId
			AND pc.ProcessId = 5 -- management
		WHERE epa.[Status] = 0 -- inprogress
			AND pc.EvaluatorId = @EmployeeId
			AND @ToDay BETWEEN CAST(pc.StartDate AS DATE)
				AND CAST(pc.EndDate AS DATE)
		GROUP BY period.PeriodId
		) AS management2 ON period.PeriodId = management2.PeriodId
END