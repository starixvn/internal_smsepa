
CREATE PROCEDURE [dbo].[JobLevelStandard_PeriodJobLevelSetting_CopyFromDefault] @PeriodId INT
	,@SectionIds VARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2022-08-10@Tim.Trinh
Task: copy job level setting
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @logData VARCHAR(MAX)
		,@logDataOld VARCHAR(MAX)
		,@xml XML
		,@modifiedDate DATETIME = GETDATE()

	CREATE TABLE #tmpSection (SectionId INT)

	INSERT INTO #tmpSection (SectionId)
	SELECT val
	FROM dbo.fnc_strSplit(@SectionIds, ',')

	-- write log
	SET @xml = (
			SELECT SectionId AS '@SectionId'
				,JobLevelId AS '@JobLevelId'
				,MinWeight AS '@MinWeight'
				,MaxWeight AS '@MaxWeight'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.JobLevelStandard_PeriodJobLevelSetting AS jls WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
				AND EXISTS (
					SELECT 1
					FROM #tmpSection AS tmp WITH (NOLOCK)
					WHERE jls.SectionId = tmp.SectionId
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	DELETE jls
	FROM dbo.JobLevelStandard_PeriodJobLevelSetting AS jls WITH (ROWLOCK, UPDLOCK)
	WHERE jls.PeriodId = @PeriodId
		AND EXISTS (
			SELECT 1
			FROM #tmpSection AS tmp WITH (NOLOCK)
			WHERE jls.SectionId = tmp.SectionId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_JobLevelSetting AS df WITH (NOLOCK)
			WHERE jls.SectionId = df.SectionId
				AND jls.JobLevelId = df.JobLevelId
			)

	UPDATE jls
	SET jls.MinWeight = df.MinWeight
		,jls.MaxWeight = df.MaxWeight
		,jls.ModifiedBy = @ModifiedBy
		,jls.ModifiedDate = @modifiedDate
	FROM dbo.JobLevelStandard_PeriodJobLevelSetting AS jls WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.JobLevelStandard_JobLevelSetting AS df WITH (NOLOCK) ON jls.SectionId = df.SectionId
		AND jls.JobLevelId = df.JobLevelId
	WHERE jls.PeriodId = @PeriodId
		AND EXISTS (
			SELECT 1
			FROM #tmpSection AS tmp WITH (NOLOCK)
			WHERE jls.SectionId = tmp.SectionId
			)
		AND (
			ISNULL(jls.MinWeight, 0) != ISNULL(df.MinWeight, 0)
			OR ISNULL(jls.MaxWeight, 0) != ISNULL(df.MaxWeight, 0)
			)

	INSERT INTO dbo.JobLevelStandard_PeriodJobLevelSetting (
		PeriodId
		,SectionId
		,JobLevelId
		,MinWeight
		,MaxWeight
		,CreatedBy
		,CreatedDate
		)
	SELECT @PeriodId
		,SectionId
		,JobLevelId
		,MinWeight
		,MaxWeight
		,@ModifiedBy
		,@modifiedDate
	FROM dbo.JobLevelStandard_JobLevelSetting AS df WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM #tmpSection AS tmp WITH (NOLOCK)
			WHERE df.SectionId = tmp.SectionId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_PeriodJobLevelSetting AS jsl WITH (NOLOCK)
			WHERE jsl.PeriodId = @PeriodId
				AND jsl.SectionId = df.SectionId
				AND jsl.JobLevelId = df.JobLevelId
			)

	-- write log
	SET @xml = (
			SELECT SectionId AS '@SectionId'
				,JobLevelId AS '@JobLevelId'
				,MinWeight AS '@MinWeight'
				,MaxWeight AS '@MaxWeight'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.JobLevelStandard_PeriodJobLevelSetting AS jls WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
				AND EXISTS (
					SELECT 1
					FROM #tmpSection AS tmp WITH (NOLOCK)
					WHERE jls.SectionId = tmp.SectionId
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SpName = 'JobLevelStandard_PeriodJobLevelSetting_CopyFromDefault'
		,@ObjectId = @PeriodId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@ObjectIds = NULL

	DROP TABLE #tmpSection
END