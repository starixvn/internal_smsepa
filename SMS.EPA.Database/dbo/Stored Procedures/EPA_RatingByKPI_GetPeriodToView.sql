
CREATE PROCEDURE [dbo].[EPA_RatingByKPI_GetPeriodToView] @EvaluatorId INT
AS
/*
Created: 20160530@Tim.Trinh
Task: Get Target Template by Evaluator.
DB: SMSEPA
Original: 
  Revisions: 

Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Today DATE = GETDATE()

	SELECT DISTINCT period.PeriodId
		,period.[Name]
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId IN (4, 5)
		AND pc.IsEndProcess = 0
		AND @Today BETWEEN pc.StartDate
			AND pc.EndDate
		INNER JOIN dbo.EPAProcess AS pc1 WITH (NOLOCK) ON epa.EPAId = pc1.EPAId
		AND pc1.ProcessId = 3
		AND pc1.EndDate < @Today
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		AND period.TargetTypeId IN (2, 3) -- Monthly, semi
		INNER JOIN dbo.PeriodTargetSetting AS pts WITH (NOLOCK) ON period.PeriodId = pts.PeriodId
		INNER JOIN dbo.TargetTemplate AS tt WITH (NOLOCK) ON pts.TargetTemplateId = tt.TargetTemplateId
		AND pts.TargetTemplateId = dt.TargetTemplateId
	WHERE epa.[Status] = 0 -- open
		AND epa.HasMonthlyEPA = 0
		AND pc.EvaluatorId = @EvaluatorId
	ORDER BY period.[Name] ASC
END