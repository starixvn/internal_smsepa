
CREATE PROCEDURE [dbo].[EpaAuto_CheckEndEmployeeEvaluationPeriod]
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Check end evaluation period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Yesterday DATE = DATEADD(DAY, - 1, GETDATE())

	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
	WHERE EndDate = @Yesterday
		AND ProcessId IN (
			SELECT MAX(pc1.ProcessId)
			FROM dbo.EPAProcess AS pc1 WITH (NOLOCK)
			WHERE pc1.EPAId = epa.EPAId
			)
		AND epa.[Status] = 0
END