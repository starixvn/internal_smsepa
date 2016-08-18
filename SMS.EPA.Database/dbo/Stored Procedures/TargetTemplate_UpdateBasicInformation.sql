CREATE PROCEDURE [dbo].[TargetTemplate_UpdateBasicInformation] @TargetTemplateId INT
	,@Name VARCHAR(200)
	,@Description NVARCHAR(MAX) = ''
	,@TargetTypeId TINYINT
	,@ScoreRatingTypeId TINYINT
	,@RatingTypeId TINYINT
	,@IsActive TINYINT = 1
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2010404@Quinn.Vo
Task: Update Target Template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.TargetTemplate WITH (NOLOCK)
			WHERE TargetTemplateId = @TargetTemplateId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Target template' + @Name + ' does not exists.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.TargetTemplate WITH (NOLOCK)
			WHERE [Name] = @Name
				AND TargetTemplateId <> @TargetTemplateId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Target template' + @Name + ' has already existed.'

		RETURN
	END

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog] @ObjectId = @TargetTemplateId
		,@ObjectType = 3

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE dbo.TargetTemplate WITH (ROWLOCK, UPDLOCK)
		SET [Name] = @Name
			,[Description] = @Description
			,[TargetTypeId] = @TargetTypeId
			,[ScoreRatingTypeId] = @ScoreRatingTypeId
			,[RatingTypeId] = @RatingTypeId
			,IsActive = @IsActive
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		WHERE TargetTemplateId = @TargetTemplateId

		EXEC @Data = [dbo].[fncGetDataForWriteLog] @ObjectId = @TargetTemplateId
			,@ObjectType = 3

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'TargetTemplate_UpdateBasicInformation'
			,@ObjectId = @TargetTemplateId
			,@ObjectTypeId = 3
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Target template unsuccessfully.'
	END CATCH
END