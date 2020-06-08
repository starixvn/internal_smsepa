
CREATE PROCEDURE [dbo].[TargetTemplate_Insert] @Name NVARCHAR(200)
	,@Description NVARCHAR(MAX) = ''
	,@TargetTypeId TINYINT
	,@ScoreRatingTypeId TINYINT
	,@RatingTypeId TINYINT
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140404@Quinn.Vo
Task: Insert Target template.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.TargetTemplate WITH (NOLOCK)
			WHERE [Name] = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Target template ' + @Name + ' has already exists '

		RETURN
	END

	DECLARE @TargetTemplateId INT

	INSERT INTO dbo.TargetTemplate (
		[Name]
		,[Description]
		,[TargetTypeId]
		,[ScoreRatingTypeId]
		,[RatingTypeId]
		,IsActive
		,[CreatedBy]
		,[CreatedDate]
		)
	VALUES (
		@Name
		,@Description
		,@TargetTypeId
		,@ScoreRatingTypeId
		,@RatingTypeId
		,1
		,@CreatedBy
		,GETDATE()
		)

	SET @TargetTemplateId = SCOPE_IDENTITY()

	DECLARE @Data NVARCHAR(MAX)

	EXEC @Data = [dbo].[fncGetDataForWriteLog] @ObjectId = @TargetTemplateId
		,@ObjectType = 3

	EXEC [dbo].[LogDetail_Insert] @ActorId = @CreatedBy
		,@SpName = 'TargetTemplate_Insert'
		,@ObjectId = @TargetTemplateId
		,@ObjectTypeId = 3
		,@Data = @Data
		,@OldData = NULL
		,@ObjectIds = NULL
END