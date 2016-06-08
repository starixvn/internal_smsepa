/****** Object:  Stored Procedure dbo.EpaEmployeeIpAddress_Insert    Script Date: 5/16/2016 2:20:07 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployeeIpAddress_Insert    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployeeIpAddress_Insert    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployeeIpAddress_Insert    Script Date: 4/6/2016 11:27:44 AM ******/

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