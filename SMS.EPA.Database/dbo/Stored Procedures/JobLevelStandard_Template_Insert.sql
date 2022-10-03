
CREATE PROCEDURE [dbo].[JobLevelStandard_Template_Insert] @TemplateId INT OUTPUT
	,@Name NVARCHAR(200)
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20220816@Tim.Trinh
Task: insert template
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_Template WITH (NOLOCK)
			WHERE [Name] = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Template ' + @Name + ' has already existed.'

		RETURN
	END

	DECLARE @logData NVARCHAR(MAX)
		,@xml XML

	INSERT INTO dbo.JobLevelStandard_Template (
		[Name]
		,IsActive
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@Name
		,1
		,@CreatedBy
		,GETDATE()
		)

	SET @TemplateId = SCOPE_IDENTITY()

	-- write log
	SET @xml = (
			SELECT [Name] AS '@Name'
				,IsActive AS '@IsActive'
				,CreatedBy AS '@CreatedBy'
				,CreatedDate AS '@CreatedDate'
			FROM dbo.JobLevelStandard_Template WITH (NOLOCK)
			WHERE TemplateId = @TemplateId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
		,@SpName = 'JobLevelStandard_Template_Insert'
		,@ObjectId = @TemplateId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@ObjectIds = NULL
END