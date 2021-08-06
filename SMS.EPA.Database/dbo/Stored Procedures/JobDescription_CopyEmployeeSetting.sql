
CREATE PROCEDURE [dbo].[JobDescription_CopyEmployeeSetting] @FromEmployeeId INT
	,@ToEmployeeId INT
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210805@Tim.Trinh
Task: job description - copy employee setting
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @JobLevelId INT
		,@GroupId INT

	SELECT @JobLevelId = JobLevelId
		,@GroupId = GroupId
	FROM dbo.Employee WITH (NOLOCK)
	WHERE EmployeeId = @ToEmployeeId

	CREATE TABLE #tblSetting (GroupItemId INT, GroupItemName NVARCHAR(500), ItemId INT, ItemName NVARCHAR(500), DisplayOrder SMALLINT)

	INSERT INTO #tblSetting (GroupItemId, GroupItemName, ItemId, ItemName, DisplayOrder)
	SELECT gitem.JobDescription_GroupItemId, gitem.[Name], item.JobDescription_ItemId, item.[Name], setting.DisplayOrder
	FROM dbo.JobDescription_EmployeeSetting AS setting WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON setting.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.JobDescription_Item AS item WITH (NOLOCK) ON setting.JobDescription_ItemId = item.JobDescription_ItemId
		AND item.IsActive = 1
		INNER JOIN dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK) ON item.JobDescription_GroupItemId = gitem.JobDescription_GroupItemId
		AND emp.GroupId = gitem.GroupId
		AND gitem.IsActive = 1
	WHERE setting.EmployeeId = @FromEmployeeId

	INSERT INTO dbo.JobDescription_GroupItem (GroupId, [Name], IsActive)
	SELECT DISTINCT @GroupId, setting.GroupItemName, 1
	FROM #tblSetting AS setting WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK)
			WHERE gitem.GroupId = @GroupId
				AND setting.GroupItemName = gitem.Name
		)

	UPDATE setting
	SET setting.GroupItemId = gitem.JobDescription_GroupItemId
	FROM #tblSetting AS setting WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK) ON setting.GroupItemName = gitem.[Name]
		AND gitem.GroupId = @GroupId

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

	IF ISNULL(@JobLevelId, 0) > 0
	BEGIN
		DELETE
		FROM dbo.JobDescription_EmployeeSetting WITH (ROWLOCK, UPDLOCK)
		WHERE EmployeeId = @ToEmployeeId

		INSERT INTO dbo.JobDescription_EmployeeSetting (EmployeeId, JobLevelId, JobDescription_ItemId, DisplayOrder)
		SELECT @ToEmployeeId, @JobLevelId, ItemId, DisplayOrder
		FROM #tblSetting WITH (NOLOCK)
	END

	DROP TABLE #tblSetting
END