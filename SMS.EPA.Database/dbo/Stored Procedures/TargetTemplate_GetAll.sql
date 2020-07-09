CREATE PROCEDURE [dbo].[TargetTemplate_GetAll] 
	@TargetTypeId INT = NULL
	,@IsActive TINYINT = NULL
	,@DepartmentId INT = NULL
	,@IsGroup BIT = NULL
	,@ScoreRatingTypeId INT = NULL
AS
/*
Created: 2014004@Quinn.Vo
Task: Get all targets template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT tg.TargetTemplateId
		,tg.[Name]
		,tg.[Description]
		,tg.[IsActive]
		,tg.[TargetTypeId]
		,tgTy.[Name] AS TargetTypeName
		,tg.[RatingTypeId]
		,ref.Value AS RatingTypeName
		,scT.[ScoreRatingTypeId]
		,scT.[Name] AS ScoreRatingTypeName
		,ISNULL(tk.KPIs, 0) AS KPIs
		,SUBSTRING(
			(
				SELECT ', ' + 
					CASE
						WHEN tmpDep.IsGroup = 0 THEN dep2.Code
						ELSE gr.Code
					END
				FROM TargetTemplate AS tg2
					LEFT JOIN TargetTemplateDepartment tmpDep WITH (NOLOCK) ON tmpDep.TargetTemplateId = tg2.TargetTemplateId
					LEFT JOIN TargetTemplateEmployee tmpEmp WITH (NOLOCK) ON tmpEmp.TargetTemplateId = tg2.TargetTemplateId
					LEFT JOIN Employee emp WITH (NOLOCK) ON emp.EmployeeId = tmpEmp.EmployeeId
					LEFT JOIN Department childDep WITH (NOLOCK) ON gr.DepartmentId = childDep.DepartmentId OR tgDep.DepartmentId = childDep.DepartmentId
					LEFT JOIN Department dep2 WITH (NOLOCK) ON dep2.DepartmentId = tmpDep.DepartmentId OR (tmpDep.IsGroup = 0 AND dep2.DepartmentId = gr.DepartmentId) 
					LEFT JOIN [Group] gr WITH (NOLOCK) ON gr.GroupId = emp.GroupId OR (tmpDep.IsGroup = 1 AND tmpDep.DepartmentId = gr.GroupId)
				WHERE tg2.TargetTemplateId = tg.TargetTemplateId
					AND gr.IsDeleted = 0
				GROUP BY tg2.TargetTemplateId, tmpDep.IsGroup, dep2.Code, gr.Code
				ORDER BY dep2.Code ASC
				FOR XML PATH('')
			), 2, 200) [Departments]
	FROM dbo.TargetTemplate AS tg WITH (NOLOCK)
		LEFT JOIN (
		SELECT TargetTemplateId
			,COUNT(1) AS KPIs
		FROM dbo.TargetTemplateKPI WITH (NOLOCK)
		GROUP BY TargetTemplateId
		) AS tk ON tg.TargetTemplateId = tk.TargetTemplateId
		INNER JOIN [dbo].[TargetType] AS tgTy WITH (NOLOCK) ON tg.[TargetTypeId] = tgTy.[TargetTypeId]
		INNER JOIN [dbo].[ScoreRatingType] AS scT WITH (NOLOCK) ON tg.[ScoreRatingTypeId] = scT.[ScoreRatingTypeId]
		INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON tg.RatingTypeId = ref.ResourceId
			AND ref.RefName = 'RatingType'
		LEFT JOIN TargetTemplateDepartment tgDep WITH (NOLOCK) ON tgDep.TargetTemplateId = tg.TargetTemplateId
		LEFT JOIN TargetTemplateEmployee tgEmp WITH (NOLOCK) ON tgEmp.TargetTemplateId = tg.TargetTemplateId
		LEFT JOIN Employee emp WITH (NOLOCK) ON emp.EmployeeId = tgEmp.EmployeeId
		LEFT JOIN [Group] gr WITH (NOLOCK) ON gr.GroupId = emp.GroupId OR (tgDep.IsGroup = 1 AND gr.GroupId = tgDep.DepartmentId)
		LEFT JOIN Department childDep WITH (NOLOCK) ON gr.DepartmentId = childDep.DepartmentId
		LEFT JOIN Department dep WITH (NOLOCK) 
			ON dep.DepartmentId = tgDep.DepartmentId 
			OR dep.DepartmentId = gr.DepartmentId 
			OR childDep.ParentId = dep.DepartmentId
	WHERE ISNULL(@TargetTypeId, tg.TargetTypeId) = tg.TargetTypeId
		AND ISNULL(@IsActive, tg.[IsActive]) = tg.[IsActive]
		AND (
			@DepartmentId IS NULL
			OR (@IsGroup = 1 AND gr.GroupId = @DepartmentId)
			OR (@IsGroup = 0 AND dep.DepartmentId = @DepartmentId)
			)
		AND  ISNULL(@ScoreRatingTypeId, scT.ScoreRatingTypeId) = scT.ScoreRatingTypeId
	ORDER BY tg.[Name] ASC
END