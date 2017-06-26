
CREATE PROCEDURE [dbo].[EpaAuto_GetPeriodHasStartedProcess] @PeriodId INT = NULL
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Get period has employee that started evaluation.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
	20170419@Hunter.Tran: Check if period is active
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT pe.PeriodId
		,pc.ProcessId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON epa.PeriodId = pe.PeriodId
	WHERE CAST(pc.StartDate AS DATE) = CAST(GETDATE() AS DATE)
		AND (
			ISNULL(@PeriodId, '') = ''
			OR pe.PeriodId = @PeriodId
			)
		AND ISNULL(epa.[Status], 0) = 0 -- Open
		AND pe.IsActive = 1
END