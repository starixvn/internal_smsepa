
CREATE PROCEDURE [dbo].[JobLevelStandard_Assessment_GetPeriodByActorId] @ActorId INT
AS
/*
Created: 20220816@Tim.Trinh
Task: get period by actor.
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
		AND EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK)
				INNER JOIN dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK) ON pes.PeriodId = pps.PeriodId
				AND pes.EmployeeId = pps.EmployeeId
				AND pps.EvaluatorId = @ActorId
				AND @Today BETWEEN pps.FromDate
					AND pps.ToDate
			WHERE pes.IsExcluded = 0
			)
	ORDER BY ps.FromDate DESC
END