
CREATE PROCEDURE [dbo].[JobDescription_InsertGroupItem] @GroupId INT = NULL
	,@EmployeeId INT = NULL
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

	IF @EmployeeId IS NOT NULL
	BEGIN
		SELECT @GroupId = emp.GroupId
		FROM dbo.Employee AS emp WITH (NOLOCK)
		WHERE emp.EmployeeId = @EmployeeId
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.JobDescription_GroupItem WITH (NOLOCK)
			WHERE GroupId = @GroupId AND [Name] = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = @Name + ' has already existed.'

		RETURN
	END

	INSERT INTO dbo.JobDescription_GroupItem (GroupId, [Name], IsActive)
	VALUES (@GroupId, @Name, 1)
END