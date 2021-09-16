
CREATE PROCEDURE [dbo].[EpaAuto_CheckLastestEndEvaluationPeriod] @PeriodId INT
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Check lastest end process of period.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Yesterday DATE = DATEADD(DAY, - 1, GETDATE())
		,@EndDate DATE

	SELECT @EndDate = MAX(pc.EndDate)
	FROM dbo.EPAProcess AS pc WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON pc.EPAId = epa.EPAId
	WHERE epa.PeriodId = @PeriodId
		AND ISNULL(epa.[Status], 0) <> 2 -- rejected

	IF @EndDate > @Yesterday
	BEGIN
		SELECT 0
	END
	ELSE
	BEGIN
		SELECT 1
	END
END