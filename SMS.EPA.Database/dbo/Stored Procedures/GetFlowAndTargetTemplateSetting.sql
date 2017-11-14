CREATE PROCEDURE [dbo].[GetFlowAndTargetTemplateSetting] @TargetTypeId INT
AS
/*
Created: 20160225@Tim.Trinh
Task: get flow, target template setting.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	CREATE TABLE #HierarchyDepartment (
		ParentId INT
		,DepartmentId INT
		,[Level] INT
		,ParentIds NVARCHAR(MAX)
		)

	CREATE TABLE #FlowAndTargetTemplate (
		DepartmentId INT
		,[Level] INT
		,ParentIds NVARCHAR(MAX)
		,FlowTemplateId INT
		,TargetTemplateId INT
		);

	INSERT INTO #HierarchyDepartment (
		ParentId
		,DepartmentId
		,[Level]
		,ParentIds
		)
	SELECT DISTINCT ParentId
		,DepartmentId
		,[Level]
		,ParentIds
	FROM dbo.HierarchyDepartment;

	WITH FlowAndTargetTemplate (
		DepartmentId
		,[Level]
		,ParentIds
		,FlowTemplateId
		,TargetTemplateId
		)
	AS (
		SELECT DISTINCT h.DepartmentId
			,h.[Level]
			,h.ParentIds
			,deptFS.FlowTemplateId
			,deptTS.TargetTemplateId
		FROM #HierarchyDepartment AS h
			LEFT JOIN (
			SELECT DISTINCT dFS.DepartmentId
				,dFS.FlowTemplateId
			FROM dbo.SettingDepartmentFlow AS dFS WITH (NOLOCK)
				INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON dFS.FlowTemplateId = ft.FlowTemplateId
				AND ft.TargetTypeId = @TargetTypeId
			WHERE dFS.IsGroup = 0
			) AS deptFS ON h.DepartmentId = deptFS.DepartmentId
			LEFT JOIN (
			SELECT DISTINCT dTS.DepartmentId
				,dTS.TargetTemplateId
			FROM dbo.TargetTemplateDepartment AS dTS WITH (NOLOCK)
				INNER JOIN dbo.TargetTemplate AS tt WITH (NOLOCK) ON dTS.TargetTemplateId = tt.TargetTemplateId
				AND tt.TargetTypeId = @TargetTypeId
			WHERE dTS.IsGroup = 0
			) AS deptTS ON h.DepartmentId = deptTS.DepartmentId
		)
	INSERT INTO #FlowAndTargetTemplate (
		DepartmentId
		,ParentIds
		,[Level]
		,FlowTemplateId
		,TargetTemplateId
		)
	SELECT ft.DepartmentId
		,ft.ParentIds
		,ft.[Level]
		,CASE 
			WHEN ft.FlowTemplateId IS NULL THEN (
					SELECT TOP 1 ft1.FlowTemplateId
					FROM FlowAndTargetTemplate AS ft1
					WHERE ft1.FlowTemplateId IS NOT NULL
						AND ft1.DepartmentId IN (
							SELECT DISTINCT DepartmentId
							FROM FlowAndTargetTemplate
							WHERE FlowTemplateId IS NOT NULL
								AND [Level] = (
									SELECT MAX([Level])
									FROM FlowAndTargetTemplate
									WHERE DepartmentId IN (
											SELECT DISTINCT val
											FROM fnc_strSplit(ft.ParentIds, ',')
											)
										AND FlowTemplateId IS NOT NULL
									)
							)
					)
			ELSE ft.FlowTemplateId
			END
		,CASE 
			WHEN ft.TargetTemplateId IS NULL THEN (
					SELECT TOP 1 ft1.TargetTemplateId
					FROM FlowAndTargetTemplate AS ft1
					WHERE ft1.TargetTemplateId IS NOT NULL
						AND ft1.DepartmentId IN (
							SELECT DISTINCT DepartmentId
							FROM FlowAndTargetTemplate
							WHERE TargetTemplateId IS NOT NULL
								AND [Level] = (
									SELECT MAX([Level])
									FROM FlowAndTargetTemplate
									WHERE DepartmentId IN (
											SELECT DISTINCT val
											FROM dbo.fnc_strSplit(ft.ParentIds, ',')
											)
										AND TargetTemplateId IS NOT NULL
									)
							)
					)
			ELSE ft.TargetTemplateId
			END
	FROM FlowAndTargetTemplate AS ft

	SELECT ft.DepartmentId
		,emp.EmployeeId
		,ft.FlowTemplateId
		,ft.TargetTemplateId
	FROM #FlowAndTargetTemplate AS ft WITH (NOLOCK)
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON ft.DepartmentId = gr.DepartmentId
		AND gr.IsDeleted = 0
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gr.GroupId = emp.GroupId
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1
	WHERE ft.FlowTemplateId IS NOT NULL
		OR ft.TargetTemplateId IS NOT NULL

	DROP TABLE #HierarchyDepartment

	DROP TABLE #FlowAndTargetTemplate
END