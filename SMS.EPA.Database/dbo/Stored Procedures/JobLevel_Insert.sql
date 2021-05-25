CREATE PROCEDURE [dbo].[JobLevel_Insert]
	@Name nvarchar(150)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210521@Lilac.Nguyen
Task: Insert Job Level.
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
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Job Level ' + @Name + ' has already exists'

		RETURN
	END

	DECLARE @JobLevelId INT

	SELECT @JobLevelId = MIN(JobLevelId) FROM JobLevel WITH (NOLOCK)

	INSERT INTO dbo.JobLevel(
		JobLevelId
		,[Name]
		,[IsDeleted]
		,[ModifiedDate]
		)
	VALUES (
		CASE @JobLevelId
			WHEN 1 THEN -1
			WHEN NULL THEN -1
			ELSE @JobLevelId -1
			END
		,@Name
		,0
		,GETDATE()
		)
END