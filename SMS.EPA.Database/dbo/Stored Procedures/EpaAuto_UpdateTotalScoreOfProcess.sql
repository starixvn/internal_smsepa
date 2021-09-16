
CREATE PROCEDURE [dbo].[EpaAuto_UpdateTotalScoreOfProcess] @EPAId INT
	,@ProcessId INT
	,@TotalScore NVARCHAR(100) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2021-09-10@Tim.Trinh
Task: Update total score.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:	
*/
BEGIN
	SET NOCOUNT ON;

	UPDATE pc
	SET pc.TotalScore = @TotalScore
	FROM dbo.EPAProcess AS pc WITH (ROWLOCK, UPDLOCK)
	WHERE pc.EPAId = @EPAId
		AND pc.ProcessId = @ProcessId
END