
CREATE PROCEDURE [dbo].[API_GO_GetTodolistEPAByUserId] @UserId INT
AS
/*
Created: 20230421@Tim.Trinh
Task: Get to-do list.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmpId INT
		,@EmployeeProcessId TINYINT = 2
		,@Today DATE = GETDATE()

	SELECT @EmpId = EmployeeId
	FROM dbo.Employee WITH (NOLOCK)
	WHERE UserId = @UserId

	SELECT period.PeriodId
		,period.[Name]
		,period.TargetTypeId
		,epap.ProcessId
		,MIN(epap.StartDate) AS StartDate
		,MAX(epap.EndDate) AS EndDate
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.EPAProcess AS epap WITH (NOLOCK) ON epa.EPAId = epap.EPAId
	WHERE epap.EvaluatorId = @EmpId
		AND epap.ProcessId >= @EmployeeProcessId
		AND ISNULL(epa.[Status], 0) = 0 -- open
		AND @Today BETWEEN epap.StartDate
			AND epap.EndDate
	GROUP BY period.PeriodId
		,period.[Name]
		,period.TargetTypeId
		,epap.ProcessId
	ORDER BY period.PeriodId
		,epap.ProcessId
END