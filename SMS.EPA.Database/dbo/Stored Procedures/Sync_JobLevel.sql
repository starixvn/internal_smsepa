CREATE PROCEDURE [dbo].[Sync_JobLevel] @JobLevelXml NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210426@Lilac.Nguyen
Task: Synchronize data job level from hrm.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
- @JobLevelXml: "<JobLevel JobLevelId='{0}' Name='{1}' IsActive='{2}' ModifiedDate='{3}'></JobLevel>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME = GETDATE()
		,@Xml XML
	DECLARE @DocHandler INT

	CREATE TABLE #tblJobLevel (
		JobLevelId INT NULL
		,[Name] NVARCHAR(150) NULL
		,IsActive BIT NULL
		,ModifiedDate DATETIME
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@JobLevelXml

		INSERT INTO #tblJobLevel (
			JobLevelId
			,[Name]
			,IsActive
			,ModifiedDate
			)
		SELECT JobLevelId
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([Name], '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,IsActive
			,ModifiedDate
		FROM OPENXML(@DocHandler, '/ROOT/JobLevel', 1) WITH (JobLevelId INT, [Name] NVARCHAR(150), IsActive BIT, ModifiedDate DATETIME) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	INSERT INTO dbo.JobLevel(
		JobLevelId
		,[Name]
		,IsActive
		,ModifiedDate
		,SyncDate
		)
	SELECT new.JobLevelId
		,new.[Name]
		,new.IsActive
		,new.ModifiedDate
		,@SyncDate
	FROM #tblJobLevel AS new WITH (NOLOCK)
		LEFT JOIN dbo.JobLevel AS curr WITH (NOLOCK) ON new.JobLevelId = curr.JobLevelId
	WHERE curr.JobLevelId IS NULL

	UPDATE curr
	SET curr.[Name] = new.[Name]
		,curr.IsActive = ISNULL(new.IsActive, 0)
		,curr.ModifiedDate = new.ModifiedDate
		,curr.SyncDate = @SyncDate
	FROM dbo.JobLevel AS curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblJobLevel AS new WITH (NOLOCK) ON curr.JobLevelId = new.JobLevelId

	DROP TABLE #tblJobLevel
END