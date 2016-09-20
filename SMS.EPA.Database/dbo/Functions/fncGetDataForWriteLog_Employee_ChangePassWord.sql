
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