
CREATE PROCEDURE [dbo].[EPA_GetKPI] @EPAIds IntId READONLY
AS
/*
Created: 20150827@Tim.Trinh
Task: Get KPI by list epas.
DB: SPM
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT epa.EPAId
		,dt.KPIGroupId
		,dt.KPIGroupDisplayOrder
		,dt.KPIId
		,kpihis.NAME
		,kpihis.[Description]
		,dt.HasComment
		,dt.IsRequiredComment
		,dt.IsRequiredEvaluate
		,dt.KPIDisplayOrder AS DisplayOrder
		,dt.KPIWeight AS [Weight]
	FROM @EPAIds AS epat
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.Id = epa.EPAId
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = 2 -- self
		INNER JOIN dbo.KPIHistory AS kpihis WITH (NOLOCK) ON dt.KPIId = kpihis.KPIId
		AND dt.KPILogNo = kpihis.LogNo
	ORDER BY dt.KPIGroupDisplayOrder
		,dt.KPIDisplayOrder
END