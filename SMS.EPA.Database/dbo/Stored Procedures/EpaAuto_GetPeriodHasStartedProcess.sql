/****** Object:  Stored Procedure dbo.EpaAuto_GetPeriodHasStartedProcess    Script Date: 7/21/2016 3:01:36 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_GetPeriodHasStartedProcess    Script Date: 7/21/2016 2:45:48 PM ******/

CREATE PROCEDURE dbo.EpaAuto_GetPeriodHasStartedProcess @PeriodId INT = NULL
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Get period has employee that started evaluation.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT pe.PeriodId
		,pc.ProcessId
		,pe.TargetTypeId
		,CASE 
			WHEN pe.TargetTypeId = 3 -- semi
				AND epa.HasMonthlyEPA = 1 THEN 1
			ELSE 0
			END AS HasMonthlyEPA
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON epa.PeriodId = pe.PeriodId
	WHERE CAST(pc.StartDate AS DATE) = CAST(GETDATE() AS DATE)
		AND (
			ISNULL(@PeriodId, '') = ''
			OR pe.PeriodId = @PeriodId
			)
		AND epa.[Status] = 0 -- Open
END