/****** Object:  Stored Procedure dbo.EpaAuto_GetEmployeeSettingToSendMail    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_GetEmployeeSettingToSendMail    Script Date: 5/16/2016 2:15:33 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_GetEmployeeSettingToSendMail    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_GetEmployeeSettingToSendMail    Script Date: 4/6/2016 11:27:43 AM ******/

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
		,@StartDate DATETIME

	SELECT @TargetTypeId = TargetTypeId
		,@StartDate = StartDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	IF @TargetTypeId IN (2, 3) -- Monthly, Semi
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
				AND pc.IsEndProcess = 0 -- in progress
				INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON pc.EvaluatorId = eva.EmployeeId
				AND eva.[Enabled] = 1
				AND eva.IsDeleted = 0
				INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
				INNER JOIN dbo.PeriodProcessSetting AS periodPS WITH (NOLOCK) ON period.PeriodId = periodPS.PeriodId
				AND pc.ProcessId = periodPS.ProcessId
			WHERE epa.[Status] = 0 -- open
				AND epa.PeriodId = @PeriodId
				AND (
					ISNULL(@ProcessId, '') = ''
					OR pc.ProcessId = @ProcessId
					)
				AND (
					ISNULL(@EmployeeIds, '') = ''
					OR epa.EmployeeId IN (
						SELECT DISTINCT val
						FROM fnc_strSplit(@EmployeeIds, ',')
						)
					)
				AND CAST(pc.StartDate AS DATE) <= CAST(GETDATE() AS DATE)
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
			AND epa.[Status] = 0 -- open
			INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON pc.EvaluatorId = eva.EmployeeId
			AND eva.[Enabled] = 1
			AND eva.IsDeleted = 0
		WHERE pc.IsEndProcess = 0 -- in progress
			AND (
				ISNULL(@ProcessId, '') = ''
				OR pc.ProcessId = @ProcessId
				)
			AND (
				ISNULL(@EmployeeIds, '') = ''
				OR epa.EmployeeId IN (
					SELECT DISTINCT val
					FROM fnc_strSplit(@EmployeeIds, ',')
					)
				)
			AND CAST(pc.StartDate AS DATE) <= CAST(GETDATE() AS DATE)
	END
END