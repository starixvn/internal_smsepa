
CREATE PROCEDURE [dbo].[TargetTemplate_GetAll] @TargetTypeId TINYINT = NULL
	,@IsActive TINYINT = NULL
	,@TeamIds NVARCHAR(MAX) = NULL
	,@ScoreRatingTypeId TINYINT = NULL
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

	CREATE TABLE #tmpTeamIds (TeamId INT PRIMARY KEY)

	CREATE TABLE #tmpAssignedTeam (
		TemplateId INT
		,TeamId INT
		,Code NVARCHAR(150)
		,[Name] NVARCHAR(150)
		)

	IF ISNULL(@TeamIds, '') != ''
	BEGIN
		INSERT INTO #tmpTeamIds (TeamId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
		WHERE ISNUMERIC(val) = 1
	END

	INSERT INTO #tmpAssignedTeam (
		TemplateId
		,TeamId
		,Code
		,[Name]
		)
	SELECT ttdept.TargetTemplateId
		,gr.GroupId
		,gr.Code
		,gr.[Name]
	FROM dbo.TargetTemplateDepartment AS ttdept WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON ttdept.DepartmentId = gr.GroupId
	WHERE ttdept.IsGroup = 1
		AND gr.IsDeleted = 0
	
	UNION
	SELECT ttemp.TargetTemplateId
		,gr.GroupId
		,gr.Code
		,gr.[Name]
	FROM dbo.TargetTemplateEmployee AS ttemp WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmployeeId = ttemp.EmployeeId
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gr.GroupId = emp.GroupId
		AND gr.IsDeleted = 0

	SELECT DISTINCT tg.TargetTemplateId
		,tg.[Name]
		,tg.[Description]
		,tg.IsActive
		,tg.TargetTypeId
		,tgTy.[Name] AS TargetTypeName
		,tg.RatingTypeId
		,ref.Value AS RatingTypeName
		,scT.ScoreRatingTypeId
		,scT.[Name] AS ScoreRatingTypeName
		,ISNULL(tk.KPIs, 0) AS KPIs
		,STUFF((
				SELECT ', ' + tmp.[Name]
				FROM dbo.TargetTemplate AS tg2 WITH (NOLOCK)
					INNER JOIN #tmpAssignedTeam AS tmp WITH (NOLOCK) ON tg2.TargetTemplateId = tmp.TemplateId
				WHERE tg.TargetTemplateId = tg2.TargetTemplateId
				GROUP BY tg2.TargetTemplateId
					,tmp.[Name]
				ORDER BY tmp.[Name] ASC
				FOR XML PATH
					,TYPE
				).value(N'.[1]', N'NVARCHAR(MAX)'), 1, 2, N'') [Teams]
	FROM dbo.TargetTemplate AS tg WITH (NOLOCK)
		LEFT JOIN (
		SELECT TargetTemplateId
			,COUNT(1) AS KPIs
		FROM dbo.TargetTemplateKPI WITH (NOLOCK)
		GROUP BY TargetTemplateId
		) AS tk ON tg.TargetTemplateId = tk.TargetTemplateId
		INNER JOIN dbo.TargetType AS tgTy WITH (NOLOCK) ON tg.TargetTypeId = tgTy.TargetTypeId
		INNER JOIN dbo.ScoreRatingType AS scT WITH (NOLOCK) ON tg.ScoreRatingTypeId = scT.ScoreRatingTypeId
		INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON tg.RatingTypeId = ref.ResourceId
		AND ref.RefName = 'RatingType'
	WHERE (
			@TargetTypeId IS NULL
			OR tg.TargetTypeId = @TargetTypeId
			)
		AND (
			@IsActive IS NULL
			OR tg.IsActive = @IsActive
			)
		AND (
			ISNULL(@TeamIds, '') = ''
			OR EXISTS (
				SELECT 1
				FROM dbo.TargetTemplateDepartment AS ttdept WITH (NOLOCK)
					INNER JOIN #tmpTeamIds AS tmp WITH (NOLOCK) ON ttdept.DepartmentId = tmp.TeamId
				WHERE tg.TargetTemplateId = ttdept.TargetTemplateId
					AND ttdept.IsGroup = 1
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.TargetTemplateEmployee AS ttemp WITH (NOLOCK)
					INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmployeeId = ttemp.EmployeeId
					AND emp.[Enabled] = 1
					AND emp.IsDeleted = 0
					INNER JOIN #tmpTeamIds AS tmp WITH (NOLOCK) ON emp.GroupId = tmp.TeamId
				WHERE tg.TargetTemplateId = ttemp.TargetTemplateId
				)
			)
		AND (
			@ScoreRatingTypeId IS NULL
			OR scT.ScoreRatingTypeId = @ScoreRatingTypeId
			)
	ORDER BY tg.[Name] ASC

	DROP TABLE #tmpTeamIds
END