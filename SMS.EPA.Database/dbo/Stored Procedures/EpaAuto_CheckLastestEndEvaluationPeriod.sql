
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

	IF EXISTS (
			SELECT 1
			FROM [dbo].[EPAProcess] AS epaP WITH (NOLOCK)
				INNER JOIN [dbo].[EPA] AS epa WITH (NOLOCK) ON epaP.[EPAId] = epa.[EPAId]
				AND epa.[PeriodId] = @PeriodId
			WHERE epaP.[EndDate] > @Yesterday
				AND [ProcessId] IN (
					SELECT MAX([ProcessId])
					FROM [dbo].[EPAProcess] WITH (NOLOCK)
					WHERE [EPAId] = epa.[EPAId]
					)
				AND ISNULL(epa.[Status], 0) <> 2 -- rejected
			)
	BEGIN
		SELECT 0
	END
	ELSE
	BEGIN
		SELECT 1
	END
END