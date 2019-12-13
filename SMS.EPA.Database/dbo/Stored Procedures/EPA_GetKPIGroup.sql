
CREATE PROCEDURE [dbo].[EPA_GetKPIGroup] @EPAIds IntId READONLY
AS
/*
Created: 20150827@Tim.Trinh
Task: Get KPI Group by list id.
DB: SMSEPA
Original:
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @KPIGroup TABLE (
		EPAId INT
		,TargetTemplateId INT
		,KPIGroupId INT
		,NumberKPI INT
		)
	DECLARE @TblKPIGroup TABLE (
		EPAId INT
		,TargetTemplateId INT
		,KPIGroupId INT
		,NumberKPIOptional INT
		)

	INSERT INTO @KPIGroup (
		EPAId
		,TargetTemplateId
		,KPIGroupId
		,NumberKPI
		)
	SELECT DISTINCT epa.EPAId
		,dt.TargetTemplateId
		,dt.KPIGroupId
		,COUNT(KPIId) AS NumberKPI
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN @EPAIds AS tbl ON epa.EPAId = tbl.Id
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = 2 -- self
	GROUP BY epa.EPAId
		,dt.TargetTemplateId
		,dt.KPIGroupId

	INSERT INTO @TblKPIGroup (
		EPAId
		,TargetTemplateId
		,KPIGroupId
		,NumberKPIOptional
		)
	SELECT DISTINCT epa.EPAId
		,dt.TargetTemplateId
		,dt.KPIGroupId
		,COUNT(KPIId) AS NumberKPIOptional
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN @EPAIds AS tbl ON epa.EPAId = tbl.Id
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = 2 -- self
	WHERE dt.IsRequiredEvaluate = 0 -- kpi optional
	GROUP BY epa.EPAId
		,dt.TargetTemplateId
		,dt.KPIGroupId

	SELECT DISTINCT epa.EPAId
		,kpigh.KPIGroupId
		,kpigh.[Name]
		,dt.IsSpreadKPI
		,dt.KPIGroupWeight AS [Weight]
		,dt.KPIGroupDisplayOrder AS DisplayOrder
		,tbl1.NumberKPI
		,(CASE 
			WHEN tbl2.NumberKPIOptional > 0 THEN tbl2.NumberKPIOptional
			ELSE 0 END
			) AS NumberKPIOptional
		,(CASE 
			WHEN tbl2.NumberKPIOptional > 0 THEN 1
			ELSE 0 END
			) AS HasKPIOptional
	FROM @EPAIds AS epat
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.Id = epa.EPAId
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = 2 -- self
		INNER JOIN dbo.KPIGroupHistory AS kpigh WITH (NOLOCK) ON dt.KPIGroupId = kpigh.KPIGroupId
		AND dt.KPIGroupLogNo = kpigh.LogNo
		INNER JOIN @KPIGroup AS tbl1 ON epa.EPAId = tbl1.EPAId
		AND dt.TargetTemplateId = tbl1.TargetTemplateId
		AND dt.KPIGroupId = tbl1.KPIGroupId
		LEFT JOIN @TblKPIGroup AS tbl2 ON epa.EPAId = tbl2.EPAId
		AND dt.TargetTemplateId = tbl2.TargetTemplateId
		AND dt.KPIGroupId = tbl2.KPIGroupId
	ORDER BY dt.KPIGroupDisplayOrder ASC
END
