
CREATE PROCEDURE [dbo].[KPIGroup_Update] @KPIGroupId INT
	,@Name NVARCHAR(500)
	,@Description NVARCHAR(MAX) = ''
	,@IsActive BIT = 1
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150709@Tim.Trinh
Task: Update KPI Group.
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

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.KPIGroup WITH (NOLOCK)
			WHERE KPIGroupId = @KPIGroupId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Group ''' + @Name + ''' does not exists.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.KPIGroup WITH (NOLOCK)
			WHERE [Name] = @Name
				AND KPIGroupId <> @KPIGroupId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Group ''' + @Name + ''' has already existed.'

		RETURN
	END

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog] @ObjectId = @KPIGroupId
		,@ObjectType = 2

	BEGIN TRY
		BEGIN TRANSACTION;

		DECLARE @LogNo VARCHAR(20)
			,@ModifiedDate DATETIME

		SET @LogNo = CONVERT(VARCHAR(8), GETDATE(), 112) + REPLACE(CONVERT(VARCHAR(12), GETDATE(), 114), ':', '')
		SET @ModifiedDate = GETDATE()

		UPDATE dbo.KPIGroup WITH (ROWLOCK, UPDLOCK)
		SET NAME = @Name
			,LogNo = @LogNo
			,[Description] = @Description
			,[IsActive] = @IsActive
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = @ModifiedDate
		WHERE KPIGroupId = @KPIGroupId

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
			,@ModifiedBy
			,@ModifiedDate
			)

		EXEC @Data = [dbo].[fncGetDataForWriteLog] @ObjectId = @KPIGroupId
			,@ObjectType = 2

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'KPIGroup_Update'
			,@ObjectId = @KPIGroupId
			,@ObjectTypeId = 2
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update KPI Group unsuccessfully.'
	END CATCH
END