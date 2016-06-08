/****** Object:  Stored Procedure dbo.EpaAuto_Monthly_GetPeriodStartedToday    Script Date: 5/16/2016 2:20:11 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_Monthly_GetPeriodStartedToday    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_Monthly_GetPeriodStartedToday    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_Monthly_GetPeriodStartedToday    Script Date: 4/6/2016 11:27:43 AM ******/

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