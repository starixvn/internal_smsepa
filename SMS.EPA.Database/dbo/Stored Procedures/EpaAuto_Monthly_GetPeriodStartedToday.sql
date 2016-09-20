/****** Object:  Stored Procedure dbo.EpaAuto_Monthly_GetPeriodStartedToday    Script Date: 7/21/2016 3:01:36 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_Monthly_GetPeriodStartedToday    Script Date: 7/21/2016 2:45:48 PM ******/

CREATE PROCEDURE [dbo].[EpaAuto_Monthly_GetPeriodStartedToday]
AS
/*
Created: 20160223@Tim.Trinh
Task: Check has start date is today.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT [PeriodId]
	FROM [dbo].[Period] WITH (NOLOCK)
	WHERE CAST(GETDATE() AS DATE) = CAST([StartDate] AS DATE)
		AND TargetTypeId = 2 -- monthly
		AND [IsActive] = 1
END