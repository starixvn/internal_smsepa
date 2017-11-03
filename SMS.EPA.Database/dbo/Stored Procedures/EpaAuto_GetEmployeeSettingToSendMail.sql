
CREATE PROCEDURE [dbo].[EpaAuto_GetEmployeeSettingToSendMail] @PeriodId INT
	,@EmployeeIds VARCHAR(MAX) = NULL
	,@ProcessId INT = NULL
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get all employee setting of evaluation period to send mail.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TargetTypeId INT
		,@StartDate DATE
		,@Today DATE = GETDATE()
	-- target type
	DECLARE @SemiId INT = 3
		,@MonthlyId INT = 2

	CREATE TABLE #tblEmployee (EmployeeId INT)

	SELECT @TargetTypeId = TargetTypeId
		,@StartDate = StartDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	IF @EmployeeIds IS NOT NULL
	BEGIN
		INSERT INTO #tblEmployee (EmployeeId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@EmployeeIds, ',')
	END

	IF @TargetTypeId IN (@MonthlyId, @SemiId)
	BEGIN
		IF ISNULL(@StartDate, '') <> ''
		BEGIN
			SELECT DISTINCT pc.ProcessId
				,periodPS.NoticeAtBegin AS HasSendMail
				,DATEADD(DAY, - periodPS.RemindDay, pc.EndDate) AS RemindDate
				,pc.StartDate
				,pc.EndDate
				,pc.EvaluatorId
				,eva.NickName
				,eva.FullName
				,eva.Email
			FROM dbo.EPA AS epa WITH (NOLOCK)
				INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
				INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON pc.EvaluatorId = eva.EmployeeId
				AND eva.[Enabled] = 1
				AND eva.IsDeleted = 0
				INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
				INNER JOIN dbo.PeriodProcessSetting AS periodPS WITH (NOLOCK) ON period.PeriodId = periodPS.PeriodId
				AND pc.ProcessId = periodPS.ProcessId
				LEFT JOIN #tblEmployee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			WHERE ISNULL(epa.[Status], 0) <> 2 -- rejected
				AND epa.PeriodId = @PeriodId
				AND ISNULL(@ProcessId, pc.ProcessId) = pc.ProcessId
				AND (
					epa.HasMonthlyEPA = 0
					OR (
						period.TargetTypeId = @SemiId
						AND epa.HasMonthlyEPA = 1
						AND pc.ProcessId NOT IN (2, 3) -- self, evaluator
						)
					)
				AND (
					ISNULL(@EmployeeIds, '') = ''
					OR emp.EmployeeId IS NOT NULL
					)
				AND @Today BETWEEN pc.StartDate
					AND pc.EndDate
		END
	END
	ELSE
	BEGIN
		SELECT DISTINCT pc.ProcessId
			,pc.NoticeAtBegin AS HasSendMail
			,pc.RemindDate
			,pc.StartDate
			,pc.EndDate
			,pc.EvaluatorId
			,eva.NickName
			,eva.FullName
			,eva.Email
		FROM dbo.EPAProcess AS pc WITH (NOLOCK)
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON pc.EPAId = epa.EPAId
			AND epa.PeriodId = @PeriodId
			AND ISNULL(epa.[Status], 0) <> 2 -- rejected
			INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON pc.EvaluatorId = eva.EmployeeId
			AND eva.[Enabled] = 1
			AND eva.IsDeleted = 0
			LEFT JOIN #tblEmployee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		WHERE ISNULL(@ProcessId, pc.ProcessId) = pc.ProcessId
			AND (
				ISNULL(@EmployeeIds, '') = ''
				OR emp.EmployeeId IS NOT NULL
				)
			AND @Today BETWEEN pc.StartDate
				AND pc.EndDate
	END

	DROP TABLE #tblEmployee
END