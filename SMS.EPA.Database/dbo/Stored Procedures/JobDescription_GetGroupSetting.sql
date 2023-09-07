
CREATE PROCEDURE [dbo].[JobDescription_GetGroupSetting]
AS
/*
Created: 20210426@Tim.Trinh
Task: job description - get group setting
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT gr.GroupId
		,gr.[Name]
		,gr.Code
		,STUFF((
				SELECT DISTINCT ', ' + jlevel.[Name]
				FROM dbo.JobDescription_GroupSetting AS setting WITH (NOLOCK)
					INNER JOIN dbo.JobLevel AS jlevel WITH (NOLOCK) ON setting.JobLevelId = jlevel.JobLevelId
					AND jlevel.IsDeleted = 0
				WHERE setting.GroupId = gr.GroupId
				FOR XML PATH
					,TYPE
				).value(N'.[1]', N'NVARCHAR(MAX)'), 1, 2, N'') JobLevels
		,dept.[Name] AS Department
		,sec.[Name] AS Section
	FROM dbo.[Group] AS gr WITH (NOLOCK)
		LEFT JOIN dbo.Section AS sec WITH (NOLOCK) ON gr.DepartmentId = sec.SectionId
		AND sec.IsActive = 1
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON sec.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
	WHERE gr.IsDeleted = 0
	ORDER BY gr.[Name]
END