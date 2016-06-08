/****** Object:  Stored Procedure dbo.EpaPeriod_Get    Script Date: 5/16/2016 2:20:08 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_Get    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_Get    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaMonthlyPeriod_Get    Script Date: 4/6/2016 11:27:45 AM ******/

CREATE PROCEDURE [dbo].[EpaPeriod_Get] @TargetTypeId INT = NULL
	,@IsActive BIT = 1
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get all evaluation period.
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
		AND (
			ISNULL(@TargetTypeId, '') = ''
			OR period.TargetTypeId = @TargetTypeId
			)
		AND (
			@IsActive IS NULL
			OR period.IsActive = @IsActive
			)
		AND period.TargetTypeId <> 2 -- monthly
	
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
		AND (
			ISNULL(@TargetTypeId, '') = ''
			OR period.TargetTypeId = @TargetTypeId
			)
		AND (
			@IsActive IS NULL
			OR period.IsActive = @IsActive
			)
		AND period.TargetTypeId <> 2 -- monthly

	SELECT DISTINCT pe.PeriodId
		,pe.[Name]
		,pe.StartDate
		,pe.EndDate
		,pe.AttendanceStartDate
		,pe.AttendanceEndDate
		,pe.[Description]
		,pe.IsActive
		,peF.TotalEmployees
		,epa.TotalEmployeesUnfinished
		,tar.TargetTypeId
		,tar.NAME
	FROM dbo.Period AS pe WITH (NOLOCK)
		INNER JOIN dbo.TargetType AS tar WITH (NOLOCK) ON pe.TargetTypeId = tar.TargetTypeId
		LEFT JOIN (
		SELECT PeriodId
			,COUNT(EmployeeId) AS TotalEmployees
		FROM #EmployeeSetting
		GROUP BY PeriodId
		) AS peF ON peF.PeriodId = pe.PeriodId
		LEFT JOIN (
		SELECT PeriodId
			,COUNT(EmployeeId) AS TotalEmployeesUnfinished
		FROM dbo.EPA WITH (NOLOCK)
		WHERE [Status] = 0
		GROUP BY PeriodId
		) AS epa ON pe.PeriodId = epa.PeriodId
	WHERE (
			ISNULL(@TargetTypeId, '') = ''
			OR pe.TargetTypeId = @TargetTypeId
			)
		AND (
			@IsActive IS NULL
			OR pe.IsActive = @IsActive
			)
		AND pe.TargetTypeId <> 2 -- monthly
END