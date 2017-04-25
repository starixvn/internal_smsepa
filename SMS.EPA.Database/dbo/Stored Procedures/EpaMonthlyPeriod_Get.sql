CREATE PROCEDURE [dbo].[EpaMonthlyPeriod_Get] 
	@FromDate DATETIME,
	@ToDate DATETIME
AS
/*
Created: 20160215@Tim.Trinh
Task: Get monthly period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
20170425@Hunter.Tran: Filter with end date instead of start date
*/
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#EmployeeSetting') IS NOT NULL
	BEGIN
		DROP TABLE #EmployeeSetting
	END

	CREATE TABLE #EmployeeSetting 
	(
		PeriodId INT,
		EmployeeId INT
	)

	INSERT INTO #EmployeeSetting
	SELECT DISTINCT period.PeriodId, peFS.EmployeeId
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN dbo.PeriodFlowSetting AS peFS WITH (NOLOCK) ON period.PeriodId = peFS.PeriodId
	WHERE NOT EXISTS 
	(
		SELECT 1
		FROM dbo.PeriodExcludedEmployee AS peExcluded WITH (NOLOCK)
		WHERE peFS.EmployeeId = peExcluded.EmployeeId
			AND peFS.PeriodId = peExcluded.PeriodId
	)
		AND period.TargetTypeId = 2 -- Monhtly
		AND CAST(period.EndDate AS DATE) BETWEEN CAST(@FromDate AS DATE)
			AND CAST(@ToDate AS DATE)
	
	UNION
	SELECT period.PeriodId
		,peTS.EmployeeId
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN dbo.PeriodTargetSetting AS peTS WITH (NOLOCK) ON period.PeriodId = peTS.PeriodId
	WHERE NOT EXISTS
	(
		SELECT 1
		FROM dbo.PeriodFlowSetting AS peFS WITH (NOLOCK)
		WHERE peTS.EmployeeId = peFS.EmployeeId
			AND peTS.PeriodId = peFS.PeriodId
	)
	AND NOT EXISTS 
	(
		SELECT 1
		FROM dbo.PeriodExcludedEmployee AS peExcluded WITH (NOLOCK)
		WHERE peTS.EmployeeId = peExcluded.EmployeeId
			AND peTS.PeriodId = peExcluded.PeriodId
	)
	AND period.TargetTypeId = 2 -- Monhtly
	AND CAST(period.EndDate AS DATE) BETWEEN CAST(@FromDate AS DATE)
		AND CAST(@ToDate AS DATE)

	SELECT DISTINCT 
		pe.PeriodId,
		pe.[Name],
		pe.StartDate,
		pe.EndDate,
		pe.[Description],
		pe.IsActive,
		peF.TotalEmployees,
		epa.TotalEmployeesUnfinished
	FROM dbo.Period AS pe WITH (NOLOCK)
		LEFT JOIN 
		(
		SELECT PeriodId
			,COUNT(EmployeeId) AS TotalEmployees
		FROM #EmployeeSetting
		GROUP BY PeriodId
		) AS peF ON peF.PeriodId = pe.PeriodId
		LEFT JOIN 
		(
		SELECT epa1.PeriodId
			,COUNT(EmployeeId) AS TotalEmployeesUnfinished
		FROM dbo.EPA AS epa1 WITH (NOLOCK)
			INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa1.PeriodId = period1.PeriodId
		WHERE epa1.[Status] = 0
			AND NOT EXISTS 
			(
				SELECT 1
				FROM dbo.PeriodExcludedEmployee AS peExcluded WITH (NOLOCK)
				WHERE epa1.PeriodId = peExcluded.PeriodId
					AND epa1.EmployeeId = peExcluded.EmployeeId
			)
		GROUP BY epa1.PeriodId
		) AS epa ON epa.PeriodId = pe.PeriodId
	WHERE pe.TargetTypeId = 2 -- Monhtly
		AND CAST(pe.EndDate AS DATE) BETWEEN CAST(@FromDate AS DATE)
			AND CAST(@ToDate AS DATE)
	ORDER BY pe.EndDate DESC

	IF OBJECT_ID('tempdb..#EmployeeSetting') IS NOT NULL
	BEGIN
		DROP TABLE #EmployeeSetting
	END
END