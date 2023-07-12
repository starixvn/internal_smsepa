

CREATE PROCEDURE [dbo].[JobLevelStandard_Template_Update] @TemplateId INT
	,@Name NVARCHAR(200)
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20220816@Tim.Trinh
Task: update template
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
				AND TemplateId != @TemplateId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'''' + @Name + ''' exists.'

		RETURN
	END

	DECLARE @logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	-- write log
	SET @xml = (
			SELECT [Name] AS '@Name'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.JobLevelStandard_Template WITH (NOLOCK)
			WHERE TemplateId = @TemplateId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE dbo.JobLevelStandard_Template WITH (ROWLOCK, UPDLOCK)
	SET [Name] = @Name
		,IsActive = @IsActive
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE TemplateId = @TemplateId

	-- write log
	SET @xml = (
			SELECT [Name] AS '@Name'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.JobLevelStandard_Template WITH (NOLOCK)
			WHERE TemplateId = @TemplateId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SpName = 'JobLevelStandard_Template_Update'
		,@ObjectId = @TemplateId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@ObjectIds = NULL
END