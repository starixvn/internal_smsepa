
CREATE PROCEDURE [dbo].[EpaAuto_GetPeriodHasReminderProcess]
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Get period and process has reminder email.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
	-- target type
	DECLARE @ProbationId INT = 1
		,@MonthlyId INT = 2
		,@SemiId INT = 3
		,@PromotionId INT = 5

	SELECT DISTINCT pe.PeriodId
		,epaP.ProcessId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS epaP WITH (NOLOCK) ON epa.EPAId = epaP.EPAId
		INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON epa.PeriodId = pe.PeriodId
	WHERE CAST(epaP.RemindDate AS DATE) = @Today
		AND ISNULL(epa.[Status], 0) = 0 -- Open
		AND pe.TargetTypeId IN (@ProbationId, @PromotionId)
		AND pe.IsActive = 1
		--AND ISNULL(pe.EndDate, '') = '' --Probation don't have end date
		--OR pe.EndDate >= GETDATE() --EndDate is still in the future
	
	UNION
	SELECT DISTINCT period.PeriodId
		,pc.ProcessId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.PeriodProcessSetting AS periodPS WITH (NOLOCK) ON period.PeriodId = periodPS.PeriodId
		AND pc.ProcessId = periodPS.ProcessId
	WHERE ISNULL(epa.[Status], 0) = 0 -- Open
		AND period.TargetTypeId IN (@MonthlyId, @SemiId)
		AND period.IsActive = 1
		AND CAST(DATEADD(DAY, - periodPS.RemindDay, pc.EndDate) AS DATE) = @Today
END