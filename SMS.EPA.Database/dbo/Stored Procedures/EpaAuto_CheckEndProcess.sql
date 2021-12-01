
CREATE PROCEDURE [dbo].[EpaAuto_CheckEndProcess]
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
	
	DECLARE @Today DATE = GETDATE()

	SELECT DISTINCT epa.EPAId
		,period.PeriodId
		,period.TargetTypeId
		,pc.EvaluatorId
		,pc.ProcessId
		,pc.TotalScore
		,(CASE 
			WHEN period.TargetTypeId = 3 --semi
				AND epa.HasMonthlyEPA = 1 THEN 1
			ELSE 0 END
			) AS HasMonthlyEPA
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND ISNULL(epa.[Status], 0) = 0
	WHERE pc.EndDate < @Today
		AND pc.IsEndProcess = 0
END