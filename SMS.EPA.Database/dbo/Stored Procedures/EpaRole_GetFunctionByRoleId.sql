/****** Object:  Stored Procedure dbo.EpaRole_GetFunctionByRoleId    Script Date: 5/16/2016 2:20:09 PM ******/
/****** Object:  Stored Procedure dbo.EpaRole_GetFunctionByRoleId    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.EpaRole_GetFunctionByRoleId    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaRole_GetFunctionByRoleId    Script Date: 4/6/2016 11:27:46 AM ******/

CREATE PROCEDURE [dbo].[EpaRole_GetFunctionByRoleId] --1
	@RoleId INT
AS
/*
Created: 20150609@Josh.Phan
Task: Get function of role by role Id.
DB: SPM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT fk.[FunctionId]
		,fk.[FunctionKey]
		,fk.[Name] AS FunctionName
		,fk.[ParentFunctionId]
		,fk.[IsReadOnly]
		,rf.[IsRead]
		,rf.[IsHost]
	FROM dbo.[FunctionKey] AS fk WITH (NOLOCK)
		LEFT JOIN (
			SELECT [FunctionId]
				,[IsRead]
				,[IsHost]
			FROM dbo.[RoleFunction] WITH (NOLOCK)
			WHERE [RoleId] = @RoleId) AS rf ON fk.[FunctionId] = rf.[FunctionId]
	WHERE fk.[IsActive] = 1
END