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

	CREATE TABLE #EmployeeAssignments (
		PeriodId INT
		,EmployeeId INT
		,EvaluatorId INT
		,ProcessId INT
		,StartDate DATE NULL
		,EndDate DATE NULL
		,[Priority] INT
		)

	CREATE TABLE #SemiPeriodSetting (
		PeriodId INT
		,[Month] INT
		,[Year] INT
		)

	DECLARE @DocHandler INT

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@EmployeeAssignments

	INSERT INTO #EmployeeAssignments (
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
	FROM OPENXML(@DocHandler, '/ROOT/Assignment', 1) WITH (EmployeeId INT, EvaluatorId INT, ProcessId INT, StartDate DATETIME, EndDate DATETIME, [Priority] INT) AS Data

	EXEC sp_xml_removedocument @DocHandler

	IF @TargetTypeId = 2 -- monthly
	BEGIN
		DELETE
		FROM #EmployeeAssignments
		WHERE #EmployeeAssignments.ProcessId <> 4 -- reviewer
			OR CAST(#EmployeeAssignments.EndDate AS DATE) < CAST(GETDATE() AS DATE)

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

		SELECT @OverlapEmployeeIds = COALESCE(@OverlapEmployeeIds + ',', '') + CAST(semiFS.EmployeeId AS NVARCHAR)
		FROM dbo.Period AS period WITH (NOLOCK)
			INNER JOIN #SemiPeriodSetting AS setting ON period.PeriodId = setting.PeriodId
			INNER JOIN dbo.PeriodFlowSetting AS semiFS WITH (NOLOCK) ON period.PeriodId = semiFS.PeriodId
			AND semiFS.ProcessId = 4 -- reviewer
			LEFT JOIN dbo.EPA AS epa WITH (NOLOCK) ON semiFS.PeriodId = epa.PeriodId
			AND epa.EmployeeId = semiFS.EmployeeId
		WHERE period.TargetTypeId = 3 -- semi
			AND period.PeriodId <> @PeriodId
			AND CAST(semiFS.StartDate AS DATE) <= (
				SELECT TOP 1 flow.EndDate
				FROM #EmployeeAssignments AS flow
				WHERE flow.EmployeeId = semiFS.EmployeeId
				)
			AND EXISTS (
				SELECT 1
				FROM #EmployeeAssignments AS flow
				WHERE flow.ProcessId = 4 -- reviewer
					AND flow.EmployeeId = semiFS.EmployeeId
					AND MONTH(flow.EndDate) = setting.[Month]
					AND YEAR(flow.EndDate) = setting.[Year]
				)
			AND (
				epa.[Status] IS NULL
				OR epa.[Status] = 0 -- in progress
				)
	END
	ELSE IF @TargetTypeId = 3 -- semi
	BEGIN
		DELETE
		FROM #EmployeeAssignments
		WHERE ProcessId <> 4 -- reviewer
			OR CAST(EndDate AS DATE) < CAST(GETDATE() AS DATE)

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

		SELECT @OverlapEmployeeIds = COALESCE(@OverlapEmployeeIds + ',', '') + CAST(monthlyFS.EmployeeId AS NVARCHAR)
		FROM dbo.Period AS period WITH (NOLOCK)
			INNER JOIN dbo.PeriodFlowSetting AS monthlyFS WITH (NOLOCK) ON period.PeriodId = monthlyFS.PeriodId
			AND monthlyFS.ProcessId = 4 -- reviewer
			LEFT JOIN dbo.EPA AS epa WITH (NOLOCK) ON monthlyFS.PeriodId = epa.PeriodId
			AND epa.EmployeeId = monthlyFS.EmployeeId
		WHERE period.TargetTypeId = 2 -- monthly
			AND period.PeriodId <> @PeriodId
			AND CAST(monthlyFS.EndDate AS DATE) >= (
				SELECT TOP 1 flow.StartDate
				FROM #EmployeeAssignments AS flow
				WHERE flow.EmployeeId = monthlyFS.EmployeeId
				)
			AND EXISTS (
				SELECT 1
				FROM #SemiPeriodSetting AS setting
				WHERE MONTH(monthlyFS.EndDate) = setting.[Month]
					AND YEAR(monthlyFS.EndDate) = setting.[Year]
				)
			AND (
				epa.[Status] IS NULL
				OR epa.[Status] = 0 -- in progress
				)
	END

	IF ISNULL(@OverlapEmployeeIds, '') <> ''
	BEGIN
		SELECT DISTINCT fnc_strSplit.val
		FROM dbo.fnc_strSplit(@OverlapEmployeeIds, ',')
	END

	DROP TABLE #EmployeeAssignments

	DROP TABLE #SemiPeriodSetting
END