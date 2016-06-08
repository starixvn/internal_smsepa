/****** Object:  Stored Procedure dbo.Menu_Get    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.Menu_Get    Script Date: 5/16/2016 2:15:32 PM ******/
/****** Object:  Stored Procedure dbo.Menu_Get    Script Date: 5/16/2016 1:59:44 PM ******/

CREATE PROCEDURE [dbo].[Menu_Get]
	@EmployeeId INT = NULL
AS
/*
Created: 20150731@Tim.Trinh
Task: Get menu by employee Id.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT 
		me.[MenuId]
		,me.[Name] AS MenuName
		,me.[Url] AS MenuLink
		,me.[ParentId]
		,me.[FunctionId]
		,me.DisplayOrder
		,me.IsActive
	FROM [dbo].[EmployeeRole] AS empRo WITH (NOLOCK)
		INNER JOIN [dbo].[RoleFunction] AS roFunc WITH (NOLOCK) ON empRo.[RoleId] = roFunc.[RoleId]
		INNER JOIN [dbo].[Menu] AS me WITH (NOLOCK) ON roFunc.[FunctionId] = me.[FunctionId]
	WHERE me.IsActive = 1
	   AND (ISNULL(@EmployeeId, '') = '' OR empRo.[EmployeeId] = @EmployeeId) 
	   AND roFunc.[IsRead] = 1
END