CREATE PROCEDURE [dbo].[EpaPeriod_AddEmployee_CheckEmployeeOverlap] @PeriodId INT
	,@TargetTypeId INT
	,@EmployeeIds IntId READONLY
AS
/*
Created: 20160304@Tim.Trinh
Task: Check overlap date between semi and monthly.
DB: SMSEPA
Original:

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @StartDate DATETIME
		,@OverlapEmployeeIds NVARCHAR(MAX)

	SELECT @StartDate = StartDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	CREATE TABLE #PeriodFlowSetting (
		ProcessId INT
		,StartDate DATE
		,EndDate DATE
		)

	CREATE TABLE #SemiPeriodSetting (
		PeriodId INT
		,[Month] INT
		,[Year] INT
		)

	IF ISNULL(@StartDate, '') <> ''
	BEGIN
		INSERT INTO #PeriodFlowSetting (
			ProcessId
			,StartDate
			,EndDate
			)
		SELECT DISTINCT periodPS.ProcessId
			,DATEADD(DAY, ISNULL(aa.Duration - periodPS.Duration, 0), @StartDate)
			,DATEADD(DAY, (CASE 
					WHEN ISNULL(aa.Duration, 0) - 1 < 0 THEN 0
					ELSE ISNULL(aa.Duration, 0) - 1 END
					), @StartDate)
		FROM dbo.PeriodProcessSetting AS periodPS WITH (NOLOCK)
			INNER JOIN (
			SELECT set1.PeriodId
				,set1.ProcessId
				,SUM(CASE 
						WHEN set2.ProcessId <= set1.ProcessId THEN set2.Duration
						ELSE 0
						END) AS Duration
			FROM dbo.PeriodProcessSetting AS set1 WITH (NOLOCK)
				INNER JOIN dbo.PeriodProcessSetting AS set2 WITH (NOLOCK) ON set1.PeriodId = set2.PeriodId
			WHERE set1.PeriodId = @PeriodId
			GROUP BY set1.PeriodId
				,set1.ProcessId
			) AS aa ON periodPS.PeriodId = aa.PeriodId
			AND periodPS.ProcessId = aa.ProcessId
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON periodPS.PeriodId = period.PeriodId
			AND period.TargetTypeId = @TargetTypeId
		WHERE period.PeriodId = @PeriodId
			AND aa.ProcessId = 4 -- reviewer
	END

	DELETE
	FROM #PeriodFlowSetting
	WHERE CAST(EndDate AS DATE) < CAST(GETDATE() AS DATE)

	IF @TargetTypeId = 2 -- monthly
	BEGIN
		INSERT INTO #SemiPeriodSetting
		SELECT period.PeriodId
			,semiSD.[Month]
			,(CASE 
				WHEN semiSD.IsLastYear = 1 THEN YEAR(period.StartDate) - 1
				ELSE YEAR(period.StartDate) END
				) AS [Year]
		FROM dbo.Period AS period WITH (NOLOCK)
			INNER JOIN dbo.SemiPeriodSetting AS semiS WITH (NOLOCK) ON CAST(period.StartDate AS DATE) BETWEEN CAST(CAST(YEAR(period.StartDate) AS VARCHAR) + '-' + semiS.FromDate AS DATE)
				AND CAST(CAST(YEAR(period.StartDate) AS VARCHAR) + '-' + semiS.ToDate AS DATE)
			INNER JOIN dbo.SemiPeriodSettingDetail AS semiSD WITH (NOLOCK) ON semiS.SemiPeriodSettingId = semiSD.SemiPeriodSettingId
			AND semiSD.IsActive = 1
		WHERE period.TargetTypeId = 3 -- semi

		SELECT @OverlapEmployeeIds = COALESCE(@OverlapEmployeeIds + ',', '') + CAST(empId.Id AS NVARCHAR)
		FROM dbo.Period AS period WITH (NOLOCK)
			INNER JOIN #SemiPeriodSetting AS setting ON period.PeriodId = setting.PeriodId
			INNER JOIN dbo.PeriodFlowSetting AS semiFS WITH (NOLOCK) ON period.PeriodId = semiFS.PeriodId
			AND semiFS.ProcessId = 4 -- reviewer
			INNER JOIN @EmployeeIds AS empId ON semiFS.EmployeeId = empId.Id
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON empId.Id = emp.EmployeeId
			AND emp.HasMonthlyEPA = 1
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			LEFT JOIN dbo.EPA AS epa WITH (NOLOCK) ON semiFS.PeriodId = epa.PeriodId
			AND epa.EmployeeId = semiFS.EmployeeId
		WHERE period.TargetTypeId = 3 -- semi
			AND period.PeriodId <> @PeriodId
			AND (
				CAST(semiFS.StartDate AS DATE) <= (
					SELECT TOP 1 CAST(empFS.EndDate AS DATE)
					FROM dbo.PeriodFlowSetting AS empFS WITH (NOLOCK)
					WHERE empFS.PeriodId = @PeriodId
						AND empFS.EmployeeId = empId.Id
						AND empFS.ProcessId = 4 -- reviewer
					)
				OR CAST(semiFS.StartDate AS DATE) <= (
					SELECT TOP 1 pFS.EndDate
					FROM #PeriodFlowSetting AS pFS
					WHERE pFS.ProcessId = 4 -- reviewer
					)
				)
			AND (
				EXISTS (
					SELECT 1
					FROM dbo.PeriodFlowSetting AS flow WITH (NOLOCK)
					WHERE flow.PeriodId = @PeriodId
						AND flow.EmployeeId = empId.Id
						AND flow.ProcessId = 4 -- reviewer
						AND MONTH(flow.EndDate) = setting.[Month]
						AND YEAR(flow.EndDate) = setting.[Year]
					)
				OR EXISTS (
					SELECT 1
					FROM #PeriodFlowSetting AS flow
					WHERE flow.ProcessId = 4 -- reviewer
						AND MONTH(flow.EndDate) = setting.[Month]
						AND YEAR(flow.EndDate) = setting.[Year]
					)
				)
			AND (
				epa.[Status] IS NULL
				OR epa.[Status] = 0 -- in progress
				)
	END
	ELSE IF @TargetTypeId = 3 -- semi
	BEGIN
		INSERT INTO #SemiPeriodSetting
		SELECT period.PeriodId
			,semiSD.[Month]
			,(CASE 
				WHEN semiSD.IsLastYear = 1 THEN YEAR(period.StartDate) - 1
				ELSE YEAR(period.StartDate) END
				) AS [Year]
		FROM dbo.Period AS period WITH (NOLOCK)
			INNER JOIN dbo.SemiPeriodSetting AS semiS WITH (NOLOCK) ON CAST(period.StartDate AS DATE) BETWEEN CAST(CAST(YEAR(period.StartDate) AS VARCHAR) + '-' + semiS.FromDate AS DATE)
				AND CAST(CAST(YEAR(period.StartDate) AS VARCHAR) + '-' + semiS.ToDate AS DATE)
			INNER JOIN dbo.SemiPeriodSettingDetail AS semiSD WITH (NOLOCK) ON semiS.SemiPeriodSettingId = semiSD.SemiPeriodSettingId
			AND semiSD.IsActive = 1
		WHERE period.TargetTypeId = 3 -- semi
			AND period.PeriodId = @PeriodId

		SELECT @OverlapEmployeeIds = COALESCE(@OverlapEmployeeIds + ',', '') + CAST(empId.Id AS NVARCHAR)
		FROM dbo.Period AS period WITH (NOLOCK)
			INNER JOIN dbo.PeriodFlowSetting AS monthlyFS WITH (NOLOCK) ON period.PeriodId = monthlyFS.PeriodId
			AND monthlyFS.ProcessId = 4 -- reviewer
			INNER JOIN @EmployeeIds AS empId ON monthlyFS.EmployeeId = empId.Id
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON empId.Id = emp.EmployeeId
			AND emp.HasMonthlyEPA = 1
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			LEFT JOIN dbo.EPA AS epa WITH (NOLOCK) ON monthlyFS.PeriodId = epa.PeriodId
			AND epa.EmployeeId = monthlyFS.EmployeeId
		WHERE period.TargetTypeId = 2 -- monthly
			AND period.PeriodId <> @PeriodId
			AND (
				CAST(monthlyFS.EndDate AS DATE) >= (
					SELECT TOP 1 CAST(empFS.StartDate AS DATE)
					FROM dbo.PeriodFlowSetting AS empFS WITH (NOLOCK)
					WHERE empFS.PeriodId = @PeriodId
						AND empFS.EmployeeId = empId.Id
						AND empFS.ProcessId = 4 -- reviewer
					)
				OR CAST(monthlyFS.EndDate AS DATE) >= (
					SELECT TOP 1 CAST(pFS.StartDate AS DATE)
					FROM #PeriodFlowSetting AS pFS
					WHERE pFS.ProcessId = 4 -- reviewer
					)
				)
			AND (
				EXISTS (
					SELECT 1
					FROM #SemiPeriodSetting AS setting
					WHERE MONTH(monthlyFS.EndDate) = setting.[Month]
						AND YEAR(monthlyFS.EndDate) = setting.[Year]
					)
				)
			AND (
				epa.[Status] IS NULL
				OR epa.[Status] = 0 -- in progress
				)
	END

	IF ISNULL(@OverlapEmployeeIds, '') <> ''
	BEGIN
		SELECT DISTINCT val
		FROM fnc_strSplit(@OverlapEmployeeIds, ',')
	END

	DROP TABLE #PeriodFlowSetting

	DROP TABLE #SemiPeriodSetting
END