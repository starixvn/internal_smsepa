/****** Object:  Stored Procedure dbo.EpaAuto_SemiAnnual_GetPeriodStartedToday    Script Date: 7/21/2016 3:01:36 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_SemiAnnual_GetPeriodStartedToday    Script Date: 7/21/2016 2:45:49 PM ******/

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