/****** Object:  Stored Procedure dbo.EpaScoreRatingType_Get    Script Date: 5/16/2016 2:20:09 PM ******/
/****** Object:  Stored Procedure dbo.EpaScoreRatingType_Get    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.EpaScoreRatingType_Get    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaScoreRatingType_Get    Script Date: 4/6/2016 11:27:46 AM ******/

CREATE PROCEDURE [dbo].[EpaScoreRatingType_Get] @ScoreRatingTypeId INT = NULL
	,@IsActive BIT = 1
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get all score rating type.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT [ScoreRatingTypeId]
		,[Name]
		,[IsActive]
		,[Description]
		,[SelfEvaluated]
		,[Evaluator]
		,[Reviewer]
		,[Management]
		,[Attendance]
	FROM [dbo].[ScoreRatingType] WITH (NOLOCK)
	WHERE [IsActive] = @IsActive
		AND (
			ISNULL(@ScoreRatingTypeId, '') = ''
			OR [ScoreRatingTypeId] = @ScoreRatingTypeId
			)
END