
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

	DECLARE @StartDate DATE
		,@OverlapEmployeeIds NVARCHAR(MAX)
		,@MonthlyTarget TINYINT = 2
		,@SemiTarget TINYINT = 3
		,@ReviewerProcessId TINYINT = 4

	SELECT @StartDate = StartDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	CREATE TABLE #tblPeriodFlowSetting (
		ProcessId INT
		,StartDate DATE
		,EndDate DATE
		)

	CREATE TABLE #tblSemiPeriodSetting (
		PeriodId INT
		,[Month] INT
		,[Year] INT
		)

	IF ISNULL(@StartDate, '') <> ''
	BEGIN
		INSERT INTO #tblPeriodFlowSetting (
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
			AND aa.ProcessId = @ReviewerProcessId
	END

	DELETE
	FROM #tblPeriodFlowSetting
	WHERE EndDate < CAST(GETDATE() AS DATE)

	IF @TargetTypeId = @MonthlyTarget
	BEGIN
		INSERT INTO #tblSemiPeriodSetting
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
		WHERE period.TargetTypeId = @SemiTarget

		SELECT @OverlapEmployeeIds = COALESCE(@OverlapEmployeeIds + ',', '') + CAST(empId.Id AS NVARCHAR)
		FROM dbo.Period AS period WITH (NOLOCK)
			INNER JOIN #tblSemiPeriodSetting AS setting WITH (NOLOCK) ON period.PeriodId = setting.PeriodId
			INNER JOIN dbo.PeriodFlowSetting AS semiFS WITH (NOLOCK) ON period.PeriodId = semiFS.PeriodId
			AND semiFS.ProcessId = @ReviewerProcessId
			INNER JOIN @EmployeeIds AS empId ON semiFS.EmployeeId = empId.Id
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON empId.Id = emp.EmployeeId
			AND emp.HasMonthlyEPA = 1
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			LEFT JOIN dbo.EPA AS epa WITH (NOLOCK) ON semiFS.PeriodId = epa.PeriodId
			AND epa.EmployeeId = semiFS.EmployeeId
		WHERE period.TargetTypeId = @SemiTarget
			AND period.PeriodId <> @PeriodId
			AND (
				semiFS.StartDate <= (
					SELECT TOP 1 empFS.EndDate
					FROM dbo.PeriodFlowSetting AS empFS WITH (NOLOCK)
					WHERE empFS.PeriodId = @PeriodId
						AND empFS.EmployeeId = empId.Id
						AND empFS.ProcessId = @ReviewerProcessId
					)
				OR semiFS.StartDate <= (
					SELECT TOP 1 pFS.EndDate
					FROM #tblPeriodFlowSetting AS pFS WITH (NOLOCK)
					WHERE pFS.ProcessId = @ReviewerProcessId
					)
				)
			AND (
				EXISTS (
					SELECT 1
					FROM dbo.PeriodFlowSetting AS flow WITH (NOLOCK)
					WHERE flow.PeriodId = @PeriodId
						AND flow.EmployeeId = empId.Id
						AND flow.ProcessId = @ReviewerProcessId
						AND MONTH(flow.EndDate) = setting.[Month]
						AND YEAR(flow.EndDate) = setting.[Year]
					)
				OR EXISTS (
					SELECT 1
					FROM #tblPeriodFlowSetting AS flow WITH (NOLOCK)
					WHERE flow.ProcessId = @ReviewerProcessId
						AND MONTH(flow.EndDate) = setting.[Month]
						AND YEAR(flow.EndDate) = setting.[Year]
					)
				)
			AND ISNULL(epa.[Status], 0) = 0
	END
	ELSE IF @TargetTypeId = @SemiTarget
	BEGIN
		INSERT INTO #tblSemiPeriodSetting
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
		WHERE period.TargetTypeId = @SemiTarget
			AND period.PeriodId = @PeriodId

		SELECT @OverlapEmployeeIds = COALESCE(@OverlapEmployeeIds + ',', '') + CAST(empId.Id AS NVARCHAR)
		FROM dbo.Period AS period WITH (NOLOCK)
			INNER JOIN dbo.PeriodFlowSetting AS monthlyFS WITH (NOLOCK) ON period.PeriodId = monthlyFS.PeriodId
			AND monthlyFS.ProcessId = @ReviewerProcessId
			INNER JOIN @EmployeeIds AS empId ON monthlyFS.EmployeeId = empId.Id
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON empId.Id = emp.EmployeeId
			AND emp.HasMonthlyEPA = 1
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			LEFT JOIN dbo.EPA AS epa WITH (NOLOCK) ON monthlyFS.PeriodId = epa.PeriodId
			AND epa.EmployeeId = monthlyFS.EmployeeId
		WHERE period.TargetTypeId = @MonthlyTarget
			AND period.PeriodId <> @PeriodId
			AND (
				monthlyFS.EndDate >= (
					SELECT TOP 1 empFS.StartDate
					FROM dbo.PeriodFlowSetting AS empFS WITH (NOLOCK)
					WHERE empFS.PeriodId = @PeriodId
						AND empFS.EmployeeId = empId.Id
						AND empFS.ProcessId = @ReviewerProcessId
					)
				OR monthlyFS.EndDate >= (
					SELECT TOP 1 pFS.StartDate
					FROM #tblPeriodFlowSetting AS pFS WITH (NOLOCK)
					WHERE pFS.ProcessId = @ReviewerProcessId
					)
				)
			AND (
				EXISTS (
					SELECT 1
					FROM #tblSemiPeriodSetting AS setting WITH (NOLOCK)
					WHERE MONTH(monthlyFS.EndDate) = setting.[Month]
						AND YEAR(monthlyFS.EndDate) = setting.[Year]
					)
				)
			AND ISNULL(epa.[Status], 0) = 0
	END

	IF ISNULL(@OverlapEmployeeIds, '') <> ''
	BEGIN
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@OverlapEmployeeIds, ',')
	END

	DROP TABLE #tblPeriodFlowSetting

	DROP TABLE #tblSemiPeriodSetting
END