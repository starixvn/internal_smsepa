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
		,@ToDay DATE = GETDATE()
		,@NumberOfEmployeesFinishedByEvaluator INT
		,@NumberOfEmployeesUnfinishedByEvaluator INT
		,@NumberOfEmployeesFinishedByReviewer INT
		,@NumberOfEmployeesUnfinishedByReviewer INT
		,@NumberOfEmployeesFinishedByManagement INT
		,@NumberOfEmployeesUnfinishedByManagement INT
		,@Inprogress TINYINT = 0

	INSERT INTO @PeriodIds (Id)
	SELECT DISTINCT period.PeriodId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId IN (3, 4, 5) -- evaluator, reviewer, management
		AND @ToDay >= pc.StartDate
	WHERE pc.EvaluatorId = @EmployeeId
		AND ISNULL(epa.[Status], 0) = @Inprogress

	-- evaluator
	SELECT @NumberOfEmployeesFinishedByEvaluator = COUNT(1)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 3 -- evaluator
	WHERE ISNULL(epa.[Status], 0) = @Inprogress
		AND pc.EvaluatorId = @EmployeeId
		AND @ToDay > pc.EndDate
	GROUP BY period.PeriodId

	SELECT @NumberOfEmployeesUnfinishedByEvaluator = COUNT(1)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 3 -- evaluator
	WHERE ISNULL(epa.[Status], 0) = @Inprogress
		AND pc.EvaluatorId = @EmployeeId
		AND epa.HasMonthlyEPA = 0
		AND @ToDay BETWEEN pc.StartDate
			AND pc.EndDate
	GROUP BY period.PeriodId

	-- reviewer
	SELECT @NumberOfEmployeesFinishedByReviewer = COUNT(1)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 4 -- reviewer
	WHERE ISNULL(epa.[Status], 0) = @Inprogress
		AND pc.EvaluatorId = @EmployeeId
		AND @ToDay > pc.EndDate
	GROUP BY period.PeriodId

	SELECT @NumberOfEmployeesUnfinishedByReviewer = COUNT(1)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 4 -- reviewer
	WHERE ISNULL(epa.[Status], 0) = @Inprogress
		AND pc.EvaluatorId = @EmployeeId
		AND @ToDay BETWEEN pc.StartDate
			AND pc.EndDate
	GROUP BY period.PeriodId

	-- management
	SELECT @NumberOfEmployeesFinishedByManagement = COUNT(1)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 5 -- management
	WHERE ISNULL(epa.[Status], 0) = @Inprogress
		AND pc.EvaluatorId = @EmployeeId
		AND @ToDay > pc.EndDate
	GROUP BY period.PeriodId

	SELECT @NumberOfEmployeesUnfinishedByManagement = COUNT(1)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 5 -- management
	WHERE ISNULL(epa.[Status], 0) = @Inprogress
		AND pc.EvaluatorId = @EmployeeId
		AND @ToDay BETWEEN pc.StartDate
			AND pc.EndDate
	GROUP BY period.PeriodId

	SELECT @NumberOfEmployeesFinishedByEvaluator AS NumberOfEmployeesFinishedByEvaluator
		,@NumberOfEmployeesUnfinishedByEvaluator AS NumberOfEmployeesUnfinishedByEvaluator
		,@NumberOfEmployeesFinishedByReviewer AS NumberOfEmployeesFinishedByReviewer
		,@NumberOfEmployeesUnfinishedByReviewer AS NumberOfEmployeesUnfinishedByReviewer
		,@NumberOfEmployeesFinishedByManagement AS NumberOfEmployeesFinishedByManagement
		,@NumberOfEmployeesUnfinishedByManagement AS NumberOfEmployeesUnfinishedByManagement
		,period.PeriodId
		,period.[Name]
		,ttype.TargetTypeId
		,ttype.[Name]
	FROM @PeriodIds AS tbl
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON tbl.Id = period.PeriodId
		INNER JOIN dbo.TargetType AS ttype WITH (NOLOCK) ON period.TargetTypeId = ttype.TargetTypeId
END