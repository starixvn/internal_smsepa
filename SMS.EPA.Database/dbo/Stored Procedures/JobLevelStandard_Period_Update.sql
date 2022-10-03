
CREATE PROCEDURE [dbo].[JobLevelStandard_Period_Update] @PeriodId INT
	,@Name NVARCHAR(200)
	,@SettingXml VARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2022-08-10@Tim.Trinh
Task: update period
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
	@SettingXml: <Root><Row ProcessId='' FromDate='' ToDate='' /></Root>
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_Period WITH (NOLOCK)
			WHERE PeriodId != @PeriodId
				AND [Name] = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Period ''' + @Name + ''' has already existed.'

		RETURN
	END

	DECLARE @today DATE = GETDATE()
		,@pendingStatusId TINYINT = 1
		,@verifiedStatusId TINYINT = 2
		,@approvedStatusId TINYINT = 3
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
	DECLARE @DocHandler INT

	CREATE TABLE #tmpPeriodSetting (
		ProcessId TINYINT
		,FromDate DATE
		,ToDate DATE
		)

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@SettingXml

	INSERT INTO #tmpPeriodSetting (
		ProcessId
		,FromDate
		,ToDate
		)
	SELECT ProcessId
		,FromDate
		,ToDate
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (ProcessId TINYINT, FromDate DATE, ToDate DATE) AS data

	EXEC sp_xml_removedocument @DocHandler

	-- write log
	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,[Name] AS '@Name'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
				,(
					SELECT ProcessId AS '@ProcessId'
						,FromDate AS '@FromDate'
						,ToDate AS '@ToDate'
					FROM dbo.JobLevelStandard_PeriodSetting WITH (NOLOCK)
					WHERE PeriodId = @PeriodId
					FOR XML PATH('z')
						,TYPE
					)
			FROM dbo.JobLevelStandard_Period WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE dbo.JobLevelStandard_Period WITH (ROWLOCK, UPDLOCK)
	SET [Name] = @Name
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE PeriodId = @PeriodId

	UPDATE ps
	SET ps.FromDate = tmp.FromDate
		,ps.ToDate = tmp.ToDate
	FROM dbo.JobLevelStandard_PeriodSetting AS ps WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpPeriodSetting AS tmp WITH (NOLOCK) ON ps.ProcessId = tmp.ProcessId
	WHERE ps.PeriodId = @PeriodId

	UPDATE pps
	SET pps.FromDate = tmp.FromDate
		,pps.ToDate = tmp.ToDate
	FROM dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpPeriodSetting AS tmp WITH (NOLOCK) ON pps.ProcessId = tmp.ProcessId
	WHERE pps.PeriodId = @PeriodId

	UPDATE a
	SET a.StatusId = @pendingStatusId
	FROM dbo.JobLevelStandard_Assessment AS a WITH (ROWLOCK, UPDLOCK)
	WHERE a.PeriodId = @PeriodId
		AND a.StatusId IN (@verifiedStatusId, @approvedStatusId)
		AND EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_PeriodSetting AS ps WITH (NOLOCK)
			WHERE a.PeriodId = ps.PeriodId
				AND ps.ProcessId = 1
				AND ps.ToDate >= @today
			)

	-- write log
	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,[Name] AS '@Name'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
				,(
					SELECT ProcessId AS '@ProcessId'
						,FromDate AS '@FromDate'
						,ToDate AS '@ToDate'
					FROM dbo.JobLevelStandard_PeriodSetting WITH (NOLOCK)
					WHERE PeriodId = @PeriodId
					FOR XML PATH('z')
						,TYPE
					)
			FROM dbo.JobLevelStandard_Period WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SpName = 'JobLevelStandard_Period_Update'
		,@ObjectId = @PeriodId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@ObjectIds = NULL

	DROP TABLE #tmpPeriodSetting
END