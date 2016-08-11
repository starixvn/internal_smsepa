CREATE PROCEDURE [dbo].[KPIGroup_Insert] @KPIGroupId INT OUTPUT
	,@Name NVARCHAR(500)
	,@Description NVARCHAR(MAX) = ''
	,@IsActive BIT = 1
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150709@Tim.Trinh
Task: Insert KPI Group.
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

	IF EXISTS (
			SELECT 1
			FROM dbo.KPIGroup WITH (NOLOCK)
			WHERE [Name] = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Group ''' + @Name + ''' has already existed.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION;

		DECLARE @LogNo VARCHAR(20)
			,@CreatedDate DATETIME

		SET @LogNo = CONVERT(VARCHAR(8), GETDATE(), 112) + REPLACE(CONVERT(VARCHAR(12), GETDATE(), 114), ':', '')
		SET @CreatedDate = GETDATE()

		INSERT INTO dbo.KPIGroup (
			[Name]
			,[Description]
			,[LogNo]
			,[IsActive]
			,[CreatedBy]
			,[CreatedDate]
			)
		VALUES (
			@Name
			,@Description
			,@LogNo
			,@IsActive
			,@CreatedBy
			,@CreatedDate
			)

		SELECT @KPIGroupId = SCOPE_IDENTITY();

		INSERT INTO dbo.KPIGroupHistory (
			[KPIGroupId]
			,[LogNo]
			,[Name]
			,[Description]
			,[IsActive]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		VALUES (
			@KPIGroupId
			,@LogNo
			,@Name
			,@Description
			,@IsActive
			,@CreatedBy
			,@CreatedDate
			)

		DECLARE @Data NVARCHAR(MAX)

		EXEC @Data = [dbo].[fncGetDataForWriteLog] @ObjectId = @KPIGroupId
			,@ObjectType = 2

		EXEC [dbo].[LogDetail_Insert] @ActorId = @CreatedBy
			,@SpName = 'KPIGroup_Insert'
			,@ObjectId = @KPIGroupId
			,@ObjectTypeId = 2
			,@Data = @Data
			,@OldData = NULL
			,@ObjectIds = NULL

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert KPI Group unsuccessfully.'
	END CATCH
END