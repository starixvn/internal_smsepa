
CREATE PROCEDURE [dbo].[JobDescription_CopyGroupSetting] @FromGroupId INT
	,@ToGroupId INT
	,@JobLevelId INT = NULL
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210805@Tim.Trinh
Task: job description - copy group setting
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblSetting (JobLevelId INT, GroupItemId INT, GroupItemName NVARCHAR(500), ItemId INT, ItemName NVARCHAR(500), DisplayOrder SMALLINT)

	INSERT INTO #tblSetting (JobLevelId, GroupItemId, GroupItemName, ItemId, ItemName, DisplayOrder)
	SELECT setting.JobLevelId, gitem.JobDescription_GroupItemId, gitem.[Name], item.JobDescription_ItemId, item.[Name], setting.DisplayOrder
	FROM dbo.JobDescription_GroupSetting AS setting WITH (NOLOCK)
		INNER JOIN dbo.JobDescription_Item AS item WITH (NOLOCK) ON setting.JobDescription_ItemId = item.JobDescription_ItemId
		AND item.IsActive = 1
		INNER JOIN dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK) ON item.JobDescription_GroupItemId = gitem.JobDescription_GroupItemId
		AND setting.GroupId = gitem.GroupId
		AND gitem.IsActive = 1
	WHERE setting.GroupId = @FromGroupId
		AND (@JobLevelId IS NULL OR setting.JobLevelId = @JobLevelId)

	INSERT INTO dbo.JobDescription_GroupItem (GroupId, [Name], IsActive)
	SELECT DISTINCT @ToGroupId, setting.GroupItemName, 1
	FROM #tblSetting AS setting WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK)
			WHERE gitem.GroupId = @ToGroupId
				AND setting.GroupItemName = gitem.Name
		)

	UPDATE setting
	SET setting.GroupItemId = gitem.JobDescription_GroupItemId
	FROM #tblSetting AS setting WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK) ON gitem.GroupId = @ToGroupId
		AND setting.GroupItemName = gitem.[Name]

	INSERT INTO dbo.JobDescription_Item (JobDescription_GroupItemId, [Name], IsActive)
	SELECT DISTINCT setting.GroupItemId, setting.ItemName, 1
	FROM #tblSetting AS setting WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.JobDescription_Item AS item WITH (NOLOCK)
			WHERE setting.GroupItemId = item.JobDescription_GroupItemId
				AND setting.ItemName = item.[Name]
		)

	UPDATE setting
	SET setting.ItemId = item.JobDescription_ItemId
	FROM #tblSetting AS setting WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.JobDescription_Item AS item WITH (NOLOCK) ON setting.GroupItemId = item.JobDescription_GroupItemId
		AND setting.ItemName = item.[Name]

	DELETE
	FROM dbo.JobDescription_GroupSetting WITH (ROWLOCK, UPDLOCK)
	WHERE GroupId = @ToGroupId
		AND (@JobLevelId IS NULL OR JobLevelId = @JobLevelId)

	INSERT INTO dbo.JobDescription_GroupSetting (GroupId, JobLevelId, JobDescription_ItemId, DisplayOrder)
	SELECT @ToGroupId, JobLevelId, ItemId, DisplayOrder
	FROM #tblSetting WITH (NOLOCK)

	DROP TABLE #tblSetting
END