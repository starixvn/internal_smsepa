
CREATE PROCEDURE [dbo].[TargetTemplate_GetKPIGroup] @TargetTemplateId INT
AS
/*
Created: 20140426@Quinn.Vo
Task: Get all kpi group in target template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TargetTypeId INT
	DECLARE @KPIGroup TABLE (
		KPIGroupId INT
		,HasKPIOptional BIT
		)

	SELECT @TargetTypeId = TargetTypeId
	FROM dbo.TargetTemplate WITH (NOLOCK)
	WHERE TargetTemplateId = @TargetTemplateId

	INSERT INTO @KPIGroup (
		KPIGroupId
		,HasKPIOptional
		)
	SELECT tbl1.KPIGroupId
		,(CASE 
			WHEN tbl2.NumberKPI IS NULL THEN 0
			WHEN tbl1.NumberKPI <> tbl2.NumberKPI THEN 1 END
			) AS HasKPIOptional
	FROM (
		SELECT DISTINCT ttkg.KPIGroupId
			,COUNT(ttk.KPIId) AS NumberKPI
		FROM dbo.TargetTemplateKPIGroup AS ttkg WITH (NOLOCK)
			INNER JOIN dbo.TargetTemplateKPI AS ttk WITH (NOLOCK) ON ttkg.TargetTemplateId = ttk.TargetTemplateId
			AND ttkg.KPIGroupId = ttk.KPIGroupId
			AND ttkg.TargetTemplateId = @TargetTemplateId
		GROUP BY ttkg.TargetTemplateId
			,ttkg.KPIGroupId
		) AS tbl1
		LEFT JOIN (
		SELECT DISTINCT ttkg.KPIGroupId
			,COUNT(ttk.KPIId) AS NumberKPI
		FROM dbo.TargetTemplateKPIGroup AS ttkg WITH (NOLOCK)
			INNER JOIN dbo.TargetTemplateKPI AS ttk WITH (NOLOCK) ON ttkg.TargetTemplateId = ttk.TargetTemplateId
			AND ttkg.KPIGroupId = ttk.KPIGroupId
			AND ttkg.TargetTemplateId = @TargetTemplateId
			INNER JOIN dbo.KPI AS kpi WITH (NOLOCK) ON ttk.KPIId = kpi.KPIId
			AND kpi.IsRequiredEvaluate = 0
		GROUP BY ttkg.TargetTemplateId
			,ttkg.KPIGroupId
		) AS tbl2 ON tbl1.KPIGroupId = tbl2.KPIGroupId

	SELECT kg.KPIGroupId
		,kg.[Name]
		,kg.[Description]
		,tg.[Weight]
		,tg.IsSpreadKPI
		,tg.HasKPIOptional
	FROM dbo.KPIGroup AS kg WITH (NOLOCK)
		LEFT JOIN (
		SELECT ttkg.KPIGroupId
			,[Weight]
			,IsSpreadKPI
			,DisplayOrder
			,kg1.HasKPIOptional
		FROM dbo.TargetTemplateKPIGroup AS ttkg WITH (NOLOCK)
		LEFT JOIN @KPIGroup AS kg1 ON ttkg.KPIGroupId = kg1.KPIGroupId
		WHERE TargetTemplateId = @TargetTemplateId
		) AS tg ON kg.KPIGroupId = tg.KPIGroupId
	WHERE tg.KPIGroupId IS NOT NULL
		OR kg.IsActive = 1
	ORDER BY ISNULL(tg.DisplayOrder, 999) ASC
		,kg.DisplayOrder ASC
END