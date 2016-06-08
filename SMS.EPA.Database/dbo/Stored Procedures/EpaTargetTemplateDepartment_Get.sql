/****** Object:  Stored Procedure dbo.EpaTargetTemplateDepartment_Get    Script Date: 5/16/2016 2:20:09 PM ******/
/****** Object:  Stored Procedure dbo.EpaTargetTemplateDepartment_Get    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.EpaTargetTemplateDepartment_Get    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaTargetTemplateDepartment_Get    Script Date: 4/6/2016 11:27:46 AM ******/

CREATE PROCEDURE [dbo].[EpaTargetTemplateDepartment_Get] @DepartmentId INT = NULL
	,@IsGroup BIT = NULL
	,@TargetTypeId INT = NULL
	,@ShowAll BIT = NULL
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get all departments and their target template.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT deptSet.[DepartmentId]
		,deptSet.[Name]
		,deptSet.[ParentId]
		,deptSet.[IsActive]
		,deptSet.[IsGroup]
	FROM (
		SELECT [DepartmentId]
			,[Name]
			,[ParentId]
			,[IsActive]
			,0 AS IsGroup
		FROM [dbo].[Department]
		WHERE [IsActive] = 1
			AND (
				@IsGroup IS NULL
				OR (
					@IsGroup = 0
					AND [DepartmentId] = @DepartmentId
					)
				)
			AND (
				(
					ISNULL(@ShowAll, '') = ''
					AND [DepartmentId] IN (
						SELECT DISTINCT tarTemp.[DepartmentId]
						FROM [dbo].[TargetTemplateDepartment] AS tarTemp WITH (NOLOCK)
							INNER JOIN [dbo].[Department] AS dept WITH (NOLOCK) ON tarTemp.[DepartmentId] = dept.[DepartmentId]
							AND tarTemp.[IsGroup] = 0
						)
					)
				OR @ShowAll = 1
				)
			AND (
				ISNULL(@TargetTypeId, '') = ''
				OR [DepartmentId] IN (
					SELECT DISTINCT [DepartmentId]
					FROM [dbo].[TargetTemplateDepartment] AS tarDept WITH (NOLOCK)
						INNER JOIN [dbo].[TargetTemplate] AS tarTemp WITH (NOLOCK) ON tarDept.[TargetTemplateId] = tarTemp.[TargetTemplateId]
						INNER JOIN [dbo].[TargetType] AS tar WITH (NOLOCK) ON tarTemp.[TargetTypeId] = tar.[TargetTypeId]
					WHERE tar.[TargetTypeId] = @TargetTypeId
						AND tarDept.[IsGroup] = 0
					)
				)
		
		UNION
		SELECT [GroupId] AS DepartmentId
			,[Name]
			,[DepartmentId] AS ParentId
			,~ [IsDeleted] AS IsActive
			,1 AS IsGroup
		FROM [dbo].[Group]
		WHERE [IsDeleted] = 0
			AND (
				@IsGroup IS NULL
				OR (
					@IsGroup = 1
					AND [GroupId] = @DepartmentId
					)
				)
			AND (
				(
					ISNULL(@ShowAll, '') = ''
					AND [GroupId] IN (
						SELECT DISTINCT tarTemp.[DepartmentId] AS GroupId
						FROM [dbo].[TargetTemplateDepartment] AS tarTemp WITH (NOLOCK)
							INNER JOIN [dbo].[Group] AS gr WITH (NOLOCK) ON tarTemp.[DepartmentId] = gr.[GroupId]
						WHERE tarTemp.[IsGroup] = 1
						)
					)
				OR @ShowAll = 1
				)
			AND (
				ISNULL(@TargetTypeId, '') = ''
				OR [GroupId] IN (
					SELECT DISTINCT [DepartmentId] AS GroupId
					FROM [dbo].[TargetTemplateDepartment] AS tarDept WITH (NOLOCK)
						INNER JOIN [dbo].[TargetTemplate] AS tarTemp WITH (NOLOCK) ON tarDept.[TargetTemplateId] = tarTemp.[TargetTemplateId]
						INNER JOIN [dbo].[TargetType] AS tar WITH (NOLOCK) ON tarTemp.[TargetTypeId] = tar.[TargetTypeId]
					WHERE tar.[TargetTypeId] = @TargetTypeId
						AND tarDept.[IsGroup] = 1
					)
				)
		) AS deptSet

	SELECT [DepartmentId]
		,[IsGroup]
		,tarDept.[TargetTemplateId]
		,tarTemp.[Name]
		,tar.[TargetTypeId]
		,tar.[Name] AS TargetTypeName
	FROM [dbo].[TargetTemplateDepartment] AS tarDept WITH (NOLOCK)
		INNER JOIN [dbo].[TargetTemplate] AS tarTemp WITH (NOLOCK) ON tarDept.[TargetTemplateId] = tarTemp.[TargetTemplateId]
		INNER JOIN [dbo].[TargetType] AS tar WITH (NOLOCK) ON tarTemp.[TargetTypeId] = tar.[TargetTypeId]
	ORDER BY [DepartmentId]
		,tar.[TargetTypeId]
END