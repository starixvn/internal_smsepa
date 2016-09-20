
CREATE PROCEDURE [dbo].[EPA_RatingByKPI_GetGroup] @EvaluatorId INT
AS
/*
Created: 20151012@Tim.Trinh
Task: Get all Group by Evaluator login.
DB: SPM
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ToDay DATE

	SET @ToDay = CAST(GETDATE() AS DATE)

	SELECT DISTINCT gr.[GroupId]
		,gr.[Name] AS GroupName
		,gr.[Code] AS GroupCode
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 3
		AND pc.EvaluatorId = @EvaluatorId
		AND CAST(pc.StartDate AS DATE) <= @ToDay
		AND CAST(pc.EndDate AS DATE) >= @ToDay
		AND pc.IsEndProcess = 0
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = 3
		AND dt.EvaluatorId = @EvaluatorId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
	WHERE epa.[Status] = 0 -- open
END