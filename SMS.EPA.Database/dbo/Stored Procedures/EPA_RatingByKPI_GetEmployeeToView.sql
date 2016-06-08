

CREATE PROCEDURE [dbo].[EPA_RatingByKPI_GetEmployeeToView] @PeriodId INT
	,@TargetTemplateId INT
	,@KPIId INT
	,@EvaluatorId INT
AS
/*
Created: 20160322@Razor.Nguyen
Task: Monthly EPA
DB: SMSEPA
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @RequiredCommentOnMaxScore BIT

	CREATE TABLE #EPA (
		EPAId INT
		,KPIGroupId INT
		)

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

	INSERT INTO #KPIGroup
	SELECT DISTINCT dt.TargetTemplateId
		,dt.KPIGroupId
		,COUNT(KPIId) AS NumberKPI
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = 3 -- evaluator phase
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 3 -- evaluator phase
		AND pc.IsEndProcess = 1
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
	WHERE epa.HasMonthlyEPA = 0
		AND dt.EvaluatorId = @EvaluatorId
		AND pc.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId
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
		AND pc.IsEndProcess = 1
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
	WHERE dt.IsRequiredEvaluate = 0 -- kpi optional
		AND epa.HasMonthlyEPA = 0
		AND dt.EvaluatorId = @EvaluatorId
		AND pc.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId
	GROUP BY epa.EPAId
		,dt.TargetTemplateId
		,dt.KPIGroupId

	SELECT @RequiredCommentOnMaxScore = CAST(lkd.Value AS BIT)
	FROM dbo.LookupData AS lkd WITH (NOLOCK)
		INNER JOIN dbo.[Lookup] AS lk WITH (NOLOCK) ON lkd.LookupId = lk.Id
		AND lk.Id = 7 -- required comment on max score	

	INSERT INTO #EPA
	SELECT DISTINCT epa.EPAId
		,dt.KPIGroupId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 3 -- evaluator phase
		AND pc.IsEndProcess = 1
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = 3 -- evaluator phase
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.PeriodTargetSetting AS pts WITH (NOLOCK) ON period.PeriodId = pts.PeriodId
		INNER JOIN dbo.TargetTemplate AS tt WITH (NOLOCK) ON pts.TargetTemplateId = tt.TargetTemplateId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE epa.[Status] = 0 -- open
		AND epa.HasMonthlyEPA = 0
		AND pc.EvaluatorId = @EvaluatorId
		AND dt.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId
		AND tt.TargetTemplateId = @TargetTemplateId
		AND dt.KPIId = @KPIId

	SELECT DISTINCT epa.EPAId
		,pc.TotalScore
		,@RequiredCommentOnMaxScore AS RequiredCommentOnMaxScore
		,emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,dept.DepartmentId
		,dept.[Name]
		,gr.GroupId
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,job.Id AS JobTitleId
		,job.[Name] AS JobTitleName
		,CAST(lkd.Value AS TINYINT) AS RatingTypeId
		,lkd.Code
		,lkd.[Name]
		,dt.KPIGroupId
		,kghis.[Name]
		,dt.IsSpreadKPI
		,dt.KPIGroupWeight AS [Weight]
		,tbl1.NumberKPI
		,tbl2.NumberKPIOptional
		,(CASE 
			WHEN tbl2.NumberKPIOptional > 0 THEN 1
			ELSE 0 END
			) AS HasKPIOptional
	FROM #EPA AS tbl
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.EPAId = epa.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
		INNER JOIN dbo.LookupData AS lkd WITH (NOLOCK) ON epa.RatingTypeId = CAST(lkd.Value AS TINYINT)
		INNER JOIN dbo.[Lookup] AS lk WITH (NOLOCK) ON lkd.LookupId = lk.Id
		AND lk.Id = 8 -- rating type
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON tbl.EPAId = dt.EPAId
		AND tbl.KPIGroupId = dt.KPIGroupId
		AND dt.ProcessId = 3 -- evaluator phase
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 3 -- evaluator phase
		INNER JOIN dbo.KPIGroupHistory AS kghis WITH (NOLOCK) ON dt.KPIGroupId = kghis.KPIGroupId
		AND dt.KPIGroupLogNo = kghis.LogNo
		INNER JOIN #KPIGroup AS tbl1 ON dt.TargetTemplateId = tbl1.TargetTemplateId
		AND dt.KPIGroupId = tbl1.KPIGroupId
		LEFT JOIN #TblKPIGroup AS tbl2 ON dt.TargetTemplateId = tbl2.TargetTemplateId
		AND dt.KPIGroupId = tbl2.KPIGroupId
	WHERE pc.EvaluatorId = @EvaluatorId
		AND dt.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId
	ORDER BY emp.NickName
		,emp.Code

	SELECT DISTINCT epa.EPAId
		,dt.KPIId
		,khis.[Name]
		,dt.HasComment
		,dt.IsRequiredComment
		,dt.IsRequiredEvaluate
		,dt.[KPIWeight] AS [Weight]
		,dt.KPIDisplayOrder AS DisplayOrder
		,dt.Score
		,dt.Rate
		,dt.IsMaxScore
		,dt.Comment
		,dt.KPIGroupId
		,dt.EvaluatorId
	FROM #EPA AS tbl
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.EPAId = epa.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON tbl.EPAId = pc.EPAId
		AND pc.ProcessId = 3
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON tbl.EPAId = dt.EPAId
		AND tbl.KPIGroupId = dt.KPIGroupId
		AND dt.ProcessId = 3
		INNER JOIN dbo.KPIHistory AS khis WITH (NOLOCK) ON dt.KPIId = khis.KPIId
		AND dt.KPILogNo = khis.LogNo
	WHERE pc.EvaluatorId = @EvaluatorId
		AND dt.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId
	ORDER BY epa.EPAId
		,dt.KPIDisplayOrder

	SELECT DISTINCT epa.EPAId
		,dt.KPIId
		,khis.[Name]
		,dt.HasComment
		,dt.IsRequiredComment
		,dt.IsRequiredEvaluate
		,dt.[KPIWeight] AS [Weight]
		,dt.KPIDisplayOrder AS DisplayOrder
		,dt.Score
		,dt.Rate
		,dt.IsMaxScore
		,dt.Comment
		,dt.KPIGroupId
		,dt.EvaluatorId
	FROM #EPA AS tbl
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.EPAId = epa.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON tbl.EPAId = pc.EPAId
		AND pc.ProcessId = 2
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON tbl.EPAId = dt.EPAId
		AND tbl.KPIGroupId = dt.KPIGroupId
		AND dt.ProcessId = 2
		INNER JOIN dbo.KPIHistory AS khis WITH (NOLOCK) ON dt.KPIId = khis.KPIId
		AND dt.KPILogNo = khis.LogNo
	WHERE period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId
	ORDER BY epa.EPAId
		,dt.KPIDisplayOrder

	DROP TABLE #EPA

	DROP TABLE #KPIGroup

	DROP TABLE #TblKPIGroup
END

