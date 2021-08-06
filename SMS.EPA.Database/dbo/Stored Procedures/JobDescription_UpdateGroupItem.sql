
CREATE PROCEDURE [dbo].[JobDescription_UpdateGroupItem] @GroupId INT = NULL
	,@EmployeeId INT = NULL
	,@GroupItemId INT
	,@Name NVARCHAR(500)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210429@Lilac.Nguyen
Task: Update Job Description.
DB: SMSEPA
Original

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF @EmployeeId IS NOT NULL
	BEGIN
		SELECT @GroupId = emp.GroupId
		FROM dbo.Employee AS emp WITH (NOLOCK)
		WHERE emp.EmployeeId = @EmployeeId
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.JobDescription_GroupItem WITH (NOLOCK)
			WHERE [Name] = @Name
				AND GroupId = @GroupId
				AND JobDescription_GroupItemId <> @GroupItemId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = @Name + ' has already existed.'

		RETURN
	END

	UPDATE dbo.JobDescription_GroupItem WITH (ROWLOCK, UPDLOCK)
	SET [Name] = @Name
	WHERE JobDescription_GroupItemId = @GroupItemId
END