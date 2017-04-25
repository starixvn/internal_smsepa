
CREATE PROCEDURE [dbo].[EPA_RatingByKPI_GetKPIByTargetTemplate_xpre] @EvaluatorId INT
	,@PeriodId INT
	,@TargetTemplateId INT
AS
/*
Created: 20150909@Tim.Trinh
Task: Get KPI by Target Template.
DB: SMSEPA
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ToDay DATE

	CREATE TABLE #KPIGroup (
		TargetTemplateId INT
		,KPIGroupId INT
		,NumberKPI INT
		)

	CREATE TABLE #TblKPIGroup (
		TargetTemplateId INT
		,KPIGroupId INT
		,NumberKPIOptional INT
		)

	SET @ToDay = CAST(GETDATE() AS DATE)

	INSERT INTO #KPIGroup
	SELECT DISTINCT dt.TargetTemplateId
		,dt.KPIGroupId
		,COUNT(KPIId) AS NumberKPI
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = 3 -- evaluator phase
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 3 -- evaluator phase
		AND CAST(pc.StartDate AS DATE) <= @ToDay
		AND CAST(pc.EndDate AS DATE) >= @ToDay
		AND pc.IsEndProcess = 0
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
	WHERE pc.EvaluatorId = @EvaluatorId
		AND dt.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId
		AND epa.HasMonthlyEPA = 0
	GROUP BY epa.EPAId
		,dt.TargetTemplateId
		,dt.KPIGroupId

	INSERT INTO #TblKPIGroup
	SELECT DISTINCT dt.TargetTemplateId
		,dt.KPIGroupId
		,COUNT(KPIId) AS NumberKPIOptional
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = 3 -- evaluator phase
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 3 -- evaluator phase
		AND CAST(pc.StartDate AS DATE) <= @ToDay
		AND CAST(pc.EndDate AS DATE) >= @ToDay
		AND pc.IsEndProcess = 0
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
	WHERE pc.EvaluatorId = @EvaluatorId
		AND dt.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId
		AND dt.IsRequiredEvaluate = 0 -- kpi optional
		AND epa.HasMonthlyEPA = 0
	GROUP BY epa.EPAId
		,dt.TargetTemplateId
		,dt.KPIGroupId

	SELECT DISTINCT dt.KPIId
		,khis.[Name]
		,khis.LogNo
		,dt.HasComment
		,dt.IsRequiredComment
		,dt.IsRequiredEvaluate
		,dt.KPIWeight AS [Weight]
		,dt.KPIDisplayOrder AS DisplayOrder
		,(CASE 
			WHEN ISNULL(dt.Score, '') <> '' THEN 1
			ELSE 0 END
			) AS Evaluated
		,dt.KPIGroupId
		,kghis.[Name]
		,dt.KPIGroupWeight AS [Weight]
		,dt.IsSpreadKPI
		,tbl.NumberKPI
		,tbl1.NumberKPIOptional
		,(CASE 
			WHEN tbl1.NumberKPIOptional > 0 THEN 1
			ELSE 0 END
			) AS HasKPIOptional
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = 3 -- evaluator
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 3 -- evaluator
		AND CAST(pc.StartDate AS DATE) <= @ToDay
		AND CAST(pc.EndDate AS DATE) >= @ToDay
		AND pc.IsEndProcess = 0
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.TargetTemplate AS tt WITH (NOLOCK) ON dt.TargetTemplateId = tt.TargetTemplateId
		INNER JOIN dbo.KPIHistory AS khis WITH (NOLOCK) ON dt.KPIId = khis.KPIId
		AND dt.KPILogNo = khis.LogNo
		INNER JOIN dbo.KPIGroupHistory AS kghis WITH (NOLOCK) ON dt.KPIGroupId = kghis.KPIGroupId
		AND dt.KPIGroupLogNo = kghis.LogNo
		INNER JOIN #KPIGroup AS tbl ON dt.TargetTemplateId = tbl.TargetTemplateId
		AND dt.KPIGroupId = tbl.KPIGroupId
		LEFT JOIN #TblKPIGroup AS tbl1 ON dt.TargetTemplateId = tbl1.TargetTemplateId
		AND dt.KPIGroupId = tbl1.KPIGroupId
	WHERE dt.EvaluatorId = @EvaluatorId
		AND pc.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND tt.TargetTemplateId = @TargetTemplateId
		AND epa.HasMonthlyEPA = 0
	ORDER BY KPIGroupId
		,dt.KPIDisplayOrder

	DROP TABLE #KPIGroup

	DROP TABLE #TblKPIGroup
END;