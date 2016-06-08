
---------------------------------------------------------------------
CREATE VIEW [dbo].[HierarchyDepartment]
AS
WITH Hierarchy (
	ParentId
	,DepartmentId
	,[Level]
	,ParentIds
	)
AS (
	--  Anchor
	SELECT dept.ParentId
		,dept.DepartmentId
		,0 AS [Level]
		,CAST('' AS NVARCHAR)
	FROM dbo.Department AS dept WITH (NOLOCK)
	WHERE dept.IsActive = 1
		AND dept.ParentId IS NULL
	
	UNION ALL
	--  Recursive query
	SELECT dept.ParentId
		,dept.DepartmentId
		,h.[Level] + 1
		,CAST(CASE 
				WHEN h.ParentIds = '' THEN (CAST(dept.ParentId AS NVARCHAR))
				ELSE (h.ParentIds + ',' + CAST(dept.ParentId AS NVARCHAR))
				END AS NVARCHAR)
	FROM dbo.Department AS dept WITH (NOLOCK)
		INNER JOIN Hierarchy h ON dept.ParentId = h.DepartmentId
	WHERE dept.IsActive = 1
	)
SELECT ParentId
	,DepartmentId
	,[Level]
	,ParentIds
FROM Hierarchy
