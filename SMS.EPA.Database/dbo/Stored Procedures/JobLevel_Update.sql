CREATE PROCEDURE [dbo].[JobLevel_Update] @JobLevelId INT
	,@Name NVARCHAR(150)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210521@Lilac.Nguyen
Task: Update Job Level.
DB: SMS EPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.JobLevel WITH (NOLOCK)
			WHERE [Name] = @Name
				AND JobLevelId <> @JobLevelId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The Job Level has already exists'

		RETURN
	END

	UPDATE dbo.JobLevel WITH (ROWLOCK, UPDLOCK)
	SET [Name] = @Name
		,ModifiedDate = GETDATE()
	WHERE JobLevelId = @JobLevelId
END