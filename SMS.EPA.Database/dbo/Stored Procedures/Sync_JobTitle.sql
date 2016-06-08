/****** Object:  Stored Procedure dbo.Sync_JobTitle    Script Date: 5/16/2016 2:20:12 PM ******/
/****** Object:  Stored Procedure dbo.Sync_JobTitle    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.Sync_JobTitle    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.Sync_JobTitle    Script Date: 4/6/2016 11:27:48 AM ******/
CREATE PROCEDURE [dbo].[Sync_JobTitle]	
	@JobTitleXml NVARCHAR(MAX)

	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 20150828@Quinn.Vo
Task: Synchronize data Job Title from hrm.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
- @JobTitleXml: "<JobTitle JobTitleId='{0}' LookupId='{1}' "
            + "Code='{2}' Name='{3}' Value='{4}'></JobTitle>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE 
		@SyncDate	DATETIME,
		@Xml	XML		 

	SET @SyncDate = GETDATE()
	DECLARE @DocHandler INT
	
	IF OBJECT_ID('tempdb..#JobTitle') IS NOT NULL
	BEGIN
		DROP TABLE #JobTitle
	END 
	
	CREATE TABLE #JobTitle(
		[Id] [int] NULL,
		[LookupId] [int] NULL,
		[Code] [varchar](20) NULL,
		[Name] [nvarchar](100) NULL,
		[Value] [varchar](500) NULL)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@JobTitleXml

		INSERT INTO #JobTitle (
			  [Id]
			  ,[LookupId]
			  ,[Code]
			  ,[Name]
			  ,[Value])
		SELECT [JobTitleId]
			  ,[LookupId]
			  ,[Code]
			  ,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Name,'&amp;','&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			  ,[Value]
		FROM OPENXML(@DocHandler, '/ROOT/JobTitle', 1) WITH (
				[JobTitleId] [int],
				[LookupId] [int],
				Code [varchar](20),
				Name [nvarchar](100),
				[Value] [varchar](500)
				) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;
	 
	INSERT INTO [dbo].[HRMJobTitle]
           ( [Id]
			  ,[LookupId]
			  ,[Code]
			  ,[Name]
			  ,[Value]
			  ,SyncDate)
	SELECT new.Id
			,new.LookupId
			,new.[Code]
			,new.[Name]
			,new.[Value]
			,@SyncDate
	FROM #JobTitle new
		LEFT JOIN  [dbo].[HRMJobTitle] curr WITH (NOLOCK) ON new.Id = curr.Id
	WHERE curr.Id IS NULL 
	
	UPDATE curr  
	SET curr.[Code] = new.[Code],
		curr.[Name] = new.[Name],
		curr.[LookupId] = new.[LookupId],
		curr.[Value] = new.[Value],
		curr.SyncDate = @SyncDate
	FROM [dbo].[HRMJobTitle] curr WITH (ROWLOCK, UPDLOCK) 
		INNER JOIN #JobTitle new ON curr.Id = new.Id

	SELECT @ErrCode = 0, @ErrMsg = ''
	
	IF OBJECT_ID('tempdb..#JobTitle') IS NOT NULL
	BEGIN
		DROP TABLE #JobTitle
	END 
END