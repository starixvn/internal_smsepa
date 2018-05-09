
CREATE PROCEDURE [dbo].[EpaAuto_Promotion_GetEmployees]
AS
/*
Created: 2015-09-01@Josh.Phan
Task: Get employee that has promotion started today.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT pe.[PeriodId]
		,peFS.[EmployeeId]
	FROM [dbo].[Period] AS pe WITH (NOLOCK)
		INNER JOIN [dbo].[PeriodFlowSetting] AS peFS WITH (NOLOCK) ON pe.[PeriodId] = peFS.[PeriodId]
		LEFT JOIN dbo.EPA AS epa WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
		AND peFS.EmployeeId = epa.EmployeeId
	WHERE pe.[TargetTypeId] = 5 -- Promotion
		AND peFS.[ProcessId] = 1 -- Human Resource
		AND CAST(peFS.[StartDate] AS DATE) = CAST(GETDATE() AS DATE)
		AND epa.EPAId IS NULL
END