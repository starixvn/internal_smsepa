
CREATE PROCEDURE [dbo].[JobDescription_UpdateItem] @GroupItemId INT
	,@ItemId INT
	,@Name NVARCHAR(500)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210510@Lilac.Nguyen
Task: Update Job Description Item.
DB: SMSEPA
Original

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.JobDescription_Item WITH (NOLOCK)
			WHERE [Name] = @Name
				AND JobDescription_GroupItemId = @GroupItemId
				AND JobDescription_ItemId <> @ItemId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = @Name + ' has already existed.'

		RETURN
	END

	UPDATE dbo.JobDescription_Item WITH (ROWLOCK, UPDLOCK)
	SET [Name] = @Name
	WHERE JobDescription_ItemId = @ItemId
END