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
	CREATE TABLE #tmpHierarchyDepartment (
		DirectParentId INT
		,DepartmentId INT
		,[Level] INT
		,ParentId INT
		)

	CREATE TABLE #tmpFlowTemplate (
		DepartmentId INT
		,[Level] INT
		,ParentId INT
		,FlowTemplateId INT
		)

	CREATE TABLE #tmpTargetTemplate (
		DepartmentId INT
		,[Level] INT
		,ParentId INT
		,TargetTemplateId INT
		)

	CREATE TABLE #tmpFlowAndTargetTemplate (
		EmployeeId INT
		,DepartmentId INT
		,FlowTemplateId INT
		,TargetTemplateId INT
		)

	INSERT INTO #tmpHierarchyDepartment (
		DirectParentId
		,DepartmentId
		,[Level]
		,ParentId
		)
	SELECT DISTINCT ParentId
		,DepartmentId
		,[Level]
		,CASE 
			WHEN ISNUMERIC(tmp.val) = 1 THEN tmp.val
			ELSE NULL
			END
	FROM dbo.HierarchyDepartment AS hd WITH (NOLOCK)
		CROSS APPLY dbo.fnc_strSplit(ParentIds, ',') AS tmp;

	INSERT INTO #tmpFlowTemplate (
		DepartmentId
		,[Level]
		,ParentId
		,FlowTemplateId
		)
	SELECT DISTINCT h.DepartmentId
		,h.[Level]
		,h.ParentId
		,dfs.FlowTemplateId
	FROM #tmpHierarchyDepartment AS h WITH (NOLOCK)
		LEFT JOIN (
		SELECT DISTINCT dfs.DepartmentId
			,dfs.FlowTemplateId
		FROM dbo.SettingDepartmentFlow AS dfs WITH (NOLOCK)
			INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON dfs.FlowTemplateId = ft.FlowTemplateId
		WHERE dFS.IsGroup = 0
			AND ft.TargetTypeId = @TargetTypeId
		) AS dfs ON h.DepartmentId = dfs.DepartmentId

	INSERT INTO #tmpTargetTemplate (
		DepartmentId
		,[Level]
		,ParentId
		,TargetTemplateId
		)
	SELECT DISTINCT h.DepartmentId
		,h.[Level]
		,h.ParentId
		,dts.TargetTemplateId
	FROM #tmpHierarchyDepartment AS h
		LEFT JOIN (
		SELECT DISTINCT dTS.DepartmentId
			,dTS.TargetTemplateId
		FROM dbo.TargetTemplateDepartment AS dts WITH (NOLOCK)
			INNER JOIN dbo.TargetTemplate AS tt WITH (NOLOCK) ON dts.TargetTemplateId = tt.TargetTemplateId
		WHERE dts.IsGroup = 0
			AND tt.TargetTypeId = @TargetTypeId
		) AS dts ON h.DepartmentId = dts.DepartmentId

	INSERT INTO #tmpFlowAndTargetTemplate (
		EmployeeId
		,DepartmentId
		,FlowTemplateId
		)
	SELECT DISTINCT emp.EmployeeId
		,ft.DepartmentId
		,ft.FlowTemplateId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN #tmpFlowTemplate AS ft WITH (NOLOCK) ON gr.DepartmentId = ft.DepartmentId
	WHERE ft.FlowTemplateId IS NOT NULL

	INSERT INTO #tmpFlowAndTargetTemplate (
		EmployeeId
		,DepartmentId
		,FlowTemplateId
		)
	SELECT DISTINCT emp.EmployeeId
		,ft.ParentId
		,parent.FlowTemplateId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN #tmpFlowTemplate AS ft WITH (NOLOCK) ON gr.DepartmentId = ft.DepartmentId
		INNER JOIN #tmpFlowTemplate AS parent WITH (NOLOCK) ON ft.ParentId = parent.DepartmentId
	WHERE parent.FlowTemplateId IS NOT NULL
		AND NOT EXISTS (
			SELECT 1
			FROM #tmpFlowAndTargetTemplate AS tmp WITH (NOLOCK)
			WHERE emp.EmployeeId = tmp.EmployeeId
				AND tmp.FlowTemplateId IS NOT NULL
			)
		AND parent.[Level] = (
			SELECT MAX(tmp.[Level])
			FROM #tmpFlowTemplate AS tmp WITH (NOLOCK)
			WHERE parent.DepartmentId = tmp.DepartmentId
				AND tmp.FlowTemplateId IS NOT NULL
			GROUP BY tmp.DepartmentId
			)

	INSERT INTO #tmpFlowAndTargetTemplate (
		EmployeeId
		,DepartmentId
		,TargetTemplateId
		)
	SELECT DISTINCT emp.EmployeeId
		,tt.DepartmentId
		,tt.TargetTemplateId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN #tmpTargetTemplate AS tt WITH (NOLOCK) ON gr.DepartmentId = tt.DepartmentId
	WHERE tt.TargetTemplateId IS NOT NULL

	INSERT INTO #tmpFlowAndTargetTemplate (
		EmployeeId
		,DepartmentId
		,TargetTemplateId
		)
	SELECT DISTINCT emp.EmployeeId
		,tt.ParentId
		,parent.TargetTemplateId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN #tmpTargetTemplate AS tt WITH (NOLOCK) ON gr.DepartmentId = tt.DepartmentId
		INNER JOIN #tmpTargetTemplate AS parent WITH (NOLOCK) ON tt.ParentId = parent.DepartmentId
	WHERE NOT EXISTS (
			SELECT 1
			FROM #tmpFlowAndTargetTemplate AS tmp WITH (NOLOCK)
			WHERE emp.EmployeeId = tmp.EmployeeId
				AND tmp.TargetTemplateId IS NOT NULL
			)
		AND parent.[Level] = (
			SELECT MAX(tmp.[Level])
			FROM #tmpTargetTemplate AS tmp WITH (NOLOCK)
			WHERE parent.DepartmentId = tmp.DepartmentId
				AND tmp.TargetTemplateId IS NOT NULL
			GROUP BY tmp.DepartmentId
			)

	SELECT DepartmentId
		,EmployeeId
		,FlowTemplateId
		,TargetTemplateId
	FROM #tmpFlowAndTargetTemplate WITH (NOLOCK)

	DROP TABLE #tmpHierarchyDepartment

	DROP TABLE #tmpFlowTemplate

	DROP TABLE #tmpTargetTemplate

	DROP TABLE #tmpFlowAndTargetTemplate
END