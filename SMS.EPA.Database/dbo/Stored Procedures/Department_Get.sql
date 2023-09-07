
CREATE PROCEDURE [dbo].[Department_Get] @IsActive BIT = 1
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

	SELECT u.UnitId AS DepartmentId
		,u.[Code]
		,u.[Name]
		,NULL AS ParentId
		,u.IsActive
		,0 AS IsGroup
	FROM dbo.Unit AS u WITH (NOLOCK)
	WHERE @IsActive IS NULL
		OR u.IsActive = @IsActive
	
	UNION ALL
	SELECT dept.DepartmentId
		,dept.[Code]
		,dept.[Name]
		,dept.UnitId AS ParentId
		,dept.IsActive
		,0 AS IsGroup
	FROM dbo.Department_New AS dept WITH (NOLOCK)
	WHERE @IsActive IS NULL
		OR dept.IsActive = @IsActive
	
	UNION ALL
	SELECT sec.SectionId AS DepartmentId
		,sec.[Code]
		,sec.[Name]
		,sec.DepartmentId AS ParentId
		,sec.IsActive
		,0 AS IsGroup
	FROM dbo.Section AS sec WITH (NOLOCK)
	WHERE @IsActive IS NULL
		OR sec.IsActive = @IsActive
	
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
	FROM dbo.[Group] AS g WITH (NOLOCK)
	WHERE @IsActive IS NULL
		OR g.IsDeleted = ~ @IsActive
END