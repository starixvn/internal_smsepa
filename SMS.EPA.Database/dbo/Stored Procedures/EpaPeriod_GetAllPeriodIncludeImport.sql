
CREATE PROCEDURE [dbo].[EpaPeriod_GetAllPeriodIncludeImport]
AS
/*
Created: 2015-10-19@Josh.Phan
Task: Get all period, inlcude data import.
DB: SMSEPA
Original: 

Revisions:  
20160524@Razor.Nguyen: Exclude Monthly EPA
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT pe.[PeriodId]
		,pe.[Name]
		,pe.StartDate
	FROM (
		SELECT [PeriodId]
			,[Name]
			,[StartDate]
		FROM [dbo].[Period] WITH (NOLOCK)
		WHERE [IsActive] = 1
			AND [TargetTypeId] != 2
		
		UNION
		SELECT [PeriodId]
			,[PeriodName] AS NAME
			,[PeriodDate] AS StartDate
		FROM [dbo].[EPASummaryImport] WITH (NOLOCK)
		) AS pe
	ORDER BY pe.[StartDate] DESC
END