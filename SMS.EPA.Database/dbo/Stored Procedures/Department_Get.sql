
CREATE PROCEDURE [dbo].[Department_Get] @ParentId INT = NULL
	,@IsActive BIT = NULL
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

	SELECT [DepartmentId]
		,dep.[Code]
		,[Name]
		,[ParentId]
		,[IsActive]
		,0 AS IsGroup
		,[ManagerId]
		,mg.FullName
		,mg.NickName
	FROM dbo.Department AS dep WITH (NOLOCK)
		LEFT JOIN dbo.Employee AS mg WITH (NOLOCK) ON dep.[ManagerId] = mg.EmployeeId
	WHERE (
			@ParentId IS NULL
			OR ParentId = @ParentId
			)
		AND (
			@IsActive IS NULL
			OR IsActive = @IsActive
			)
	
	UNION ALL
	SELECT (- g.GroupId) AS [DepartmentId]
		,g.Code
		,g.NAME
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