
CREATE PROCEDURE [dbo].[EPA_GetPeriodToUpdateFinalScore]
AS
/*
Created: 20160601@Tim.Trinh
Task: Get period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT PeriodId
		,[Name]
	FROM dbo.Period WITH (NOLOCK)
	WHERE TargetTypeId = 1 -- probation
		AND IsActive = 1
	
	UNION
	SELECT PeriodId
		,[Name]
	FROM dbo.Period WITH (NOLOCK)
	WHERE TargetTypeId = 5 -- promotion
		AND IsActive = 1
		AND StartDate = (
			SELECT MAX(StartDate)
			FROM dbo.Period
			WHERE TargetTypeId = 5
				AND IsActive = 1
			)
	
	UNION
	SELECT PeriodId
		,[Name]
	FROM dbo.Period WITH (NOLOCK)
	WHERE TargetTypeId = 3 -- semi annual
		AND IsActive = 1
		AND StartDate = (
			SELECT MAX(StartDate)
			FROM dbo.Period
			WHERE TargetTypeId = 3
				AND IsActive = 1
			)
END