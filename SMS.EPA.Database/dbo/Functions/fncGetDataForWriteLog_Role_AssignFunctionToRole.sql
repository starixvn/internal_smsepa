/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Role_AssignFunctionToRole    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Role_AssignFunctionToRole    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Role_AssignFunctionToRole    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Role_AssignFunctionToRole    Script Date: 4/6/2016 11:27:50 AM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Role_AssignFunctionToRole] (@ObjectId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
	DECLARE @Functions NVARCHAR(MAX)

	SET @Functions = ''

	SELECT @Functions = @Functions + '<func><id>' + CAST([FunctionId] AS NVARCHAR(10)) + '</id><r>' + CAST([IsRead] AS NVARCHAR) + '</r><h>' + CAST([IsHost] AS NVARCHAR) + '</h></func>'
	FROM [dbo].[RoleFunction] WITH (NOLOCK)
	WHERE [RoleId] = @ObjectId

	SELECT @Data = CONCAT ('<r><id>', CAST(@ObjectId AS NVARCHAR(10)), '</id><funcs>', @Functions, '</funcs></r>')

	RETURN @Data
END