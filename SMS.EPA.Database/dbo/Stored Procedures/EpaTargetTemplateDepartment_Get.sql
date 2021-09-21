CREATE PROCEDURE [dbo].[EpaTargetTemplateDepartment_Get] @DepartmentId INT = NULL
	,@IsGroup BIT = NULL
	,@TargetTypeId INT = NULL
	,@ShowAll BIT = 0
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get all departments and their target template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT deptSet.DepartmentId
		,deptSet.Department
		,deptSet.Section
		,deptSet.ParentId
		,deptSet.IsActive
		,deptSet.IsGroup
	FROM (
		SELECT DepartmentId
			,[Name] AS Department
			,NULL AS Section
			,ParentId
			,IsActive
			,0 AS IsGroup
		FROM dbo.Department AS dept WITH (NOLOCK)
		WHERE IsActive = 1
			AND (
				@IsGroup IS NULL
				OR (
					@IsGroup = 0
					AND DepartmentId = @DepartmentId
					)
				)
			AND (
				(
					@ShowAll = 0
					AND dept.DepartmentId IN (
						SELECT DISTINCT tarTemp.DepartmentId
						FROM dbo.TargetTemplateDepartment AS tarTemp WITH (NOLOCK)
							INNER JOIN dbo.Department AS dept1 WITH (NOLOCK) ON tarTemp.DepartmentId = dept1.DepartmentId
							AND tarTemp.IsGroup = 0
						)
					)
				OR @ShowAll = 1
				)
			AND (
				ISNULL(@TargetTypeId, '') = ''
				OR dept.DepartmentId IN (
					SELECT DISTINCT DepartmentId
					FROM dbo.TargetTemplateDepartment AS tarDept WITH (NOLOCK)
						INNER JOIN dbo.TargetTemplate AS tarTemp WITH (NOLOCK) ON tarDept.TargetTemplateId = tarTemp.TargetTemplateId
						INNER JOIN dbo.TargetType AS tar WITH (NOLOCK) ON tarTemp.TargetTypeId = tar.TargetTypeId
					WHERE tar.TargetTypeId = @TargetTypeId
						AND tarDept.IsGroup = 0
					)
				)
		
		UNION
		SELECT gr.GroupId AS DepartmentId
			,dept.[Name] AS Department
			,gr.[Name] AS Section
			,gr.DepartmentId AS ParentId
			,~ [IsDeleted] AS IsActive
			,1 AS IsGroup
		FROM [dbo].[Group] AS gr WITH (NOLOCK)
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		WHERE IsDeleted = 0
			AND (
				@IsGroup IS NULL
				OR (
					@IsGroup = 1
					AND GroupId = @DepartmentId
					)
				)
			AND (
				(
					@ShowAll = 0
					AND gr.GroupId IN (
						SELECT DISTINCT tarTemp.DepartmentId AS GroupId
						FROM dbo.TargetTemplateDepartment AS tarTemp WITH (NOLOCK)
							INNER JOIN dbo.[Group] AS gr1 WITH (NOLOCK) ON tarTemp.DepartmentId = gr1.GroupId
						WHERE tarTemp.IsGroup = 1
						)
					)
				OR @ShowAll = 1
				)
			AND (
				ISNULL(@TargetTypeId, '') = ''
				OR gr.GroupId IN (
					SELECT DISTINCT DepartmentId AS GroupId
					FROM dbo.TargetTemplateDepartment AS tarDept WITH (NOLOCK)
						INNER JOIN dbo.TargetTemplate AS tarTemp WITH (NOLOCK) ON tarDept.TargetTemplateId = tarTemp.TargetTemplateId
						INNER JOIN dbo.TargetType AS tar WITH (NOLOCK) ON tarTemp.TargetTypeId = tar.TargetTypeId
					WHERE tar.TargetTypeId = @TargetTypeId
						AND tarDept.IsGroup = 1
					)
				)
		) AS deptSet

	SELECT DepartmentId
		,IsGroup
		,tarDept.TargetTemplateId
		,tarTemp.[Name]
		,tar.TargetTypeId
		,tar.[Name] AS TargetTypeName
	FROM dbo.TargetTemplateDepartment AS tarDept WITH (NOLOCK)
		INNER JOIN dbo.TargetTemplate AS tarTemp WITH (NOLOCK) ON tarDept.TargetTemplateId = tarTemp.TargetTemplateId
		INNER JOIN dbo.TargetType AS tar WITH (NOLOCK) ON tarTemp.TargetTypeId = tar.TargetTypeId
	ORDER BY DepartmentId ASC
		,tar.TargetTypeId ASC
END