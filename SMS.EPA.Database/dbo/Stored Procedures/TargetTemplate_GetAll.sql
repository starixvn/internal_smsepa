
CREATE PROCEDURE [dbo].[TargetTemplate_GetAll] @TargetTypeId INT = NULL
	,@IsActive TINYINT = NULL
AS
/*
Created: 2014004@Quinn.Vo
Task: Get all targets template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT tg.TargetTemplateId
		,tg.[Name]
		,tg.[Description]
		,tg.[IsActive]
		,tg.[TargetTypeId]
		,tgTy.[Name] AS TargetTypeName
		,tg.[RatingTypeId]
		,ref.Value AS RatingTypeName
		,scT.[ScoreRatingTypeId]
		,scT.[Name] AS ScoreRatingTypeName
		,ISNULL(tk.KPIs, 0) AS KPIs
	FROM dbo.TargetTemplate AS tg WITH (NOLOCK)
		LEFT JOIN (
		SELECT TargetTemplateId
			,COUNT(1) AS KPIs
		FROM dbo.TargetTemplateKPI WITH (NOLOCK)
		GROUP BY TargetTemplateId
		) AS tk ON tg.TargetTemplateId = tk.TargetTemplateId
		INNER JOIN [dbo].[TargetType] AS tgTy WITH (NOLOCK) ON tg.[TargetTypeId] = tgTy.[TargetTypeId]
		INNER JOIN [dbo].[ScoreRatingType] AS scT WITH (NOLOCK) ON tg.[ScoreRatingTypeId] = scT.[ScoreRatingTypeId]
		INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON tg.RatingTypeId = ref.ResourceId
		AND ref.RefName = 'RatingType'
	WHERE ISNULL(@TargetTypeId, tg.TargetTypeId) = tg.TargetTypeId
		AND ISNULL(@IsActive, tg.[IsActive]) = tg.[IsActive]
	ORDER BY tg.[Name]
END