
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
		,@Yesterday DATE = DATEADD(DAY, - 1, GETDATE())
		,@EvaluatorProcessId TINYINT = 3

	SELECT DISTINCT epa.EPAId
		,period.PeriodId
		,period.TargetTypeId
		,pc.EvaluatorId
		,pc.ProcessId
		,pc.TotalScore
		,pc.AdjustScore
		,(CASE 
			WHEN period.TargetTypeId = 3 --semi
				AND epa.HasMonthlyEPA = 1 THEN 1
			ELSE 0 END
			) AS HasMonthlyEPA
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId > 1
	WHERE epa.[Status] = 0
		AND EXISTS (
			SELECT 1
			FROM dbo.EPAProcess AS pc1 WITH (NOLOCK)
			WHERE epa.EPAId = pc1.EPAId
				AND pc1.EndDate = @Yesterday
			)
	ORDER BY epa.EPAId
		,pc.ProcessId
END