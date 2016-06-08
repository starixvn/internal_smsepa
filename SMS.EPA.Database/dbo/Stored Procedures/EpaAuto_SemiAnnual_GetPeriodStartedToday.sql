/****** Object:  Stored Procedure dbo.EpaAuto_SemiAnnual_GetPeriodStartedToday    Script Date: 5/16/2016 2:20:07 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_SemiAnnual_GetPeriodStartedToday    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_SemiAnnual_GetPeriodStartedToday    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_SemiAnnual_GetPeriodStartedToday    Script Date: 4/6/2016 11:27:43 AM ******/

CREATE PROCEDURE [dbo].[EpaAuto_SemiAnnual_GetPeriodStartedToday]
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Check  has start date is today.
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
		AND TargetTypeId = 3 -- semi annual
		AND [IsActive] = 1
END