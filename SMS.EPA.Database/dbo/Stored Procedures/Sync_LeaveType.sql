/****** Object:  Stored Procedure dbo.Sync_LeaveType    Script Date: 7/21/2016 3:01:47 PM ******/
/****** Object:  Stored Procedure dbo.Sync_LeaveType    Script Date: 7/21/2016 2:46:01 PM ******/
CREATE PROCEDURE [dbo].[Sync_LeaveType]	
	@LeaveTypeXml NVARCHAR(MAX)

	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 20150828@Quinn.Vo
Task: Synchronize data leave type from hrm.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
- @LeaveTypeXml: "<JobTitle JobTitleId='{0}' LookupId='{1}' "
            + "Code='{2}' Name='{3}' Value='{4}'></JobTitle>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE 
		@SyncDate	DATETIME,
		@Xml	XML		 

	SET @SyncDate = GETDATE()
	DECLARE @DocHandler INT

	IF OBJECT_ID('tempdb..#LeaveType') IS NOT NULL
	BEGIN
		DROP TABLE #LeaveType
	END 
	
	CREATE TABLE #LeaveType(
		[LeaveTypeId] [int] NULL,
		[Code] [varchar](10) NULL,
		[Name] [nvarchar](100) NULL,
		[IsDisabled] [bit] NULL)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@LeaveTypeXml

		INSERT INTO #LeaveType (
			  [LeaveTypeId]
			  ,[Code]
			  ,[Name]
			  ,[IsDisabled])
		SELECT [LeaveTypeId]
			  ,[Code]
			  ,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Name,'&amp;','&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			  ,[IsDisabled]
		FROM OPENXML(@DocHandler, '/ROOT/LeaveType', 1) WITH (
				[LeaveTypeId] [int],
				Code [varchar](10),
				Name [nvarchar](100),
				[IsDisabled] [bit]
				) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;
	 
	INSERT INTO [dbo].[HRMLeaveType]
           (  [LeaveTypeId]
			  ,[Code]
			  ,[Name]
			  ,[IsDisabled]
			  ,SyncDate)
	SELECT new.[LeaveTypeId]
			,new.[Code]
			,new.[Name]
			,new.[IsDisabled]
			,@SyncDate
	FROM #LeaveType new
		LEFT JOIN  [dbo].[HRMLeaveType] curr WITH (NOLOCK) ON new.[LeaveTypeId] = curr.[LeaveTypeId]
	WHERE curr.[LeaveTypeId] IS NULL 
	
	UPDATE curr  
	SET curr.[Code] = new.[Code],
		curr.[Name] = new.[Name],
		curr.[IsDisabled] = new.[IsDisabled],
		curr.SyncDate = @SyncDate
	FROM [dbo].[HRMLeaveType] curr WITH (ROWLOCK, UPDLOCK) 
		INNER JOIN #LeaveType new ON curr.[LeaveTypeId] = new.[LeaveTypeId]

	SELECT @ErrCode = 0, @ErrMsg = ''
	
	IF OBJECT_ID('tempdb..#LeaveType') IS NOT NULL
	BEGIN
		DROP TABLE #LeaveType
	END 
END