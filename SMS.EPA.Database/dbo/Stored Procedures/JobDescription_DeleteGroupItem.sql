
CREATE PROCEDURE [dbo].[JobDescription_DeleteGroupItem] @GroupItemId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210429@Lilac.Nguyen
Task: Delete Job Description Group Item.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DELETE setting
	FROM dbo.JobDescription_EmployeeSetting AS setting WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.JobDescription_Item AS item WITH (NOLOCK) ON setting.JobDescription_ItemId = item.JobDescription_ItemId
		AND item.JobDescription_GroupItemId = @GroupItemId

	DELETE setting
	FROM dbo.JobDescription_GroupSetting AS setting WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.JobDescription_Item AS item WITH (NOLOCK) ON setting.JobDescription_ItemId = item.JobDescription_ItemId
		AND item.JobDescription_GroupItemId = @GroupItemId

	DELETE
	FROM dbo.JobDescription_GroupItem WITH (ROWLOCK, UPDLOCK)
	WHERE JobDescription_GroupItemId = @GroupItemId

	DELETE
	FROM dbo.JobDescription_Item WITH (ROWLOCK, UPDLOCK)
	WHERE JobDescription_GroupItemId = @GroupItemId
END