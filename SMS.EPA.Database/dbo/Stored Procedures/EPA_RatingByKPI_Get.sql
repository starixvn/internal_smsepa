
CREATE PROCEDURE [dbo].[EPA_RatingByKPI_Get] @EvaluatorId INT
	,@PeriodId INT
	,@TargetTemplateId INT
	,@KPIId INT
	,@GroupId INT = NULL
	,@EmployeeIds NVARCHAR(MAX) = NULL
AS
/*
Created: 20150909@Tim.Trinh
Task: Get KPI by Target Template.
DB: SMSEPA
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Employees IntId
		,@RequiredCommentOnMaxScore BIT
		,@RequiredCommentOnMaxScoreForEvaluator BIT
		,@Today DATE = GETDATE()
		,@EvaluatorProcessId TINYINT = 3

	CREATE TABLE #tblEPA (
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

	INSERT INTO #KPIGroup (
		TargetTemplateId
		,KPIGroupId
		,NumberKPI
		)
	SELECT DISTINCT dt.TargetTemplateId
		,dt.KPIGroupId
		,COUNT(KPIId) AS NumberKPI
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = @EvaluatorProcessId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = @EvaluatorProcessId
		AND @Today BETWEEN pc.StartDate
			AND pc.EndDate
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
	WHERE dt.EvaluatorId = @EvaluatorId
		AND pc.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId
		AND epa.HasMonthlyEPA = 0
	GROUP BY epa.EPAId
		,dt.TargetTemplateId
		,dt.KPIGroupId

	INSERT INTO #TblKPIGroup (
		TargetTemplateId
		,KPIGroupId
		,NumberKPIOptional
		)
	SELECT DISTINCT dt.TargetTemplateId
		,dt.KPIGroupId
		,COUNT(KPIId) AS NumberKPIOptional
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = @EvaluatorProcessId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = @EvaluatorProcessId
		AND @Today BETWEEN pc.StartDate
			AND pc.EndDate
	WHERE dt.EvaluatorId = @EvaluatorId
		AND pc.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId
		AND dt.IsRequiredEvaluate = 0 -- kpi optional
		AND epa.HasMonthlyEPA = 0
	GROUP BY epa.EPAId
		,dt.TargetTemplateId
		,dt.KPIGroupId

	SELECT @RequiredCommentOnMaxScore = CAST(Value AS BIT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'RequiredCommentOnMaxScore'
	
	SELECT @RequiredCommentOnMaxScoreForEvaluator = CAST(Value AS BIT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'RequiredCommentOnMaxScoreForEvaluator'

	IF @EmployeeIds IS NOT NULL
	BEGIN
		INSERT INTO @Employees (Id)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@EmployeeIds, ',')
	END

	INSERT INTO #tblEPA (
		EPAId
		,KPIGroupId
		)
	SELECT DISTINCT epa.EPAId
		,dt.KPIGroupId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = @EvaluatorProcessId
		AND @ToDay BETWEEN pc.StartDate
			AND pc.EndDate
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = @EvaluatorProcessId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.PeriodTargetSetting AS pts WITH (NOLOCK) ON period.PeriodId = pts.PeriodId
		INNER JOIN dbo.TargetTemplate AS tt WITH (NOLOCK) ON pts.TargetTemplateId = tt.TargetTemplateId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE epa.[Status] = 0 -- open
		AND epa.HasMonthlyEPA = 0
		AND ISNULL(@GroupId, gr.GroupId) = gr.GroupId
		AND (
			@EmployeeIds IS NULL
			OR epa.EmployeeId IN (
				SELECT Id
				FROM @Employees
				)
			)
		AND pc.EvaluatorId = @EvaluatorId
		AND dt.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId
		AND tt.TargetTemplateId = @TargetTemplateId
		AND dt.KPIId = @KPIId

	SELECT DISTINCT epa.EPAId
		,pc.TotalScore
		,@RequiredCommentOnMaxScore AS RequiredCommentOnMaxScore
		,@RequiredCommentOnMaxScoreForEvaluator AS RequiredCommentOnMaxScoreForEvaluator
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
		,ref.ResourceId AS RatingTypeId
		,ref.Value AS [Name]
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
	FROM #tblEPA AS tbl WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.EPAId = epa.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
		INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON epa.RatingTypeId = ref.ResourceId
		AND ref.RefName = 'RatingType'
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON tbl.EPAId = dt.EPAId
		AND tbl.KPIGroupId = dt.KPIGroupId
		AND dt.ProcessId = @EvaluatorProcessId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = @EvaluatorProcessId
		INNER JOIN dbo.KPIGroupHistory AS kghis WITH (NOLOCK) ON dt.KPIGroupId = kghis.KPIGroupId
		AND dt.KPIGroupLogNo = kghis.LogNo
		INNER JOIN #KPIGroup AS tbl1 WITH (NOLOCK) ON dt.TargetTemplateId = tbl1.TargetTemplateId
		AND dt.KPIGroupId = tbl1.KPIGroupId
		LEFT JOIN #TblKPIGroup AS tbl2 WITH (NOLOCK) ON dt.TargetTemplateId = tbl2.TargetTemplateId
		AND dt.KPIGroupId = tbl2.KPIGroupId
	WHERE dt.EvaluatorId = @EvaluatorId
		AND pc.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId
		AND epa.HasMonthlyEPA = 0
	ORDER BY emp.NickName ASC
		,emp.Code ASC

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
	FROM #tblEPA AS tbl WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.EPAId = epa.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON tbl.EPAId = pc.EPAId
		AND pc.ProcessId = @EvaluatorProcessId
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON tbl.EPAId = dt.EPAId
		AND tbl.KPIGroupId = dt.KPIGroupId
		AND dt.ProcessId = @EvaluatorProcessId
		INNER JOIN dbo.KPIHistory AS khis WITH (NOLOCK) ON dt.KPIId = khis.KPIId
		AND dt.KPILogNo = khis.LogNo
	WHERE pc.EvaluatorId = @EvaluatorId
		AND dt.EvaluatorId = @EvaluatorId
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId
		AND epa.HasMonthlyEPA = 0
	ORDER BY epa.EPAId ASC
		,dt.KPIDisplayOrder ASC

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
	FROM #tblEPA AS tbl WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.EPAId = epa.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON tbl.EPAId = pc.EPAId
		AND pc.ProcessId = 2
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON tbl.EPAId = dt.EPAId
		AND tbl.KPIGroupId = dt.KPIGroupId
		AND dt.ProcessId = 2
		INNER JOIN dbo.KPIHistory AS khis WITH (NOLOCK) ON dt.KPIId = khis.KPIId
		AND dt.KPILogNo = khis.LogNo
	WHERE dt.TargetTemplateId = @TargetTemplateId
		AND period.PeriodId = @PeriodId
		AND epa.HasMonthlyEPA = 0
	ORDER BY epa.EPAId ASC
		,dt.KPIDisplayOrder ASC

	DROP TABLE #tblEPA

	DROP TABLE #KPIGroup

	DROP TABLE #TblKPIGroup
END
