
CREATE PROCEDURE [dbo].[JobLevelStandard_Period_GetById] @PeriodId INT
AS
/*
Created: 2022-08-11@Tim.Trinh
Task: get period by id
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT PeriodId
		,[Name]
		,IsActive
	FROM dbo.JobLevelStandard_Period  WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	SELECT PeriodId
		,ProcessId
		,FromDate
		,ToDate
	FROM dbo.JobLevelStandard_PeriodSetting WITH (NOLOCK)
	WHERE PeriodId = @PeriodId
	ORDER BY PeriodId
		,ProcessId
END