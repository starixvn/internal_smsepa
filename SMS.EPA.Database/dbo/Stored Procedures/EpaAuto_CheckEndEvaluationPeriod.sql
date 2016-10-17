
CREATE PROCEDURE [dbo].[EpaAuto_CheckEndEvaluationPeriod]
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

	DECLARE @Yesterday DATETIME

	SET @Yesterday = DATEADD(DAY, - 1, GETDATE())

	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS epaP WITH (NOLOCK) ON epa.EPAId = epaP.EPAId
		INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON epa.PeriodId = pe.PeriodId
	WHERE CAST(pe.EndDate AS DATE) = CAST(GETDATE() AS DATE)
		AND CAST(epaP.EndDate AS DATE) < CAST(pe.EndDate AS DATE)
		AND ProcessId IN (
			SELECT MAX(ProcessId)
			FROM dbo.EPAProcess WITH (NOLOCK)
			WHERE EPAId = epaP.EPAId
			)
		AND pe.TargetTypeId IN (2, 3) -- Monthly, Semi
		AND epa.[Status] <> 2
		AND pe.IsActive = 1
	
	UNION
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPAProcess AS epaP WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epa.EPAId = epaP.EPAId
		INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
	WHERE CAST(epaP.EndDate AS DATE) = CAST(@Yesterday AS DATE)
		AND CAST(epaP.EndDate AS DATE) >= CAST(pe.EndDate AS DATE)
		AND ProcessId IN (
			SELECT MAX(ProcessId)
			FROM dbo.EPAProcess WITH (NOLOCK)
			WHERE EPAId = epaP.EPAId
			)
		AND pe.TargetTypeId IN (2, 3) -- Monthly, Semi
		AND epa.[Status] <> 2
		AND pe.IsActive = 1

	UNION
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPAProcess AS epaP WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epaP.EPAId = epa.EPAId
		INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
	WHERE CAST(epaP.EndDate AS DATE) = CAST(@Yesterday AS DATE)
		AND ProcessId IN (
			SELECT MAX(ProcessId)
			FROM dbo.EPAProcess WITH (NOLOCK)
			WHERE EPAId = epa.EPAId
			)
		AND pe.TargetTypeId NOT IN (2, 3) -- Monthly, Semi
		AND epa.[Status] <> 2
		AND pe.IsActive = 1

	UNION
	SELECT EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
	WHERE epa.[Status] = 1
		AND ISNULL(epa.FinalScore, '') = ''
		AND CAST(pe.EndDate AS DATE) < CAST(GETDATE() AS DATE)
		AND pe.IsActive = 1
END