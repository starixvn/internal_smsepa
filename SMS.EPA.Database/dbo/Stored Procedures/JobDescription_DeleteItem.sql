
CREATE PROCEDURE [dbo].[JobDescription_DeleteItem] @ItemId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210510@Lilac.Nguyen
Task: Delete Job Description Item.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DELETE
	FROM dbo.JobDescription_EmployeeSetting WITH (ROWLOCK, UPDLOCK)
	WHERE JobDescription_ItemId = @ItemId

	DELETE
	FROM dbo.JobDescription_GroupSetting WITH (ROWLOCK, UPDLOCK)
	WHERE JobDescription_ItemId = @ItemId

	DELETE
	FROM dbo.JobDescription_Item WITH (ROWLOCK, UPDLOCK)
	WHERE JobDescription_ItemId = @ItemId
END