
CREATE PROCEDURE [dbo].[JobDescription_UpdateGroupSetting] @GroupId INT
	,@JobLevelId INT
	,@SettingXml NVARCHAR(MAX) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210429@Tim.Trinh
Task: job description - edit group setting
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
	@SettingXml = <ROOT>
			<Row ItemId='1' DisplayOrder='1' />
		</ROOT>
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblSetting (ItemId INT, DisplayOrder SMALLINT)

	IF ISNULL (@SettingXml, '') <> ''
	BEGIN
		DECLARE @docHandler INT

		EXEC sp_xml_preparedocument @docHandler OUTPUT
			,@SettingXml

		INSERT INTO #tblSetting (ItemId, DisplayOrder)
		SELECT ItemId, DisplayOrder
		FROM OPENXML(@DocHandler, '/ROOT/Row', 1) WITH (ItemId INT, DisplayOrder SMALLINT) AS Data

		EXEC sp_xml_removedocument @docHandler
	END

	DELETE
	FROM dbo.JobDescription_GroupSetting WITH (ROWLOCK, UPDLOCK)
	WHERE GroupId = @GroupId AND JobLevelId = @JobLevelId

	INSERT INTO dbo.JobDescription_GroupSetting (GroupId, JobLevelId, JobDescription_ItemId, DisplayOrder)
	SELECT @GroupId, @JobLevelId, ItemId, DisplayOrder
	FROM #tblSetting WITH (NOLOCK)

	DROP TABLE #tblSetting
END