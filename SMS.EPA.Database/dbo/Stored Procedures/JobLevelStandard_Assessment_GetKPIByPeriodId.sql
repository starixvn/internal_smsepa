
CREATE PROCEDURE [dbo].[JobLevelStandard_Assessment_GetKPIByPeriodId] @PeriodId INT
AS
/*
Created: 20220816@Tim.Trinh
Task: get kpi.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT kpi.KPIId
		,kpi.[Name]
		,tkpi.DisplayOrder
	FROM dbo.JobLevelStandard_Template AS t WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_TemplateKPI AS tkpi WITH (NOLOCK) ON t.TemplateId = tkpi.TemplateId
		INNER JOIN dbo.JobLevelStandard_KPI AS kpi WITH (NOLOCK) ON tkpi.KPIId = kpi.KPIId
	WHERE EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK)
			WHERE pes.PeriodId = @PeriodId
				AND t.TemplateId = pes.TemplateId
			)
	ORDER BY tkpi.DisplayOrder
END