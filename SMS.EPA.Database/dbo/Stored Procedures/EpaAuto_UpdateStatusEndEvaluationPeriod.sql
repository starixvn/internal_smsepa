
CREATE PROCEDURE [dbo].[EpaAuto_UpdateStatusEndEvaluationPeriod] @EPAIds VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Check end process.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF @EPAIds IS NOT NULL
	BEGIN
		UPDATE dbo.EPA WITH (ROWLOCK, UPDLOCK)
		SET [Status] = 1 -- Closed
		WHERE EPAId IN (
				SELECT DISTINCT val
				FROM dbo.fnc_strSplit(@EPAIds, ',')
				)
	END
END