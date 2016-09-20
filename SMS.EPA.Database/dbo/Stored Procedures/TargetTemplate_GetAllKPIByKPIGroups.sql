
CREATE PROCEDURE [dbo].[TargetTemplate_GetAllKPIByKPIGroups] --1
	@TargetTemplateId INT
AS
/*
Created: 20140426@Quinn.Vo
Task: Get all KPIs by all group in a target template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT k.KPIId
		,k.[Name]
		,k.[Description]
		,(CASE 
			WHEN tk.HasComment IS NULL THEN k.HasComment
			ELSE tk.HasComment END
			) AS HasComment
		,(CASE 
			WHEN tk.IsRequiredComment IS NULL THEN k.IsRequiredComment
			ELSE tk.IsRequiredComment END
			) AS IsRequiredComment
		,(CASE 
			WHEN tk.IsRequiredEvaluate IS NULL THEN k.IsRequiredEvaluate
			ELSE tk.IsRequiredEvaluate END
			) AS IsRequiredEvaluate
		,k.IsActive
		,tk.[Weight]
		,g.KPIGroupId
		,g.[Name]
		,tg.[Weight]
		,tg.IsSpreadKPI
	FROM dbo.TargetTemplateKPIGroup AS tg WITH (NOLOCK)
		INNER JOIN dbo.KPIDetail AS kd WITH (NOLOCK) ON tg.KPIGroupId = kd.KPIGroupId
		INNER JOIN dbo.KPI AS k WITH (NOLOCK) ON kd.KPIId = k.KPIId
		INNER JOIN dbo.KPIGroup AS g WITH (NOLOCK) ON g.KPIGroupId = kd.KPIGroupId
		LEFT JOIN dbo.TargetTemplateKPI AS tk WITH (NOLOCK) ON kd.KPIId = tk.KPIId
		AND tg.KPIGroupId = tk.KPIGroupId
		AND tg.TargetTemplateId = tk.TargetTemplateId
	WHERE tg.TargetTemplateId = @TargetTemplateId
		AND (
			k.IsActive = 1
			OR (
				k.IsActive = 0
				AND tk.KPIId = k.KPIId
				)
			)
	ORDER BY tg.DisplayOrder
		,ISNULL(tk.DisplayOrder, 999)
END