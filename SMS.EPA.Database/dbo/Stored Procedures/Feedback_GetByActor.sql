
CREATE PROCEDURE [dbo].[Feedback_GetByActor] @ActorId INT
	,@PeriodId INT
AS
/*
Created: 20200526@Tim.Trinh
Task: Get feedback by actor
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	SELECT fb.FeedbackId
		,period.[Name] AS Period
		,fb.FromDate
		,fb.ToDate
		,fb.FinalScore
		,fb.EvaluatedDate
		,fb.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,gr.[Name] AS Section
		,emp.JoinedDate
		,job.[Name] AS JobTitle
	FROM dbo.Feedback AS fb WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON fb.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON fb.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
	WHERE fb.EvaluatorId = @ActorId
		AND fb.PeriodId = @PeriodId
		AND fb.FromDate <= @Today
END