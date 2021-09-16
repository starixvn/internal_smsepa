CREATE PROCEDURE [dbo].[EpaEmployee_UpdateJobLevel]
	@EmployeeId INT
	,@JobLevelId INT = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210525@Lilac.Nguyen
Task: Update Job Level for employee
DB: SMS EPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.Employee WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = 'User does not exist.'

		RETURN
	END

	UPDATE dbo.Employee WITH (ROWLOCK, UPDLOCK)
	SET JobLevelId = @JobLevelId
	WHERE EmployeeId = @EmployeeId
END

