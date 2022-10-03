
CREATE PROCEDURE [dbo].[JobLevelStandard_PeriodJobLevelSetting_GetByPeriodId] @PeriodId INT
AS
/*
Created: 2022-08-10@Tim.Trinh
Task: get job level setting by periodid
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT jls.SectionId
		,jl.JobLevelId
		,jl.[Name]
		,jls.MinWeight
		,jls.MaxWeight
	FROM dbo.JobLevel AS jl WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodJobLevelSetting AS jls WITH (NOLOCK) ON jl.JobLevelId = jls.JobLevelId
		AND jls.PeriodId = @PeriodId
	ORDER BY jls.SectionId
		,jl.JobLevelId
END