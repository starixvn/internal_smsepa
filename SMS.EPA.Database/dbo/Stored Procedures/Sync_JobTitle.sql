
CREATE PROCEDURE [dbo].[Sync_JobTitle] @JobTitleXml NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150828@Quinn.Vo
Task: Synchronize data Job Title from hrm.
DB: NexEPA
Original: 

Revisions:  
20160414@Razor.Nguyen: Implement EPA Service API
Param's Explanation:
- @JobTitleXml: "<JobTitle JobTitleId='{0}' Code='{1}' Name='{2}'></JobTitle>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME
		,@Xml XML

	SET @SyncDate = GETDATE()

	DECLARE @DocHandler INT

	IF OBJECT_ID('tempdb..#JobTitle') IS NOT NULL
	BEGIN
		DROP TABLE #JobTitle
	END

	CREATE TABLE #JobTitle (
		[Id] [int] NULL
		,[Code] [varchar](20) NULL
		,[Name] [nvarchar](100) NULL
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@JobTitleXml

		INSERT INTO #JobTitle (
			[Id]
			,[Code]
			,[Name]
			)
		SELECT [JobTitleId]
			,[Code]
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([Name], '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
		FROM OPENXML(@DocHandler, '/ROOT/JobTitle', 1) WITH ([JobTitleId] [int], Code [varchar](20), [Name] [nvarchar](100)) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	INSERT INTO [dbo].[HRMJobTitle] (
		[Id]
		,LookupId
		,[Code]
		,[Name]
		,SyncDate
		)
	SELECT new.Id
		,1
		,new.[Code]
		,new.[Name]
		,@SyncDate
	FROM #JobTitle new
		LEFT JOIN [dbo].[HRMJobTitle] curr WITH (NOLOCK) ON new.Id = curr.Id
	WHERE curr.Id IS NULL

	UPDATE curr
	SET curr.[Code] = new.[Code]
		,curr.[Name] = new.[Name]
		,curr.SyncDate = @SyncDate
	FROM [dbo].[HRMJobTitle] curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #JobTitle new ON curr.Id = new.Id

	IF OBJECT_ID('tempdb..#JobTitle') IS NOT NULL
	BEGIN
		DROP TABLE #JobTitle
	END
END