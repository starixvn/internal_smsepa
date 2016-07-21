CREATE PROCEDURE [dbo].[EpaEmployee_Insert] @NickName NVARCHAR(20)
	,@FullName NVARCHAR(50) = ''
	,@GroupId INT = ''
	,@JobTitleId INT = ''
	,@DomainUsername NVARCHAR(50)
	,@Password NVARCHAR(100)
	,@Email VARCHAR(50) = NULL
	,@Enabled BIT = 1
	,@IsLocal BIT = 1
	,@CreatedBy INT = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Insert new user.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM [dbo].[Employee] WITH (NOLOCK)
			WHERE [DomainUsername] = @DomainUsername
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = 'Username ''' + REPLACE(@DomainUsername, 'sms\', '') + ''' has already existed. Please try again.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM [dbo].[Employee] WITH (NOLOCK)
			WHERE [NickName] = @NickName
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = 'Nick Name ''' + @NickName + ''' has already existed. Please try again.'

		RETURN
	END

	DECLARE @MinSpecialEmployee INT
		,@NumberSpecialEmployee INT
		,@EmpId INT

	SET @MinSpecialEmployee = 100000
	SET @NumberSpecialEmployee = (
			SELECT COUNT([EmployeeId])
			FROM [dbo].[Employee] WITH (NOLOCK)
			WHERE [IsSpecialUser] = 1
			)

	BEGIN TRY
		BEGIN TRANSACTION

		INSERT INTO [dbo].[Employee] (
			[EmployeeId]
			,[DomainUsername]
			,[Code]
			,[NickName]
			,[FullName]
			,[GroupId]
			,[Phone]
			,[CardNumber]
			,[Enabled]
			,[IsDeleted]
			,[IsSpecialUser]
			,[Password]
			,[JoinedDate]
			,[ModifiedBy]
			,[ModifiedDate]
			,[CreatedBy]
			,[CreatedDate]
			,[JobTitle]
			,[IsLocal]
			,[Email]
			)
		VALUES (
			@MinSpecialEmployee + @NumberSpecialEmployee + 1
			,@DomainUsername
			,''
			,@NickName
			,@FullName
			,@GroupId
			,''
			,''
			,@Enabled
			,0
			,1
			,@Password
			,GETDATE()
			,''
			,''
			,@CreatedBy
			,GETDATE()
			,@JobTitleId
			,@IsLocal
			,@Email
			)

		SELECT @EmpId = @MinSpecialEmployee + @NumberSpecialEmployee + 1

		COMMIT TRANSACTION

		DECLARE @Data NVARCHAR(MAX)

		EXEC @Data = [dbo].[fncGetDataForWriteLog] @ObjectId = @EmpId
			,@ObjectType = 4

		EXEC [dbo].[LogDetail_Insert] @ActorId = @CreatedBy
			,@SpName = 'EpaEmployee_Insert'
			,@ObjectId = @EmpId
			,@ObjectTypeId = 4
			,@Data = @Data
			,@OldData = NULL
			,@ObjectIds = NULL

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert special employee unsuccessfully.'
	END CATCH
END