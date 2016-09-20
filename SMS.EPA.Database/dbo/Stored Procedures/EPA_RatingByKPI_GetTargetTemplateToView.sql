
CREATE PROCEDURE [dbo].[EPA_RatingByKPI_GetTargetTemplateToView] @EvaluatorId INT
	,@PeriodId INT
AS
/*
Created: 20150909@Razor.Nguyen
Task: Get Target Template by Evaluator.
DB: SMSEPA
Original: 
  Revisions: 

Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ToDay DATE

	SET @ToDay = CAST(GETDATE() AS DATE)

	SELECT DISTINCT tt.TargetTemplateId
		,tt.[Name]
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId IN (4, 5)
		AND pc.IsEndProcess = 0
		AND CAST(pc.StartDate AS DATE) <= @ToDay
		AND CAST(pc.EndDate AS DATE) >= @ToDay
		INNER JOIN dbo.EPAProcess AS pc1 WITH (NOLOCK) ON epa.EPAId = pc1.EPAId
		AND pc1.ProcessId = 3
		AND pc1.IsEndProcess = 1
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		AND period.TargetTypeId IN (2, 3) -- Monthly, semi
		INNER JOIN dbo.PeriodTargetSetting AS pts WITH (NOLOCK) ON period.PeriodId = pts.PeriodId
		INNER JOIN dbo.TargetTemplate AS tt WITH (NOLOCK) ON pts.TargetTemplateId = tt.TargetTemplateId
		AND pts.TargetTemplateId = dt.TargetTemplateId
	WHERE epa.[Status] = 0 -- open
		AND epa.HasMonthlyEPA = 0
		AND pc.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
	ORDER BY tt.[Name]
END