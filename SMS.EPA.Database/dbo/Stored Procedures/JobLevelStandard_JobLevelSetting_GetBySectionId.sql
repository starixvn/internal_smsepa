
CREATE PROCEDURE [dbo].[JobLevelStandard_JobLevelSetting_GetBySectionId] @SectionId INT
AS
/*
Created: 2022-08-10@Tim.Trinh
Task: get job level setting by SectionId
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT jl.JobLevelId
		,jl.[Name]
		,jls.MinWeight
		,jls.MaxWeight
	FROM dbo.JobLevel AS jl WITH (NOLOCK)
		LEFT JOIN dbo.JobLevelStandard_JobLevelSetting AS jls WITH (NOLOCK) ON jl.JobLevelId = jls.JobLevelId
		AND jls.SectionId = @SectionId
	WHERE jl.IsDeleted = 0
	ORDER BY jl.DisplayOrder DESC
END