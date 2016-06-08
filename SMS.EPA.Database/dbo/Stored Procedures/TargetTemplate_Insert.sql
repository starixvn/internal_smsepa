/****** Object:  Stored Procedure dbo.TargetTemplate_Insert    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_Insert    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_Insert    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_Insert    Script Date: 4/6/2016 11:27:49 AM ******/

CREATE PROCEDURE [dbo].[TargetTemplate_Insert] 
	@Name NVARCHAR(200)
	,@Description NVARCHAR(MAX) = ''
	,@TargetTypeId TINYINT = NULL
	,@ScoreRatingTypeId TINYINT = NULL
	,@RatingTypeId TINYINT
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140404@Quinn.Vo
Task: Insert Target template.
DB: NexEPA
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

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.TargetTemplate WITH (NOLOCK)
				WHERE [Name] = @Name
				)
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

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert Target template failed.'
	END
	ELSE
	BEGIN
		DECLARE @Data NVARCHAR(MAX)

		EXEC @Data = [dbo].[fncGetDataForWriteLog] @ObjectId = @TargetTemplateId , @ObjectType = 3
		EXEC [dbo].[LogDetail_Insert] @ActorId = @CreatedBy, @SpName = 'TargetTemplate_Insert', @ObjectId = @TargetTemplateId, @ObjectTypeId = 3, @Data = @Data, @OldData = NULL, @ObjectIds = NULL

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END