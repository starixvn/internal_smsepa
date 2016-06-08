/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Employee_ChangePassWord    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Employee_ChangePassWord    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Employee_ChangePassWord    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Employee_ChangePassWord    Script Date: 4/6/2016 11:27:49 AM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Employee_ChangePassWord] (@EmployeeId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)

	SELECT @Data = '<r><eid>' + CAST(@EmployeeId AS NVARCHAR(20)) + '</eid><pa>' + [Password] + '</pa></r>'
	FROM [dbo].[Employee] WITH (NOLOCK)
	WHERE [EmployeeId] = @EmployeeId

	RETURN @Data
END