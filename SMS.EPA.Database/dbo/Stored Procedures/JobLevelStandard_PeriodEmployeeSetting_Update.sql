

CREATE PROCEDURE [dbo].[JobLevelStandard_PeriodEmployeeSetting_Update] @PeriodId INT
	,@SettingXml NVARCHAR(MAX) = NULL
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 2022-08-10@Tim.Trinh
Task: update employee setting
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
	@SettingXml: <Root><Row EmployeeId='' ProcessId='' EvaluatorId='' FromDate='' ToDate='' /></Root>
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @pendingStatusId TINYINT = 1
		,@verifiedStatusId TINYINT = 2
		,@approvedStatusId TINYINT = 3
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		,@today DATE = GETDATE()
		,@modifiedDate DATETIME = GETDATE()
	DECLARE @DocHandler INT

	CREATE TABLE #tmpEmployeeSetting (
		ProcessId TINYINT
		,EmployeeId INT
		,EvaluatorId INT
		,FromDate DATE
		,ToDate DATE
		)

	CREATE TABLE #tmpLogData (
		ProcessId TINYINT
		,EmployeeId INT
		,OldEvaluatorId INT
		,NewEvaluatorId INT
		,OldFromDate DATE
		,NewFromDate DATE
		,OldToDate DATE
		,NewToDate DATE
		)

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@SettingXml

	INSERT INTO #tmpEmployeeSetting (
		EmployeeId
		,EvaluatorId
		,ProcessId
		,FromDate
		,ToDate
		)
	SELECT EmployeeId
		,EvaluatorId
		,ProcessId
		,FromDate
		,ToDate
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (EmployeeId INT, EvaluatorId INT, ProcessId TINYINT, FromDate DATE, ToDate DATE) AS Data

	EXEC sp_xml_removedocument @DocHandler

	DELETE pps
	OUTPUT DELETED.ProcessId
		,DELETED.EmployeeId
		,DELETED.EvaluatorId
		,DELETED.FromDate
		,DELETED.ToDate
	INTO #tmpLogData(ProcessId, EmployeeId, OldEvaluatorId, OldFromDate, OldToDate)
	FROM dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (ROWLOCK, UPDLOCK)
	WHERE pps.PeriodId = @PeriodId
		AND EXISTS (
			SELECT 1
			FROM #tmpEmployeeSetting AS tmp WITH (NOLOCK)
			WHERE tmp.EmployeeId = pps.EmployeeId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM #tmpEmployeeSetting AS tmp WITH (NOLOCK)
			WHERE tmp.ProcessId = pps.ProcessId
				AND tmp.EmployeeId = pps.EmployeeId
				AND tmp.EvaluatorId = pps.EvaluatorId
			)

	UPDATE pps
	SET pps.FromDate = tmp.FromDate
		,pps.ToDate = tmp.ToDate
		,pps.ModifiedBy = @ModifiedBy
		,pps.ModifiedDate = @modifiedDate
	OUTPUT INSERTED.ProcessId
		,INSERTED.EmployeeId
		,DELETED.EvaluatorId
		,INSERTED.EvaluatorId
		,DELETED.FromDate
		,INSERTED.FromDate
		,DELETED.ToDate
		,INSERTED.ToDate
	INTO #tmpLogData(ProcessId, EmployeeId, OldEvaluatorId, NewEvaluatorId, OldFromDate, NewFromDate, OldToDate, NewToDate)
	FROM dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpEmployeeSetting AS tmp WITH (NOLOCK) ON pps.ProcessId = tmp.ProcessId
		AND pps.EmployeeId = tmp.EmployeeId
		AND pps.EvaluatorId = tmp.EvaluatorId
	WHERE pps.PeriodId = @PeriodId
		AND (
			pps.FromDate != tmp.FromDate
			OR pps.ToDate != tmp.ToDate
			)

	INSERT INTO dbo.JobLevelStandard_PeriodProcessSetting (
		PeriodId
		,ProcessId
		,EmployeeId
		,EvaluatorId
		,FromDate
		,ToDate
		,CreatedBy
		,CreatedDate
		)
	OUTPUT INSERTED.ProcessId
		,INSERTED.EmployeeId
		,INSERTED.EvaluatorId
		,INSERTED.FromDate
		,INSERTED.ToDate
	INTO #tmpLogData(ProcessId, EmployeeId, NewEvaluatorId, NewFromDate, NewToDate)
	SELECT @PeriodId
		,ProcessId
		,EmployeeId
		,EvaluatorId
		,FromDate
		,ToDate
		,@ModifiedBy
		,@modifiedDate
	FROM #tmpEmployeeSetting AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK)
			WHERE pps.PeriodId = @PeriodId
				AND pps.ProcessId = tmp.ProcessId
				AND pps.EmployeeId = tmp.EmployeeId
				AND pps.EvaluatorId = tmp.EvaluatorId
			)

	UPDATE a
	SET a.StatusId = @pendingStatusId
		,a.ModifiedBy = @ModifiedBy
		,a.ModifiedDate = @modifiedDate
	FROM dbo.JobLevelStandard_Assessment AS a WITH (ROWLOCK, UPDLOCK)
	WHERE a.PeriodId = @PeriodId
		AND a.StatusId IN (@verifiedStatusId, @approvedStatusId)
		AND EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_PeriodProcessSetting AS ps WITH (NOLOCK)
				INNER JOIN #tmpEmployeeSetting AS tmp WITH (NOLOCK) ON ps.EmployeeId = tmp.EmployeeId
				AND ps.EvaluatorId = tmp.EvaluatorId
				AND ps.ProcessId = tmp.ProcessId
			WHERE a.PeriodId = ps.PeriodId
				AND a.EmployeeId = tmp.EmployeeId
				AND ps.ProcessId = 1
				AND ps.ToDate >= @today
			)

	-- write log
	SET @xml = (
			SELECT ProcessId AS '@ProcessId'
				,EmployeeId AS '@EmployeeId'
				,OldEvaluatorId AS '@EvaluatorId'
				,OldFromDate AS '@FromDate'
				,OldToDate AS '@ToDate'
			FROM #tmpLogData WITH (NOLOCK)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @xml = (
			SELECT ProcessId AS '@ProcessId'
				,EmployeeId AS '@EmployeeId'
				,NewEvaluatorId AS '@EvaluatorId'
				,NewFromDate AS '@FromDate'
				,NewToDate AS '@ToDate'
			FROM #tmpLogData WITH (NOLOCK)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SpName = 'JobLevelStandard_PeriodEmployeeSetting_Update'
		,@ObjectId = @PeriodId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@ObjectIds = NULL

	DROP TABLE #tmpEmployeeSetting

	DROP TABLE #tmpLogData
END