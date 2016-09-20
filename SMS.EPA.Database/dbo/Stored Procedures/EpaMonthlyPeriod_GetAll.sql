
CREATE PROCEDURE [dbo].[EpaMonthlyPeriod_GetAll]
AS
/*
Created: 20160520@Razor.Nguyen
Task: Get All monthly period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#EmployeeSetting') IS NOT NULL
	BEGIN
		DROP TABLE #EmployeeSetting
	END

	CREATE TABLE #EmployeeSetting (
		PeriodId INT
		,EmployeeId INT
		)

	INSERT INTO #EmployeeSetting
	SELECT DISTINCT period.PeriodId
		,peFS.EmployeeId
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN dbo.PeriodFlowSetting AS peFS WITH (NOLOCK) ON period.PeriodId = peFS.PeriodId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodExcludedEmployee AS peExcluded WITH (NOLOCK)
			WHERE peFS.EmployeeId = peExcluded.EmployeeId
				AND peFS.PeriodId = peExcluded.PeriodId
			)
		AND period.TargetTypeId = 2 -- Monhtly
	
	UNION
	SELECT period.PeriodId
		,peTS.EmployeeId
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN dbo.PeriodTargetSetting AS peTS WITH (NOLOCK) ON period.PeriodId = peTS.PeriodId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodFlowSetting AS peFS WITH (NOLOCK)
			WHERE peTS.EmployeeId = peFS.EmployeeId
				AND peTS.PeriodId = peFS.PeriodId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodExcludedEmployee AS peExcluded WITH (NOLOCK)
			WHERE peTS.EmployeeId = peExcluded.EmployeeId
				AND peTS.PeriodId = peExcluded.PeriodId
			)
		AND period.TargetTypeId = 2 -- Monhtly

	SELECT DISTINCT pe.PeriodId
		,pe.[Name]
		,pe.StartDate
		,pe.EndDate
		,pe.[Description]
		,pe.IsActive
		,peF.TotalEmployees
		,epa.TotalEmployeesUnfinished
	FROM dbo.Period AS pe WITH (NOLOCK)
		LEFT JOIN (
		SELECT PeriodId
			,COUNT(EmployeeId) AS TotalEmployees
		FROM #EmployeeSetting
		GROUP BY PeriodId
		) AS peF ON peF.PeriodId = pe.PeriodId
		LEFT JOIN (
		SELECT epa1.PeriodId
			,COUNT(epa1.EmployeeId) AS TotalEmployeesUnfinished
		FROM dbo.EPA AS epa1 WITH (NOLOCK)
			INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa1.PeriodId = period1.PeriodId
		WHERE epa1.[Status] = 0
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.PeriodExcludedEmployee AS peExcluded WITH (NOLOCK)
				WHERE epa1.PeriodId = peExcluded.PeriodId
					AND epa1.EmployeeId = peExcluded.EmployeeId
				)
		GROUP BY epa1.PeriodId
		) AS epa ON epa.PeriodId = pe.PeriodId
	WHERE pe.TargetTypeId = 2 -- Monhtly
	ORDER BY pe.StartDate DESC

	IF OBJECT_ID('tempdb..#EmployeeSetting') IS NOT NULL
	BEGIN
		DROP TABLE #EmployeeSetting
	END
END