
CREATE PROCEDURE [dbo].[EpaAuto_GetEmployeeSettingToSendMail] @PeriodId INT = NULL
	,@ProcessId TINYINT = NULL
	,@MailMode TINYINT = NULL
	,@IsEvaluated BIT = NULL
	,@EmployeeIds VARCHAR(MAX) = NULL
AS
/*
Created: 2021-09-09@Tim.Trinh
Task: Get employee setting to send mail.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@SemiTargetTypeId TINYINT = 3
		-- mail mode
		,@startMode TINYINT = 1
		,@reminderMode TINYINT = 2
		,@endPeriodMode TINYINT = 4

	CREATE TABLE #tblEmployee (EmployeeId INT)

	IF ISNULL(@EmployeeIds, '') = ''
	BEGIN
		INSERT INTO #tblEmployee (EmployeeId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@EmployeeIds, ',')
	END

	SELECT DISTINCT period.PeriodId
		,period.[Name] AS Period
		,psetting.NoticeAtBegin AS HasSendMail
		,pc.ProcessId
		,pc.StartDate
		,pc.EndDate
		,pc.EvaluatorId
		,eva.NickName
		,eva.FullName
		,eva.Email
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.StartDate = @Today
		INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON pc.EvaluatorId = eva.EmployeeId
		AND eva.[Enabled] = 1
		AND eva.IsDeleted = 0
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.PeriodProcessSetting AS psetting WITH (NOLOCK) ON period.PeriodId = psetting.PeriodId
		AND pc.ProcessId = psetting.ProcessId
		LEFT JOIN #tblEmployee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
	WHERE (
			@PeriodId IS NULL
			OR period.PeriodId = @PeriodId
		)
		AND (@ProcessId IS NULL OR pc.ProcessId = @ProcessId)
		AND (
			ISNULL(@EmployeeIds, '') = ''
			OR emp.EmployeeId IS NOT NULL
		)
		AND ISNULL(epa.[Status], 0) <> 2 -- rejected
		AND (
			epa.HasMonthlyEPA = 0
			OR (
				period.TargetTypeId = @SemiTargetTypeId
				AND epa.HasMonthlyEPA = 1
				AND pc.ProcessId NOT IN (2, 3) -- self, evaluator
			)
		)
		AND (
			@IsEvaluated IS NULL
			OR (
				@IsEvaluated = 0
				AND pc.EvaluateDate IS NULL
				AND pc.ProcessId > 1 -- hr
			)
			OR (
				@IsEvaluated = 1
				AND pc.EvaluateDate IS NOT NULL
				AND pc.ProcessId > 1 -- hr
			)
		)
		AND (
			@MailMode IS NULL
			OR (@MailMode = @startMode AND pc.StartDate = @Today)
			OR (@MailMode = @reminderMode AND CAST(DATEADD(DAY, - psetting.RemindDay, pc.EndDate) AS DATE) = @Today)
		)
	ORDER BY period.PeriodId
		,pc.ProcessId

	DROP TABLE #tblEmployee
END