
CREATE PROCEDURE [dbo].[EpaTargetTemplateDepartment_Get] @DepartmentIds VARCHAR(MAX) = NULL
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

	CREATE TABLE #tmpDepartment (DepartmentId INT)

	IF ISNULL(@DepartmentIds, '') != ''
	BEGIN
		INSERT INTO #tmpDepartment (DepartmentId)
		SELECT val
		FROM dbo.fnc_strSplit(@DepartmentIds, ',')
	END

	--SELECT deptSet.DepartmentId
	--	,deptSet.Unit
	--	,deptSet.Department
	--	,deptSet.Section
	--	,deptSet.Team
	--	,deptSet.ParentId
	--	,deptSet.IsActive
	--	,deptSet.IsGroup
	--FROM (
	--	SELECT DepartmentId
	--		,NULL AS Unit
	--		,[Name] AS Department
	--		,NULL AS Section
	--		,NULL AS Team
	--		,UnitId AS ParentId 
	--		,IsActive
	--		,0 AS IsGroup
	--	FROM dbo.Department_New AS dept WITH (NOLOCK)
	--	WHERE IsActive = 1
	--		AND (
	--			ISNULL(@DepartmentIds, '') = ''
	--			OR EXISTS (
	--				SELECT 1
	--				FROM #tmpDepartment AS tmp WITH (NOLOCK)
	--				WHERE dept.DepartmentId = tmp.DepartmentId
	--				)
	--		)
	--		AND (
	--			(
	--				@ShowAll = 0
	--				AND dept.DepartmentId IN (
	--					SELECT DISTINCT tarTemp.DepartmentId
	--					FROM dbo.TargetTemplateDepartment AS tarTemp WITH (NOLOCK)
	--						INNER JOIN dbo.Department_New AS dept1 WITH (NOLOCK) ON tarTemp.DepartmentId = dept1.DepartmentId
	--						AND tarTemp.IsGroup = 0
	--					)
	--				)
	--			OR @ShowAll = 1
	--			)
	--		AND (
	--			ISNULL(@TargetTypeId, '') = ''
	--			OR dept.DepartmentId IN (
	--				SELECT DISTINCT DepartmentId
	--				FROM dbo.TargetTemplateDepartment AS tarDept WITH (NOLOCK)
	--					INNER JOIN dbo.TargetTemplate AS tarTemp WITH (NOLOCK) ON tarDept.TargetTemplateId = tarTemp.TargetTemplateId
	--					INNER JOIN dbo.TargetType AS tar WITH (NOLOCK) ON tarTemp.TargetTypeId = tar.TargetTypeId
	--				WHERE tar.TargetTypeId = @TargetTypeId
	--					AND tarDept.IsGroup = 0
	--				)
	--			)
	--	UNION
	--	SELECT UnitId AS DepartmentId
	--		,dept.Name AS Unit
	--		,NULL AS Department
	--		,NULL AS Section
	--		,NULL AS Team
	--		,NULL AS ParentId
	--		,IsActive
	--		,0 AS IsGroup
	--	FROM dbo.Unit AS dept WITH (NOLOCK)
	--	WHERE IsActive = 1
	--		AND (
	--			ISNULL(@DepartmentIds, '') = ''
	--			OR EXISTS (
	--				SELECT 1
	--				FROM #tmpDepartment AS tmp WITH (NOLOCK)
	--				WHERE dept.UnitId = tmp.DepartmentId
	--				)
	--		)
	--		AND (
	--			(
	--				@ShowAll = 0
	--				AND dept.UnitId IN (
	--					SELECT DISTINCT tarTemp.DepartmentId
	--					FROM dbo.TargetTemplateDepartment AS tarTemp WITH (NOLOCK)
	--						INNER JOIN dbo.Unit AS dept1 WITH (NOLOCK) ON tarTemp.DepartmentId = dept1.UnitId
	--						AND tarTemp.IsGroup = 0
	--					)
	--				)
	--			OR @ShowAll = 1
	--			)
	--		AND (
	--			ISNULL(@TargetTypeId, '') = ''
	--			OR dept.UnitId IN (
	--				SELECT DISTINCT DepartmentId
	--				FROM dbo.TargetTemplateDepartment AS tarDept WITH (NOLOCK)
	--					INNER JOIN dbo.TargetTemplate AS tarTemp WITH (NOLOCK) ON tarDept.TargetTemplateId = tarTemp.TargetTemplateId
	--					INNER JOIN dbo.TargetType AS tar WITH (NOLOCK) ON tarTemp.TargetTypeId = tar.TargetTypeId
	--				WHERE tar.TargetTypeId = @TargetTypeId
	--					AND tarDept.IsGroup = 0
	--				)
	--			)
	--	UNION
	--	SELECT SectionId AS DepartmentId
	--		,NULL AS Unit
	--		,NULL AS Department
	--		,[Name] AS Section
	--		,NULL AS Team
	--		,DepartmentId AS ParentId
	--		,IsActive
	--		,0 AS IsGroup
	--	FROM dbo.Section AS dept WITH (NOLOCK)
	--	WHERE IsActive = 1
	--		AND (
	--			ISNULL(@DepartmentIds, '') = ''
	--			OR EXISTS (
	--				SELECT 1
	--				FROM #tmpDepartment AS tmp WITH (NOLOCK)
	--				WHERE dept.SectionId = tmp.DepartmentId
	--				)
	--		)
	--		AND (
	--			(
	--				@ShowAll = 0
	--				AND dept.SectionId IN (
	--					SELECT DISTINCT tarTemp.DepartmentId
	--					FROM dbo.TargetTemplateDepartment AS tarTemp WITH (NOLOCK)
	--						INNER JOIN dbo.Section AS dept1 WITH (NOLOCK) ON tarTemp.DepartmentId = dept1.SectionId
	--						AND tarTemp.IsGroup = 0
	--					)
	--				)
	--			OR @ShowAll = 1
	--			)
	--		AND (
	--			ISNULL(@TargetTypeId, '') = ''
	--			OR dept.SectionId IN (
	--				SELECT DISTINCT DepartmentId
	--				FROM dbo.TargetTemplateDepartment AS tarDept WITH (NOLOCK)
	--					INNER JOIN dbo.TargetTemplate AS tarTemp WITH (NOLOCK) ON tarDept.TargetTemplateId = tarTemp.TargetTemplateId
	--					INNER JOIN dbo.TargetType AS tar WITH (NOLOCK) ON tarTemp.TargetTypeId = tar.TargetTypeId
	--				WHERE tar.TargetTypeId = @TargetTypeId
	--					AND tarDept.IsGroup = 0
	--				)
	--			)
	--	UNION
	SELECT gr.GroupId AS DepartmentId
		,dept.[Name] AS Department
		,sec.[Name] AS Section
		,gr.[Name] AS Team
		,gr.DepartmentId AS ParentId
		,~ [IsDeleted] AS IsActive
		,1 AS IsGroup
	FROM dbo.[Group] AS gr WITH (NOLOCK)
		LEFT JOIN dbo.Section AS sec WITH (NOLOCK) ON gr.DepartmentId = sec.SectionId
		AND sec.IsActive = 1
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON sec.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
	WHERE gr.IsDeleted = 0
		AND (
			ISNULL(@DepartmentIds, '') = ''
			OR EXISTS (
				SELECT 1
				FROM #tmpDepartment AS tmp WITH (NOLOCK)
				WHERE gr.GroupId = tmp.DepartmentId
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
	ORDER BY dept.[Name] DESC
		,sec.[Name] DESC
		,gr.[Name] DESC

	SELECT DepartmentId
		,IsGroup
		,tarDept.TargetTemplateId
		,tarTemp.[Name]
		,tar.TargetTypeId
		,tar.[Name] AS TargetTypeName
	FROM dbo.TargetTemplateDepartment AS tarDept WITH (NOLOCK)
		INNER JOIN dbo.TargetTemplate AS tarTemp WITH (NOLOCK) ON tarDept.TargetTemplateId = tarTemp.TargetTemplateId
		INNER JOIN dbo.TargetType AS tar WITH (NOLOCK) ON tarTemp.TargetTypeId = tar.TargetTypeId
	WHERE tarDept.IsGroup = 1
	ORDER BY DepartmentId ASC
		,tar.TargetTypeId ASC

	DROP TABLE #tmpDepartment
END