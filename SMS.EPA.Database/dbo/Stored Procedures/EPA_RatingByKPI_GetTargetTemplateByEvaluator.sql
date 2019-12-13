
CREATE PROCEDURE [dbo].[EPA_RatingByKPI_GetTargetTemplateByEvaluator] @PeriodId INT
	,@EvaluatorId INT
AS
/*
Created: 20150909@Tim.Trinh
Task: Get Target Template by Evaluator.
DB: SMSEPA
Original: 
  Revisions: 

Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Today DATE = GETDATE()

	SELECT DISTINCT tt.TargetTemplateId
		,tt.[Name]
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 3 -- Evaluator
		AND @Today BETWEEN pc.StartDate
			AND pc.EndDate
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		AND period.TargetTypeId IN (2, 3) -- Monthly, semi
		INNER JOIN dbo.PeriodTargetSetting AS pts WITH (NOLOCK) ON period.PeriodId = pts.PeriodId
		INNER JOIN dbo.TargetTemplate AS tt WITH (NOLOCK) ON pts.TargetTemplateId = tt.TargetTemplateId
		AND pts.TargetTemplateId = dt.TargetTemplateId
	WHERE pc.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND epa.[Status] = 0 -- open
		AND epa.HasMonthlyEPA = 0
	ORDER BY tt.[Name] ASC
END