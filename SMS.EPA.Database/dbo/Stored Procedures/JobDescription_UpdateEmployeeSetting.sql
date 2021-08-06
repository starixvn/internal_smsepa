

CREATE PROCEDURE [dbo].[JobDescription_UpdateEmployeeSetting] @EmployeeId INT
	,@SettingXml NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210512@Lilac.Nguyen
Task: job description - edit employee setting
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

	DECLARE @docHandler INT, @JobLevelId INT
	CREATE TABLE #tblSetting (ItemId INT, DisplayOrder SMALLINT)

	IF ISNULL (@SettingXml, '') <> ''
	BEGIN
		EXEC sp_xml_preparedocument @docHandler OUTPUT
			,@SettingXml

		INSERT INTO #tblSetting (ItemId, DisplayOrder)
		SELECT ItemId, DisplayOrder
		FROM OPENXML(@DocHandler, '/ROOT/Row', 1) WITH (ItemId INT, DisplayOrder SMALLINT) AS Data

		EXEC sp_xml_removedocument @docHandler
	END

	SELECT @JobLevelId = JobLevelId
	FROM dbo.Employee WITH(NOLOCK)
	WHERE EmployeeId = @EmployeeId

	DELETE
	FROM  dbo.JobDescription_EmployeeSetting WITH (ROWLOCK, UPDLOCK)
	WHERE EmployeeId = @EmployeeId

	INSERT INTO dbo.JobDescription_EmployeeSetting(EmployeeId, JobLevelId, JobDescription_ItemId, DisplayOrder)
	SELECT @EmployeeId, @JobLevelId, ItemId, DisplayOrder
	FROM #tblSetting WITH (NOLOCK)

	DROP TABLE #tblSetting
END