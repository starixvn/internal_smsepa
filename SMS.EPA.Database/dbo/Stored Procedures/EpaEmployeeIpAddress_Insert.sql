
CREATE PROCEDURE [dbo].[EpaEmployeeIpAddress_Insert] @EmployeeId INT
	,@IPAddress NVARCHAR(50)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20151104@Josh.Phan
Task: Insert ipaddress of employee login.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[EmployeeIpAddress] (
		[EmployeeId]
		,[IpAddress]
		,[CreatedDate]
		)
	SELECT @EmployeeId
		,@IPAddress
		,GETDATE()
END