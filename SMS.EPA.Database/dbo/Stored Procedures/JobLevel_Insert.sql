
CREATE PROCEDURE [dbo].[JobLevel_Insert] @Name NVARCHAR(150)
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

	SELECT @JobLevelId = MIN(JobLevelId)
	FROM dbo.JobLevel WITH (NOLOCK)

	SET @JobLevelId = ISNULL(@JobLevelId, 0)

	INSERT INTO dbo.JobLevel (
		JobLevelId
		,[Name]
		,IsDeleted
		)
	VALUES (CASE 
			WHEN @JobLevelId >= 0 THEN - 1
			ELSE @JobLevelId - 1
			END
		,@Name
		,0
		)
END