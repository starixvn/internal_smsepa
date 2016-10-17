
CREATE PROCEDURE [dbo].[EPA_GetGroup] @ActorUserId INT
	,@ProcessId INT
AS
/*
Created: 20151109@Tim.Trinh
Task: Get Group by Evaluator or Reviewer or Management.
DB: SMSEPA
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
		AND pc.EvaluatorId = @ActorUserId
		AND pc.ProcessId = @ProcessId
		AND CAST(pc.StartDate AS DATE) <= @ToDay
		--AND CAST(pc.EndDate AS DATE) >= @ToDay
		--AND pc.IsEndProcess = 0
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
	WHERE epa.[Status] = 0 -- open
END