
CREATE PROCEDURE [dbo].[EPA_GetSemiById] @EPAId INT
AS
/*
Created: 20160203@Razor.Nguyen
Task: Get Semi EPA by Id.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @MonthlyEPAIds IntId
		,@Processes ProcessTable
		,@EmployeeId INT
		,@StartDate DATE
		,@EndDate DATE

	SELECT @Employeeid = semi.EmployeeId
		,@StartDate = p.StartDate
		,@EndDate = p.EndDate
	FROM [dbo].[EPA] AS semi WITH (NOLOCK)
		LEFT JOIN Period AS p WITH (NOLOCK) ON p.PeriodId = semi.PeriodId
	WHERE semi.EPAId = @EPAId
		AND semi.HasMonthlyEPA = 1

	INSERT INTO @MonthlyEPAIds (Id)
	SELECT mon.EPAId
	FROM [dbo].[EPA] AS mon WITH (NOLOCK)
		LEFT JOIN Period p ON p.PeriodId = mon.PeriodId
		AND p.StartDate >= @StartDate
		AND p.StartDate <= @EndDate
	WHERE mon.EmployeeId = @EmployeeId
		AND mon.HasMonthlyEPA = 0

	INSERT INTO @Processes (
		ProcessId
		,EvaluatorId
		)
	SELECT DISTINCT pc.ProcessId
		,pc.EvaluatorId
	FROM @MonthlyEPAIds AS epat
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.Id = epa.EPAId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId <> 1 -- ignore hr phase

	-- General Info
	EXECUTE dbo.EPA_GetGeneralInfo @MonthlyEPAIds

	-- Process
	EXECUTE dbo.EPA_GetProcess @MonthlyEPAIds
		,@Processes

	-- Last performance score
	EXECUTE dbo.EPA_GetLastPerformanceScore @EPAId
END