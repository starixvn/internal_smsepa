
CREATE PROCEDURE [dbo].[EpaMonthlyPeriod_GetAll]
AS
/*
Created: 20160520@Razor.Nguyen
Task: Get all monthly period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	SELECT DISTINCT pe.PeriodId
		,pe.[Name]
		,pe.StartDate
		,pe.EndDate
		,pe.[Description]
		,pe.IsActive
	FROM dbo.Period AS pe WITH (NOLOCK)
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON pe.TargetTypeId = tt.TargetTypeId
	WHERE pe.TargetTypeId = 2 -- Monhtly
	ORDER BY pe.StartDate DESC
END