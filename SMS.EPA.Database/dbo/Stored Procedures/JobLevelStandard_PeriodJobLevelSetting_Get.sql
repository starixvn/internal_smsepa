
CREATE PROCEDURE [dbo].[JobLevelStandard_PeriodJobLevelSetting_Get] @PeriodId INT
	,@HasSetting BIT = NULL
AS
/*
Created: 2022-08-10@Tim.Trinh
Task: get job level setting by period
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT s.DepartmentId AS SectionId
		,s.Code
		,s.[Name]
		,dept.DepartmentId
		,dept.[Name] AS Department
		,unit.DepartmentId AS UnitId
		,unit.[Name] AS Unit
		,CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.JobLevelStandard_PeriodJobLevelSetting AS jls WITH (NOLOCK)
					WHERE jls.PeriodId = @PeriodId
						AND s.DepartmentId = jls.SectionId
						AND jls.MinWeight IS NOT NULL
						AND jls.MaxWeight IS NOT NULL
					) THEN 1
			ELSE 0
			END AS HasSetting
	FROM dbo.Department AS s WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS team WITH (NOLOCK) ON s.DepartmentId = team.DepartmentId
		AND team.IsDeleted = 0
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON s.ParentId = dept.DepartmentId
		AND dept.IsActive = 1
		LEFT JOIN dbo.Department AS unit WITH (NOLOCK) ON dept.ParentId = unit.DepartmentId
		AND unit.IsActive = 1
	WHERE s.IsActive = 1
		AND (
			@HasSetting IS NULL
			OR (
				@HasSetting = 0
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.JobLevelStandard_PeriodJobLevelSetting AS jls WITH (NOLOCK)
					WHERE jls.PeriodId = @PeriodId
						AND s.DepartmentId = jls.SectionId
						AND jls.MinWeight IS NOT NULL
						AND jls.MaxWeight IS NOT NULL
					)
				)
			OR (
				@HasSetting = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.JobLevelStandard_PeriodJobLevelSetting AS jls WITH (NOLOCK)
					WHERE jls.PeriodId = @PeriodId
						AND s.DepartmentId = jls.SectionId
						AND jls.MinWeight IS NOT NULL
						AND jls.MaxWeight IS NOT NULL
					)
				)
			)
	ORDER BY dept.[Name]
		,s.[Name]
END