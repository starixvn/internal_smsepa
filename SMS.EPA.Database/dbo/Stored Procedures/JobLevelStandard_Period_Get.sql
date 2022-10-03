
CREATE PROCEDURE [dbo].[JobLevelStandard_Period_Get] @IsActive BIT = 1
AS
/*
Created: 2022-08-10@Tim.Trinh
Task: get period
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT p.PeriodId
		,p.[Name]
		,p.IsActive
	FROM dbo.JobLevelStandard_Period AS p WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodSetting AS ps WITH (NOLOCK) ON p.PeriodId = ps.PeriodId
		AND ps.ProcessId = 1
	ORDER BY ps.FromDate DESC

	SELECT PeriodId
		,ProcessId
		,FromDate
		,ToDate
	FROM dbo.JobLevelStandard_PeriodSetting WITH (NOLOCK)
	ORDER BY PeriodId
		,ProcessId
END