/****** Object:  Stored Procedure dbo.Sync_RequestHistory    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.Sync_RequestHistory    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.Sync_RequestHistory    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.Sync_RequestHistory    Script Date: 4/6/2016 11:27:49 AM ******/
CREATE PROCEDURE [dbo].[Sync_RequestHistory]
	@RequestHistoryXml NVARCHAR(MAX)

	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 20150909@Quinn.Vo
Task: Synchronize data requests history from hrm.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
- @RequestHistoryXml: "<Request RequestId='{0}' EmployeeId='{1}' RequestType='{2}' Remark='{3}' CurrentStatus='{4}' InsertedDate='{5}'></Request>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE 
		@SyncDate	DATETIME,
		@Xml	XML		 

	SET @SyncDate = GETDATE()
	DECLARE @DocHandler INT
	
	IF OBJECT_ID('tempdb..#Request') IS NOT NULL
	BEGIN
		DROP TABLE #Request
	END 
	
	CREATE TABLE #Request(
		[RequestId] BIGINT
		,[EmployeeId] INT
		,[RequestType] INT
		,[Remark] NVARCHAR(2000)
		,[CurrentStatus] TINYINT
		,[InsertedDate] DATETIME)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@RequestHistoryXml

		INSERT INTO #Request (
			  [RequestId]
			  ,[EmployeeId]
			  ,[RequestType]
			  ,[Remark]
			  ,[CurrentStatus]
			  ,[InsertedDate])
		SELECT [RequestId]
			  ,[EmployeeId]
			  ,[RequestType]
			  ,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([Remark],'&amp;','&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			  ,[CurrentStatus]
			  ,[InsertedDate]
		FROM OPENXML(@DocHandler, '/ROOT/Request', 1) WITH (
				[RequestId] BIGINT
				,[EmployeeId] INT
				,[RequestType] INT
				,[Remark] NVARCHAR(2000)
				,[CurrentStatus] TINYINT
				,[InsertedDate] DATETIME
				) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;
	 
	INSERT INTO [dbo].[HRMEmployeeRequestHistory]
           ( [RequestId]
			  ,[EmployeeId]
			  ,[RequestType]
			  ,[Remark]
			  ,[CurrentStatus]
			  ,[InsertedDate]
			  ,SyncDate)
	SELECT new.[RequestId]
			,new.[EmployeeId]
			,new.[RequestType]
			,new.[Remark]
			,new.[CurrentStatus]
			,new.[InsertedDate]
			,@SyncDate
	FROM #Request new
		LEFT JOIN  [dbo].[HRMEmployeeRequestHistory] curr WITH (NOLOCK) 
			ON new.[RequestId] = curr.[RequestId]
	WHERE curr.[RequestId] IS NULL 
	
	SELECT @ErrCode = 0, @ErrMsg = ''
	
	IF OBJECT_ID('tempdb..#Request') IS NOT NULL
	BEGIN
		DROP TABLE #Request
	END 
END