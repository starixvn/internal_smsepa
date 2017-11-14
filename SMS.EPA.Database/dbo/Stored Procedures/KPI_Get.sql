
CREATE PROCEDURE [dbo].[KPI_Get] @KPIGroupId INT = NULL
	,@IsActive BIT = NULL
	,@IsRequiredEvaluate BIT = NULL
AS
/*
Created: 20150707@Tim.Trinh
Task: Get all KPI.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblKPI (KPIId INT)

	INSERT INTO #tblKPI (KPIId)
	SELECT DISTINCT k.KPIId
	FROM dbo.KPI AS k WITH (NOLOCK)
		INNER JOIN dbo.KPIDetail AS kg WITH (NOLOCK) ON k.KPIId = kg.KPIId
		INNER JOIN dbo.KPIGroup AS g WITH (NOLOCK) ON kg.KPIGroupId = g.KPIGroupId
	WHERE ISNULL(@IsActive, k.IsActive) = k.IsActive
		AND ISNULL(@IsRequiredEvaluate, k.IsRequiredEvaluate) = k.IsRequiredEvaluate
		AND ISNULL(@KPIGroupId, g.KPIGroupId) = g.KPIGroupId

	SELECT DISTINCT k.*
	FROM dbo.KPI AS k WITH (NOLOCK)
		INNER JOIN #tblKPI AS tmp WITH (NOLOCK) ON k.KPIId = tmp.KPIId

	SELECT k.KPIId
		,g.KPIGroupId
		,g.[Name]
	FROM #tblKPI AS k WITH (NOLOCK)
		INNER JOIN dbo.KPIDetail AS kg WITH (NOLOCK) ON k.KPIId = kg.KPIId
		INNER JOIN dbo.KPIGroup AS g WITH (NOLOCK) ON kg.KPIGroupId = g.KPIGroupId;

	DROP TABLE #tblKPI
END