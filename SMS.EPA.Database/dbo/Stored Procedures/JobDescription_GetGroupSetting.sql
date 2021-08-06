
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
	FROM dbo.[Group] AS gr WITH (NOLOCK)
	WHERE gr.IsDeleted = 0
END