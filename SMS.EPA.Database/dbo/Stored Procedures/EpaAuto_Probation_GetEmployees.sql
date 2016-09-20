/****** Object:  Stored Procedure dbo.EpaAuto_Probation_GetEmployees    Script Date: 7/21/2016 3:01:36 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_Probation_GetEmployees    Script Date: 7/21/2016 2:45:49 PM ******/

CREATE PROCEDURE [dbo].[EpaAuto_Probation_GetEmployees]
AS
/*
Created: 2015-09-01@Josh.Phan
Task: Get employee that has probation.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ProbDay VARCHAR(50)

	SET @ProbDay = (
			SELECT Value
			FROM [dbo].[LookupData]
			WHERE [Id] = 7 -- Probation Setting Day
			)

	SELECT [EmployeeId]
	FROM [dbo].[Employee] WITH (NOLOCK)
	WHERE CAST(DATEADD(DAY, CAST(@ProbDay AS INT), [JoinedDate]) AS DATE) = CAST(GETDATE() AS DATE)
		AND [IsDeleted] = 0
		AND [Enabled] = 1

	SELECT peM.[PeriodId]
	FROM [dbo].[Period] AS pe WITH (NOLOCK)
		INNER JOIN (
		SELECT MAX([PeriodId]) AS PeriodId
		FROM [dbo].[Period] WITH (NOLOCK)
		WHERE [TargetTypeId] = 1 -- Probation
		GROUP BY [PeriodId]
		) AS peM ON pe.[PeriodId] = peM.[PeriodId]
END