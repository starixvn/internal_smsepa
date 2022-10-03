
CREATE PROCEDURE [dbo].[JobLevelStandard_KPI_Update] @KPIId INT
	,@Name NVARCHAR(200)
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20220816@Tim.Trinh
Task: update kpi
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_KPI WITH (NOLOCK)
			WHERE [Name] = @Name
				AND KPIId != @KPIId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'KPI ''' + @Name + ''' has already existed.'

		RETURN
	END

	DECLARE @logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
	DECLARE @LogNo VARCHAR(20) = CONVERT(VARCHAR(8), GETDATE(), 112) + REPLACE(CONVERT(VARCHAR(12), GETDATE(), 114), ':', '')
		,@ModifiedDate DATETIME = GETDATE()

	-- write log
	SET @xml = (
			SELECT [Name] AS '@Name'
				,LogNo AS '@LogNo'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.JobLevelStandard_KPI WITH (NOLOCK)
			WHERE KPIId = @KPIId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE dbo.JobLevelStandard_KPI WITH (ROWLOCK, UPDLOCK)
	SET [Name] = @Name
		,LogNo = @LogNo
		,IsActive = @IsActive
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @ModifiedDate
	WHERE KPIId = @KPIId

	INSERT INTO dbo.JobLevelStandard_KPILog (
		KPIId
		,LogNo
		,[Name]
		,IsActive
		)
	VALUES (
		@KPIId
		,@LogNo
		,@Name
		,@IsActive
		)

	-- write log
	SET @xml = (
			SELECT [Name] AS '@Name'
				,LogNo AS '@LogNo'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.JobLevelStandard_KPI WITH (NOLOCK)
			WHERE KPIId = @KPIId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SpName = 'JobLevelStandard_KPI_Update'
		,@ObjectId = @KPIId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@ObjectIds = NULL
END