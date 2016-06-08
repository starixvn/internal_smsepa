/****** Object:  Stored Procedure dbo.EpaAuto_GetPeriodHasReminderProcess    Script Date: 5/16/2016 2:20:07 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_GetPeriodHasReminderProcess    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_GetPeriodHasReminderProcess    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_GetPeriodHasReminderProcess    Script Date: 4/6/2016 11:27:43 AM ******/

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

	SELECT DISTINCT pe.PeriodId
		,epaP.ProcessId
		,pe.TargetTypeId
		,0 AS HasMonthlyEPA
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS epaP WITH (NOLOCK) ON epa.EPAId = epaP.EPAId
		INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON epa.PeriodId = pe.PeriodId
	WHERE CAST(epaP.RemindDate AS DATE) = CAST(GETDATE() AS DATE)
		AND epa.[Status] = 0 -- Open
		AND pe.TargetTypeId IN (1, 5) -- Probation or Promotion
	
	UNION
	SELECT DISTINCT period.PeriodId
		,pc.ProcessId
		,period.TargetTypeId
		,(CASE 
			WHEN period.TargetTypeId = 3 -- semi
				AND epa.HasMonthlyEPA = 1 THEN 1
			ELSE 0 END
			) AS HasMonthlyEPA
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.PeriodProcessSetting AS periodPS WITH (NOLOCK) ON period.PeriodId = periodPS.PeriodId
		AND pc.ProcessId = periodPS.ProcessId
	WHERE epa.[Status] = 0 -- Open
		AND period.TargetTypeId IN (2, 3) -- Monthly, Semi annual
		AND CAST(DATEADD(DAY, - periodPS.RemindDay, pc.EndDate) AS DATE) = CAST(GETDATE() AS DATE)
END