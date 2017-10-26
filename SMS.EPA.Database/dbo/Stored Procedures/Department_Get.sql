
CREATE PROCEDURE [dbo].[Department_Get] @ParentId INT = NULL
	,@IsActive BIT = 1
AS
/*
Created: 20150721@Tim.Trinh
Task: Get departments by departmentid.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT dept.DepartmentId
		,dept.[Code]
		,dept.[Name]
		,dept.ParentId
		,dept.IsActive
		,0 AS IsGroup
		,dept.ManagerId
		,mg.FullName
		,mg.NickName
	FROM dbo.Department AS dept WITH (NOLOCK)
		LEFT JOIN dbo.Employee AS mg WITH (NOLOCK) ON dept.[ManagerId] = mg.EmployeeId
	WHERE (
			@ParentId IS NULL
			OR dept.ParentId = @ParentId
			)
		AND ISNULL(@IsActive, dept.IsActive) = dept.IsActive
	
	UNION ALL
	SELECT (- g.GroupId) AS DepartmentId
		,g.Code
		,g.[Name]
		,g.DepartmentId AS ParentId
		,(CASE g.IsDeleted
			WHEN 0 THEN 1
			ELSE 0 END
			) AS IsActive
		,1 AS IsGroup
		,g.LeaderId AS [ManagerId]
		,ld.FullName
		,ld.NickName
	FROM dbo.[Group] AS g WITH (NOLOCK)
		LEFT JOIN dbo.Employee AS ld WITH (NOLOCK) ON g.LeaderId = ld.EmployeeId
	WHERE g.IsDeleted = 0
END