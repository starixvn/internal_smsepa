
CREATE PROCEDURE [dbo].[JobDescription_GetGroupSettingById] @GroupId INT
	,@JobLevelId INT
AS
/*
Created: 20210428@Tim.Trinh
Task: job description - get group setting by id
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT gr.GroupId
		,gr.Code
		,gr.[Name]
		,dept.[Name] AS Department
	FROM dbo.[Group] AS gr WITH (NOLOCK)
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
	WHERE gr.GroupId = @GroupId

	SELECT gitem.JobDescription_GroupItemId AS GroupItemId
		,gitem.[Name]
	FROM dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gitem.GroupId = gr.GroupId
	WHERE gr.GroupId = @GroupId

	SELECT item.JobDescription_GroupItemId AS GroupItemId
		,item.JobDescription_ItemId AS ItemId
		,item.[Name]
		,CASE WHEN setting.GroupId IS NOT NULL AND gr.GroupId IS NOT NULL THEN 1 ELSE 0 END AS IsSelected
		,ISNULL(setting.DisplayOrder, 999) AS DisplayOrder
	FROM dbo.JobDescription_Item AS item WITH (NOLOCK)
		INNER JOIN dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK) ON item.JobDescription_GroupItemId = gitem.JobDescription_GroupItemId
		AND gitem.GroupId = @GroupId
		LEFT JOIN dbo.JobDescription_GroupSetting AS setting WITH (NOLOCK) ON item.JobDescription_ItemId = setting.JobDescription_ItemId
		AND gitem.GroupId = setting.GroupId
		AND setting.JobLevelId = @JobLevelId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON setting.GroupId = gr.GroupId
		AND gr.GroupId = @GroupId
	ORDER BY ISNULL(setting.DisplayOrder, 999) ASC
END