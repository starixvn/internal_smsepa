

CREATE PROCEDURE [dbo].[JobLevelStandard_KPI_Insert] @KPIId INT OUTPUT
	,@Name NVARCHAR(200)
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20220816@Tim.Trinh
Task: insert kpi
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
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'''' + @Name + ''' exists.'

		RETURN
	END

	DECLARE @LogNo VARCHAR(20) = CONVERT(VARCHAR(8), GETDATE(), 112) + REPLACE(CONVERT(VARCHAR(12), GETDATE(), 114), ':', '')
	DECLARE @logData NVARCHAR(MAX)
		,@xml XML

	INSERT INTO dbo.JobLevelStandard_KPI (
		[Name]
		,LogNo
		,IsActive
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@Name
		,@LogNo
		,1
		,@CreatedBy
		,GETDATE()
		)

	SELECT @KPIId = SCOPE_IDENTITY();

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
		,1
		)

	-- write log
	SET @xml = (
			SELECT [Name] AS '@Name'
				,LogNo AS '@LogNo'
				,IsActive AS '@IsActive'
				,CreatedBy AS '@CreatedBy'
				,CreatedDate AS '@CreatedDate'
			FROM dbo.JobLevelStandard_KPI WITH (NOLOCK)
			WHERE KPIId = @KPIId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
		,@SpName = 'JobLevelStandrd_KPI_Insert'
		,@ObjectId = @KPIId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@ObjectIds = NULL
END