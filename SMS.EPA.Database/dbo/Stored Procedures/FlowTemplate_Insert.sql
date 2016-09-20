
CREATE PROCEDURE [dbo].[FlowTemplate_Insert] @FlowTemplateId INT OUTPUT
	,@Name NVARCHAR(200)
	,@Description NVARCHAR(500) = NULL
	,@IsActive BIT
	,@TargetTypeId INT
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150804@Tim.Trinh
Task: Insert Flow Template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	IF EXISTS (
			SELECT 1
			FROM dbo.FlowTemplate WITH (NOLOCK)
			WHERE [Name] = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Flow Template ''' + @Name + ''' has already existed.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.FlowTemplate WITH (NOLOCK)
			WHERE TargetTypeId = @TargetTypeId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'This Target Type has already existed setting.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION

		INSERT INTO dbo.FlowTemplate (
			[Name]
			,TargetTypeId
			,[Description]
			,IsActive
			,CreatedBy
			,CreatedDate
			)
		VALUES (
			@Name
			,@TargetTypeId
			,@Description
			,@IsActive
			,@CreatedBy
			,GETDATE()
			)

		SELECT @FlowTemplateId = SCOPE_IDENTITY();

		DECLARE @Data NVARCHAR(MAX)

		EXEC @Data = dbo.fncGetDataForWriteLog @ObjectId = @FlowTemplateId
			,@ObjectType = 6

		EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
			,@SpName = 'FlowTemplate_Insert'
			,@ObjectId = @FlowTemplateId
			,@ObjectTypeId = 6
			,@Data = @Data
			,@OldData = NULL
			,@ObjectIds = NULL

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert unsuccessfully.'
	END CATCH
END