
CREATE PROCEDURE [dbo].[TargetTemplate_GetAll] @TargetTypeId INT = NULL
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
		,STUFF((
				SELECT ', ' + tttemp.Code
				FROM dbo.TargetTemplate AS tg2 WITH (NOLOCK)
					INNER JOIN (
					SELECT ttdept.TargetTemplateId
						,ttdept.IsGroup
						,dept.Code
					FROM dbo.TargetTemplateDepartment AS ttdept WITH (NOLOCK)
						INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON ttdept.DepartmentId = dept.DepartmentId
						AND ttdept.IsGroup = 0
						AND dept.IsActive = 1
					
					UNION
					SELECT ttdept.TargetTemplateId
						,ttdept.IsGroup
						,gr.Code
					FROM dbo.TargetTemplateDepartment AS ttdept WITH (NOLOCK)
						INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON ttdept.DepartmentId = gr.GroupId
						AND ttdept.IsGroup = 1
						AND gr.IsDeleted = 0
					
					UNION
					SELECT ttemp.TargetTemplateId
						,1 AS IsGroup
						,gr.Code
					FROM dbo.TargetTemplateEmployee AS ttemp WITH (NOLOCK)
						INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmployeeId = ttemp.EmployeeId
						AND emp.IsDeleted = 0
						AND emp.[Enabled] = 1
						INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gr.GroupId = emp.GroupId
						AND gr.IsDeleted = 0
					) AS tttemp ON tg2.TargetTemplateId = tttemp.TargetTemplateId
				WHERE tg.TargetTemplateId = tg2.TargetTemplateId
				GROUP BY tg2.TargetTemplateId
					,tttemp.IsGroup
					,tttemp.Code
				ORDER BY tttemp.Code ASC
				FOR XML PATH
					,TYPE
				).value(N'.[1]', N'NVARCHAR(MAX)'), 1, 2, N'') [Departments]
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
	WHERE ISNULL(@TargetTypeId, tg.TargetTypeId) = tg.TargetTypeId
		AND ISNULL(@IsActive, tg.[IsActive]) = tg.[IsActive]
		AND (
			@DepartmentId IS NULL
			OR EXISTS (
				SELECT 1
				FROM dbo.TargetTemplateDepartment AS ttdept WITH (NOLOCK)
					LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON ttdept.DepartmentId = dept.DepartmentId
					AND dept.IsActive = 1
					LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON ttdept.DepartmentId = gr.GroupId
					AND gr.IsDeleted = 0
				WHERE tg.TargetTemplateId = ttdept.TargetTemplateId
					AND (
						(
							@IsGroup = 0
							AND ttdept.IsGroup = 0
							AND dept.DepartmentId = @DepartmentId
							)
						OR (
							@IsGroup = 1
							AND ttdept.IsGroup = 1
							AND gr.GroupId = @DepartmentId
							)
						)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.TargetTemplateEmployee AS ttemp WITH (NOLOCK)
					INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmployeeId = ttemp.EmployeeId
					AND emp.[Enabled] = 1
					AND emp.IsDeleted = 0
					INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
					AND gr.IsDeleted = 0
					INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON dept.IsActive = 1
					AND (
						gr.DepartmentId = dept.DepartmentId
						OR (
							dept.lft < gr.lft
							AND dept.rgt > gr.rgt
							)
						)
				WHERE tg.TargetTemplateId = ttemp.TargetTemplateId
					AND (
						(
							@IsGroup = 1
							AND gr.GroupId = @DepartmentId
							)
						OR (
							@IsGroup = 0
							AND dept.DepartmentId = @DepartmentId
							)
						)
				)
			)
		AND  ISNULL(@ScoreRatingTypeId, scT.ScoreRatingTypeId) = scT.ScoreRatingTypeId
	ORDER BY tg.[Name] ASC
END