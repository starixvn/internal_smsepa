
CREATE PROCEDURE [dbo].[JobLevelStandard_JobLevelSetting_Update] @SectionIds VARCHAR(MAX)
	,@SettingXml VARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2022-08-10@Tim.Trinh
Task: edit job level setting
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
	@SettingXml: <Root><Row JobLevelId='' MinWeight='' MaxWeight='' /></Root>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @logData VARCHAR(MAX)
		,@logDataOld VARCHAR(MAX)
		,@xml XML
	DECLARE @DocHandler INT

	CREATE TABLE #tmpSection (SectionId INT)

	CREATE TABLE #tmpSetting (
		JobLevelId INT
		,MinWeight DECIMAL(5, 2) NULL
		,MaxWeight DECIMAL(5, 2) NULL
		)

	INSERT INTO #tmpSection (SectionId)
	SELECT val
	FROM dbo.fnc_strSplit(@SectionIds, ',')

	-- write log
	SET @xml = (
			SELECT jls.SectionId AS '@SectionId'
				,jls.JobLevelId AS '@JobLevelId'
				,MinWeight AS '@MinWeight'
				,MaxWeight AS '@MaxWeight'
			FROM dbo.JobLevelStandard_JobLevelSetting AS jls WITH (NOLOCK)
				INNER JOIN #tmpSection AS tmp WITH (NOLOCK) ON jls.SectionId = tmp.SectionId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@SettingXml

	INSERT INTO #tmpSetting (
		JobLevelId
		,MinWeight
		,MaxWeight
		)
	SELECT JobLevelId
		,CASE 
			WHEN ISNUMERIC(MinWeight) = 1 THEN MinWeight
			ELSE NULL
			END
		,CASE 
			WHEN ISNUMERIC(MaxWeight) = 1 THEN MaxWeight
			ELSE NULL
			END
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (JobLevelId INT, MinWeight VARCHAR(MAX), MaxWeight VARCHAR(MAX)) AS data
	WHERE ISNUMERIC(MinWeight) = 1
		OR ISNUMERIC(MaxWeight) = 1

	EXEC sp_xml_removedocument @DocHandler

	DELETE jls
	FROM dbo.JobLevelStandard_JobLevelSetting AS jls WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpSection AS tmp WITH (NOLOCK) ON jls.SectionId = tmp.SectionId
	WHERE NOT EXISTS (
			SELECT 1
			FROM #tmpSetting AS tmp WITH (NOLOCK)
			WHERE jls.JobLevelId = tmp.JobLevelId
			)

	UPDATE jls
	SET jls.MinWeight = tmp.MinWeight
		,jls.MaxWeight = tmp.MaxWeight
	FROM dbo.JobLevelStandard_JobLevelSetting AS jls WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpSection AS section WITH (NOLOCK) ON jls.SectionId = section.SectionId
		INNER JOIN #tmpSetting AS tmp WITH (NOLOCK) ON jls.JobLevelId = tmp.JobLevelId
	WHERE ISNULL(jls.MinWeight, 0) != ISNULL(tmp.MinWeight, 0)
		OR ISNULL(jls.MaxWeight, 0) != ISNULL(tmp.MaxWeight, 0)

	INSERT INTO dbo.JobLevelStandard_JobLevelSetting (
		SectionId
		,JobLevelId
		,MinWeight
		,MaxWeight
		)
	SELECT section.SectionId
		,tmp.JobLevelId
		,tmp.MinWeight
		,tmp.MaxWeight
	FROM #tmpSection AS section WITH (NOLOCK)
		,#tmpSetting AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_JobLevelSetting AS jls WITH (NOLOCK)
			WHERE jls.SectionId = section.SectionId
				AND jls.JobLevelId = tmp.JobLevelId
			)

	-- write log
	SET @xml = (
			SELECT jls.SectionId AS '@SectionId'
				,jls.JobLevelId AS '@JobLevelId'
				,MinWeight AS '@MinWeight'
				,MaxWeight AS '@MaxWeight'
			FROM dbo.JobLevelStandard_JobLevelSetting AS jls WITH (NOLOCK)
				INNER JOIN #tmpSection AS tmp WITH (NOLOCK) ON jls.SectionId = tmp.SectionId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SpName = 'JobLevelStandard_JobLevelSetting_Update'
		,@ObjectId = NULL
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@ObjectIds = @SectionIds

	DROP TABLE #tmpSection

	DROP TABLE #tmpSetting
END