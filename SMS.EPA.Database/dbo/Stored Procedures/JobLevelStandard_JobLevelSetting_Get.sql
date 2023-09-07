

CREATE PROCEDURE [dbo].[JobLevelStandard_JobLevelSetting_Get] @HasSetting BIT = NULL
AS
/*
Created: 2022-08-10@Tim.Trinh
Task: get job level setting summary
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT s.SectionId
		,s.Code
		,s.[Name]
		,dept.DepartmentId
		,dept.[Name] AS Department
		,unit.UnitId
		,unit.[Name] AS Unit
		,CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.JobLevelStandard_JobLevelSetting AS jls WITH (NOLOCK)
					WHERE s.SectionId = jls.SectionId
						AND jls.MinWeight IS NOT NULL
						AND jls.MaxWeight IS NOT NULL
					) THEN 1
			ELSE 0
			END AS HasSetting
	FROM dbo.Section AS s WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS team WITH (NOLOCK) ON s.SectionId = team.DepartmentId
		AND team.IsDeleted = 0
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON s.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
		LEFT JOIN dbo.Unit AS unit WITH (NOLOCK) ON dept.UnitId = unit.UnitId
		AND unit.IsActive = 1
	WHERE s.IsActive = 1
		AND (
			@HasSetting IS NULL
			OR (
				@HasSetting = 0
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.JobLevelStandard_JobLevelSetting AS jls WITH (NOLOCK)
					WHERE s.SectionId = jls.SectionId
						AND jls.MinWeight IS NOT NULL
						AND jls.MaxWeight IS NOT NULL
					)
				)
			OR (
				@HasSetting = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.JobLevelStandard_JobLevelSetting AS jls WITH (NOLOCK)
					WHERE s.SectionId = jls.SectionId
						AND jls.MinWeight IS NOT NULL
						AND jls.MaxWeight IS NOT NULL
					)
				)
			)
	ORDER BY dept.[Name]
		,s.[Name]
END