/****** Object:  Stored Procedure dbo.EpaAuto_CheckEndProcess    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_CheckEndProcess    Script Date: 5/16/2016 2:15:33 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_CheckEndProcess    Script Date: 5/16/2016 1:59:44 PM ******/

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

	SELECT DISTINCT epa.EPAId
		,period.PeriodId
		,period.TargetTypeId
		,pc.ProcessId
		,pc.TotalScore
		,CASE 
			WHEN period.TargetTypeId = 3 --semi
				AND epa.HasMonthlyEPA = 1 THEN 1
			ELSE 0
			END AS HasMonthlyEPA
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND epa.[Status] = 0 -- open
	WHERE CAST(pc.EndDate AS DATE) < CAST(GETDATE() AS DATE)
		AND pc.IsEndProcess = 0
END