/****** Object:  Stored Procedure dbo.EpaEmployee_Update    Script Date: 5/16/2016 2:20:08 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_Update    Script Date: 5/16/2016 2:15:30 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_Update    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_Update    Script Date: 4/6/2016 11:27:44 AM ******/

CREATE PROCEDURE [dbo].[EpaEmployee_Update] @EmployeeId INT
	,@NickName NVARCHAR(20)
	,@FullName NVARCHAR(50) = ''
	,@GroupId INT = ''
	,@JobTitleId INT = ''
	,@DomainUsername NVARCHAR(50)
	,@Enabled BIT = 1
	,@IsLocal BIT = 1
	,@ModifiedBy INT = ''
	,@Email VARCHAR(50) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Update new user.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[Employee] WITH (NOLOCK)
			WHERE [EmployeeId] <> @EmployeeId
				AND [DomainUsername] = @DomainUsername
			)
		--AND [Enabled] = 1
		--AND [IsDeleted] = 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = 'Username ''' + REPLACE(@DomainUsername, 'sms\', '') + ''' has already existed. Please try again.'

		RETURN
	END

	IF EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[Employee] WITH (NOLOCK)
			WHERE [EmployeeId] <> @EmployeeId
				AND [NickName] = @NickName
			)
		--AND [Enabled] = 1
		--AND [IsDeleted] = 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = 'NickName ''' + @NickName + ''' has already existed. Please try again.'

		RETURN
	END

	DECLARE @Data NVARCHAR(MAX)
	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog] @ObjectId = @EmployeeId, @ObjectType = 4

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE [dbo].[Employee] WITH (ROWLOCK, UPDLOCK)
		SET [DomainUsername] = @DomainUsername
			,[NickName] = @NickName
			,[FullName] = @FullName
			,[GroupId] = @GroupId
			,[Enabled] = @Enabled
			,[ModifiedBy] = @ModifiedBy
			,[ModifiedDate] = GETDATE()
			,[JobTitle] = @JobTitleId
			,[IsLocal] = @IsLocal
			,[Email] = @Email
		WHERE [EmployeeId] = @EmployeeId

		COMMIT TRANSACTION

		EXEC @Data = [dbo].[fncGetDataForWriteLog] @ObjectId = @EmployeeId, @ObjectType = 4
		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy, @SpName = 'EpaEmployee_Update', @ObjectId = @EmployeeId, @ObjectTypeId = 4, @Data = @Data, @OldData = @OldData, @ObjectIds = NULL

	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Employee unsuccessfully.'
	END CATCH
END