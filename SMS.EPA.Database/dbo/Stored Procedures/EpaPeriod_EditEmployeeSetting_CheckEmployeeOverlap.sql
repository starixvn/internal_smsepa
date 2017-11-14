
CREATE PROCEDURE [dbo].[EpaPeriod_EditEmployeeSetting_CheckEmployeeOverlap] @PeriodId INT
	,@TargetTypeId INT
	,@EmployeeAssignments NVARCHAR(MAX)
AS
/*
Created: 20160304@Tim.Trinh
Task: Check overlap date between semi and monthly.
DB: SMSEPA
Original:

Revisions:  
Param's Explanation:
     @EmployeeAssignments = 
		'<ROOT>
			<Assignment EmployeeId='' ProcessId='' EvaluatorId='' StartDate='' EndDate='' Priority=''></Assignment>
			<Assignment EmployeeId=''ProcessId='' EvaluatorId='' StartDate='' EndDate='' Priority=''></Assignment>
		</ROOT>'
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @OverlapEmployeeIds NVARCHAR(MAX)
		,@MonthlyTarget TINYINT = 2
		,@SemiTarget TINYINT = 3
		,@ReviewerProcessId TINYINT = 4

	CREATE TABLE #tblEmployeeAssignments (
		PeriodId INT
		,EmployeeId INT
		,EvaluatorId INT
		,ProcessId TINYINT
		,StartDate DATE NULL
		,EndDate DATE NULL
		,[Priority] TINYINT
		)

	CREATE TABLE #tblSemiPeriodSetting (
		PeriodId INT
		,[Month] TINYINT
		,[Year] INT
		)

	DECLARE @DocHandler INT

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@EmployeeAssignments

	INSERT INTO #tblEmployeeAssignments (
		PeriodId
		,EmployeeId
		,EvaluatorId
		,ProcessId
		,StartDate
		,EndDate
		,[Priority]
		)
	SELECT @PeriodId
		,EmployeeId
		,EvaluatorId
		,ProcessId
		,CASE 
			WHEN StartDate = '01 Jan, 1900' THEN NULL
			ELSE StartDate
			END
		,CASE 
			WHEN EndDate = '01 Jan, 1900' THEN NULL
			ELSE EndDate
			END
		,[Priority]
	FROM OPENXML(@DocHandler, '/ROOT/Assignment', 1) WITH (EmployeeId INT, EvaluatorId INT, ProcessId TINYINT, StartDate DATE, EndDate DATE, [Priority] TINYINT) AS Data

	EXEC sp_xml_removedocument @DocHandler

	IF @TargetTypeId = @MonthlyTarget
	BEGIN
		DELETE
		FROM #tblEmployeeAssignments
		WHERE ProcessId <> @ReviewerProcessId
			OR EndDate < CAST(GETDATE() AS DATE)

		INSERT INTO #tblSemiPeriodSetting (
			PeriodId
			,[Month]
			,[Year]
			)
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

		SELECT @OverlapEmployeeIds = COALESCE(@OverlapEmployeeIds + ',', '') + CAST(semiFS.EmployeeId AS NVARCHAR)
		FROM dbo.Period AS period WITH (NOLOCK)
			INNER JOIN #tblSemiPeriodSetting AS setting ON period.PeriodId = setting.PeriodId
			INNER JOIN dbo.PeriodFlowSetting AS semiFS WITH (NOLOCK) ON period.PeriodId = semiFS.PeriodId
			AND semiFS.ProcessId = @ReviewerProcessId
			LEFT JOIN dbo.EPA AS epa WITH (NOLOCK) ON semiFS.PeriodId = epa.PeriodId
			AND epa.EmployeeId = semiFS.EmployeeId
		WHERE period.TargetTypeId = @SemiTarget
			AND period.PeriodId <> @PeriodId
			AND semiFS.StartDate <= (
				SELECT TOP 1 flow.EndDate
				FROM #tblEmployeeAssignments AS flow WITH (NOLOCK)
				WHERE flow.EmployeeId = semiFS.EmployeeId
				)
			AND EXISTS (
				SELECT 1
				FROM #tblEmployeeAssignments AS flow WITH (NOLOCK)
				WHERE flow.ProcessId = @ReviewerProcessId
					AND flow.EmployeeId = semiFS.EmployeeId
					AND MONTH(flow.EndDate) = setting.[Month]
					AND YEAR(flow.EndDate) = setting.[Year]
				)
			AND ISNULL(epa.[Status], 0) = 0
	END
	ELSE IF @TargetTypeId = @SemiTarget
	BEGIN
		DELETE
		FROM #tblEmployeeAssignments
		WHERE ProcessId <> @ReviewerProcessId
			OR EndDate < CAST(GETDATE() AS DATE)

		INSERT INTO #tblSemiPeriodSetting (
			PeriodId
			,[Month]
			,[Year]
			)
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

		SELECT @OverlapEmployeeIds = COALESCE(@OverlapEmployeeIds + ',', '') + CAST(monthlyFS.EmployeeId AS NVARCHAR)
		FROM dbo.Period AS period WITH (NOLOCK)
			INNER JOIN dbo.PeriodFlowSetting AS monthlyFS WITH (NOLOCK) ON period.PeriodId = monthlyFS.PeriodId
			AND monthlyFS.ProcessId = @ReviewerProcessId
			LEFT JOIN dbo.EPA AS epa WITH (NOLOCK) ON monthlyFS.PeriodId = epa.PeriodId
			AND epa.EmployeeId = monthlyFS.EmployeeId
		WHERE period.TargetTypeId = @MonthlyTarget
			AND period.PeriodId <> @PeriodId
			AND monthlyFS.EndDate >= (
				SELECT TOP 1 flow.StartDate
				FROM #tblEmployeeAssignments AS flow WITH (NOLOCK)
				WHERE flow.EmployeeId = monthlyFS.EmployeeId
				)
			AND EXISTS (
				SELECT 1
				FROM #tblSemiPeriodSetting AS setting WITH (NOLOCK)
				WHERE MONTH(monthlyFS.EndDate) = setting.[Month]
					AND YEAR(monthlyFS.EndDate) = setting.[Year]
				)
			AND ISNULL(epa.[Status], 0) = 0
	END

	IF ISNULL(@OverlapEmployeeIds, '') <> ''
	BEGIN
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@OverlapEmployeeIds, ',')
	END

	DROP TABLE #tblEmployeeAssignments

	DROP TABLE #tblSemiPeriodSetting
END