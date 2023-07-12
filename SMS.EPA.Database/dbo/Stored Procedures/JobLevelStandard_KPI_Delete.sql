
CREATE PROCEDURE [dbo].[JobLevelStandard_KPI_Delete] @KPIId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2023-04-20@Frankie.Nguyen
Task: Delete JobLevelStandard KPI.
DB: NexEPA
Original: 

Revisions:  
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_TemplateKPI WITH (NOLOCK)
			WHERE KPIId = @KPIId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'KPI already used in template.'

		RETURN
	END

	DECLARE @OldData NVARCHAR(MAX)
		,@xml XML
		,@LogNo VARCHAR(20)

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
	SET @OldData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @LogNo = CONVERT(VARCHAR(8), GETDATE(), 112) + REPLACE(CONVERT(VARCHAR(12), GETDATE(), 114), ':', '')

	INSERT INTO dbo.JobLevelStandard_KPILog (
		KPIId
		,LogNo
		,[Name]
		,IsActive
		)
	SELECT @KPIId
		,@LogNo
		,[Name]
		,IsActive
	FROM dbo.JobLevelStandard_KPI WITH (NOLOCK)
	WHERE KPIId = @KPIId

	DELETE
	FROM dbo.JobLevelStandard_KPI WITH (ROWLOCK, UPDLOCK)
	WHERE KPIId = @KPIId

	EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
		,@SpName = 'JobLevelStandardKPI_Delete'
		,@ObjectId = @KPIId
		,@ObjectTypeId = NULL
		,@Data = NULL
		,@OldData = @OldData
		,@ObjectIds = NULL
END