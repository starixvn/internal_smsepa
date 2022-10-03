
CREATE PROCEDURE [dbo].[JobLevelStandard_Report_GetPeriod]
AS
/*
Created: 20220901@Tim.Trinh
Task: get period.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	SELECT p.PeriodId
		,p.[Name]
	FROM dbo.JobLevelStandard_Period AS p WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodSetting AS ps WITH (NOLOCK) ON p.PeriodId = ps.PeriodId
		AND ps.ProcessId = 1
	WHERE p.IsActive = 1
		AND ps.FromDate < @Today
	ORDER BY ps.FromDate DESC
END