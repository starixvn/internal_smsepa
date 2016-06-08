/****** Object:  Stored Procedure dbo.TargetTemplate_GetById    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_GetById    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_GetById    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_GetById    Script Date: 4/6/2016 11:27:49 AM ******/

CREATE PROCEDURE [dbo].[TargetTemplate_GetById] --53
	@TargetTemplateId INT
AS
/*
Created: 2014004@Quinn.Vo
Task: Get target by id.
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
		,tt.NAME AS TargetTypeName
		,tg.[RatingTypeId]
		,lk.[Name] AS RatingTypeName
		,sco.[ScoreRatingTypeId]
		,sco.[Name] AS ScoreRatingTypeName
		,sco.[SelfEvaluated] AS MaxWeightSelf
		,tg.CreatedBy
		,e.NickName AS CreatedNickName
		,e.FullName AS CreatedFullName
		,tg.CreatedDate
		,tg.ModifiedBy
		,ee.NickName AS ModifiedNickName
		,ee.FullName AS ModifiedFullName
		,tg.ModifiedDate
	FROM dbo.TargetTemplate AS tg WITH (NOLOCK)
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON tg.TargetTypeId = tt.TargetTypeId
		INNER JOIN dbo.[ScoreRatingType] AS sco WITH (NOLOCK) ON tg.[ScoreRatingTypeId] = sco.[ScoreRatingTypeId]
		INNER JOIN [dbo].[LookupData] AS lk WITH (NOLOCK) ON tg.[RatingTypeId] = CAST(lk.Value AS INT)
		AND lk.[LookupId] = 8 -- Rating Type
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON tg.CreatedBy = e.EmployeeId
		LEFT JOIN dbo.Employee AS ee WITH (NOLOCK) ON tg.ModifiedBy = ee.EmployeeId
	WHERE tg.TargetTemplateId = @TargetTemplateId
END