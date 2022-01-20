
CREATE PROCEDURE [dbo].[Employee_UpdateUserId] @EmployeeId INT
	,@UserId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20220111@Tim.Trinh
Task: update userid
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @xml XML
		,@logData NVARCHAR(MAX)
		,@logOldData NVARCHAR(MAX)

	SET @xml = (
			SELECT EmployeeId AS '@EmployeeId'
				,UserId AS '@UserId'
			FROM dbo.Employee WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
			FOR XML PATH('Row')
				,TYPE
			)
	SET @logOldData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	UPDATE dbo.Employee WITH (ROWLOCK)
	SET UserId = @UserId
	WHERE EmployeeId = @EmployeeId

	SET @xml = (
			SELECT EmployeeId AS '@EmpId'
				,UserId AS '@UserId'
			FROM dbo.Employee WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
			FOR XML PATH('Row')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
END