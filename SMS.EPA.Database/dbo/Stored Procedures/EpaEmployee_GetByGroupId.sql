/****** Object:  Stored Procedure dbo.EpaEmployee_GetByGroupId    Script Date: 5/16/2016 2:20:07 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_GetByGroupId    Script Date: 5/16/2016 2:15:30 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_GetByGroupId    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_GetByGroupId    Script Date: 4/6/2016 11:27:44 AM ******/

CREATE PROCEDURE [dbo].[EpaEmployee_GetByGroupId] @GroupId INT
AS
/*
Created: 20150722@Tim.Trinh
Task: Get employees by group
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.[EmployeeId]
		,e.[Code]
		,[DomainUsername]
		,[FullName]
		,[NickName]
		,[Email]
		,e.HasMonthlyEPA
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE GroupId = @GroupId
		AND e.IsDeleted = 0
		AND e.[Enabled] = 1
	ORDER BY Code
END