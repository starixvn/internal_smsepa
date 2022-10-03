
CREATE PROCEDURE [dbo].[JobLevelStandard_Period_Insert] @PeriodId INT OUTPUT
	,@Name NVARCHAR(200)
	,@IsActive BIT = 1
	,@SettingXml VARCHAR(MAX)
	,@CreatedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2022-08-10@Tim.Trinh
Task: insert period
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
			WHERE [Name] = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Period ''' + @Name + ''' has already existed.'

		RETURN
	END

	INSERT INTO dbo.JobLevelStandard_Period (
		[Name]
		,IsActive
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@Name
		,@IsActive
		,@CreatedBy
		,GETDATE()
		)

	SET @PeriodId = SCOPE_IDENTITY()

	DECLARE @DocHandler INT

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@SettingXml

	INSERT INTO dbo.JobLevelStandard_PeriodSetting (
		PeriodId
		,ProcessId
		,FromDate
		,ToDate
		)
	SELECT @PeriodId
		,ProcessId
		,FromDate
		,ToDate
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (ProcessId TINYINT, FromDate DATE, ToDate DATE) AS data

	EXEC sp_xml_removedocument @DocHandler

	INSERT INTO dbo.JobLevelStandard_PeriodJobLevelSetting (
		PeriodId
		,SectionId
		,JobLevelId
		,MinWeight
		,MaxWeight
		)
	SELECT @PeriodId
		,SectionId
		,JobLevelId
		,MinWeight
		,MaxWeight
	FROM dbo.JobLevelStandard_JobLevelSetting WITH (ROWLOCK)

	-- write log
	DECLARE @logData NVARCHAR(MAX)
		,@xml XML

	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,[Name] AS '@Name'
				,IsActive AS '@IsActive'
				,CreatedBy AS '@CreatedBy'
				,CreatedDate AS '@CreatedDate'
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

	EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
		,@SpName = 'JobLevelStandard_Period_Insert'
		,@ObjectId = @PeriodId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@ObjectIds = NULL
END