/****** Object:  Stored Procedure dbo.EpaAuto_UpdateTotalScoreWhenNull    Script Date: 5/16/2016 2:20:07 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_UpdateTotalScoreWhenNull    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_UpdateTotalScoreWhenNull    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_UpdateTotalScoreWhenNull    Script Date: 4/6/2016 11:27:44 AM ******/

CREATE PROCEDURE [dbo].[EpaAuto_UpdateTotalScoreWhenNull] @EPAId INT
	,@ProcessId INT
	,@AverageScore NVARCHAR(100) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-10-23@Josh.Phan
Task: Update total score of current process by average score of pre-process.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:	
*/
BEGIN
	SET NOCOUNT ON;

	UPDATE epaP
	SET epaP.[TotalScore] = @AverageScore
	FROM [dbo].[EPAProcess] AS epaP WITH (NOLOCK)
	WHERE epaP.[EPAId] = @EPAId
		AND epaP.[ProcessId] = @ProcessId
		AND ISNULL(epaP.[TotalScore], '') = ''
END