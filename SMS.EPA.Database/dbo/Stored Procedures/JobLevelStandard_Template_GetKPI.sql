
CREATE PROCEDURE [dbo].[JobLevelStandard_Template_GetKPI] @TemplateId INT
	,@IsSelected BIT = 0
AS
/*
Created: 2023-04-26@Frankie.Nguyen
Task: get KPI in template
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF @IsSelected = 0
	BEGIN
		CREATE TABLE #tmpKPI (
			KPIId INT
			,[Name] NVARCHAR(200)
			,DisplayOrder TINYINT
			,IsSelected BIT
			,[Weight] FLOAT
			)

		INSERT INTO #tmpKPI (
			KPIId
			,[Name]
			,DisplayOrder
			,IsSelected
			,[Weight]
			)
		SELECT kpi.KPIId
			,kpi.[Name]
			,jt.DisplayOrder
			,(CASE 
				WHEN jt.DisplayOrder IS NOT NULL
					AND jt.TemplateId = @TemplateId THEN 1
				ELSE 0 END
				) IsSelected
			,jt.[Weight]
		FROM dbo.JobLevelStandard_KPI AS kpi WITH (NOLOCK)
			LEFT JOIN dbo.JobLevelStandard_TemplateKPI AS jt WITH (NOLOCK) ON jt.KPIId = kpi.KPIId
		WHERE kpi.IsActive = 1
			AND jt.TemplateId = @TemplateId

		INSERT INTO #tmpKPI (
			KPIId
			,[Name]
			,DisplayOrder
			,IsSelected
			,[Weight]
			)
		SELECT kpi.KPIId
			,kpi.[Name]
			,100 AS DisplayOrder
			,0 AS IsSelected
			,NULL AS [Weight]
		FROM dbo.JobLevelStandard_KPI AS kpi WITH (NOLOCK)
		WHERE kpi.IsActive = 1
			AND NOT EXISTS (
				SELECT 1
				FROM #tmpKPI AS t WITH (NOLOCK)
				WHERE t.KPIId = kpi.KPIId
				)

		SELECT KPIId
			,[Name]
			,[Weight]
			,DisplayOrder
			,IsSelected
		FROM #tmpKPI WITH (NOLOCK)
		ORDER BY DisplayOrder ASC

		DROP TABLE #tmpKPI
	END
	ELSE
	BEGIN
		SELECT kpi.KPIId
			,kpi.[Name]
			,jt.[Weight]
			,jt.DisplayOrder
		FROM dbo.JobLevelStandard_TemplateKPI AS jt WITH (NOLOCK)
			INNER JOIN dbo.JobLevelStandard_KPI AS kpi WITH (NOLOCK) ON jt.KPIId = kpi.KPIId
		WHERE jt.TemplateId = @TemplateId
			AND kpi.IsActive = 1
		ORDER BY jt.DisplayOrder ASC
	END
END