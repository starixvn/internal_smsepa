
CREATE PROCEDURE [dbo].[JobDescription_InsertItem] @GroupItemId INT
	,@Name NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210426@Tim.Trinh
Task: add job description
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.JobDescription_Item WITH (NOLOCK)
			WHERE JobDescription_GroupItemId = @GroupItemId AND [Name] = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = @Name + ' has already existed.'

		RETURN
	END

	INSERT INTO dbo.JobDescription_Item (JobDescription_GroupItemId, [Name], IsActive)
	VALUES (@GroupItemId, @Name, 1)

END