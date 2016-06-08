
CREATE PROCEDURE [dbo].[EpaAuto_CheckTotalScoreIsNull] @EPAId INT
	,@ProcessId INT
AS
/*
Created: 2015-10-23@Josh.Phan
Task: Check if the total score of current process is null.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:	
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsTotalScoreNull BIT

	SET @IsTotalScoreNull = 0

	IF EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[EPAProcess] WITH (NOLOCK)
			WHERE [EPAId] = @EPAId
				AND [ProcessId] = @ProcessId
				AND ISNULL([TotalScore], '') = ''
			)
	BEGIN
		SET @IsTotalScoreNull = 1
	END

	SELECT @IsTotalScoreNull
END
