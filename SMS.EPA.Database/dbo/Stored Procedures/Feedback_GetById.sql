
CREATE PROCEDURE [dbo].[Feedback_GetById] @FeedbackId BIGINT
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

	SELECT fb.FeedbackId
		,period.[Name] AS Period
		,fb.FromDate
		,fb.ToDate
		,fb.FinalScore
		,fb.Comment
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
	WHERE fb.FeedbackId = @FeedbackId

	SELECT fbkg.KPIGroupId
		,kgh.[Name]
		,fbkg.IsSpreadKPI
		,fbkg.[Weight]
		,fbkg.DisplayOrder
	FROM dbo.FeedbackKPIGroup AS fbkg WITH (NOLOCK)
		INNER JOIN dbo.KPIGroupHistory AS kgh WITH (NOLOCK) ON fbkg.KPIGroupId = kgh.KPIGroupId
		AND fbkg.LogNo = kgh.LogNo
	WHERE fbkg.FeedbackId = @FeedbackId
	ORDER BY fbkg.DisplayOrder

	SELECT fbkpi.KPIGroupId
		,fbkpi.KPIId
		,kpih.[Name]
		,fbkpi.HasComment
		,fbkpi.IsRequiredComment
		,fbkpi.IsRequiredEvaluate
		,fbkpi.[Weight]
		,fbkpi.Score
		,fbkpi.Rate
		,fbkpi.Comment
		,fbkpi.DisplayOrder
	FROM dbo.FeedbackKPI AS fbkpi WITH (NOLOCK)
		INNER JOIN dbo.FeedbackKPIGroup AS fbkg WITH (NOLOCK) ON fbkpi.FeedbackId = fbkg.FeedbackId
		AND fbkpi.KPIGroupId = fbkg.KPIGroupId
		INNER JOIN dbo.KPIHistory AS kpih WITH (NOLOCK) ON fbkpi.KPIId = kpih.KPIId
		AND fbkpi.LogNo = kpih.LogNo
	WHERE fbkpi.FeedbackId = @FeedbackId
	ORDER BY fbkg.DisplayOrder
		,fbkpi.DisplayOrder
END