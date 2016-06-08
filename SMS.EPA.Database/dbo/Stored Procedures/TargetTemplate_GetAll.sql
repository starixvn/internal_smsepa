/****** Object:  Stored Procedure dbo.TargetTemplate_GetAll    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_GetAll    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_GetAll    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_GetAll    Script Date: 4/6/2016 11:27:49 AM ******/

CREATE PROCEDURE [dbo].[TargetTemplate_GetAll] @TargetTypeId INT = NULL
	,@IsActive TINYINT = NULL
AS
/*
Created: 2014004@Quinn.Vo
Task: Get all targets template.
DB: NexEPA
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
		,lk.[Name] AS RatingTypeName
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
		INNER JOIN [dbo].[LookupData] AS lk WITH (NOLOCK) ON tg.[RatingTypeId] = CAST(lk.Value AS INT)
		AND lk.[LookupId] = 8 -- Rating Type
	WHERE (
			@TargetTypeId IS NULL
			OR tg.TargetTypeId = @TargetTypeId
			)
		AND (
			@IsActive IS NULL
			OR tg.[IsActive] = @IsActive
			)
END