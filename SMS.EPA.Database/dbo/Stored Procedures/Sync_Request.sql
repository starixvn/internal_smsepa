/****** Object:  Stored Procedure dbo.Sync_Request    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Request    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Request    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Request    Script Date: 4/6/2016 11:27:49 AM ******/
CREATE PROCEDURE [dbo].[Sync_Request]
	@RequestXml NVARCHAR(MAX)

	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 20150828@Quinn.Vo
Task: Synchronize data requests from hrm.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
- @RequestXml: "<Request RequestId='{0}' EmployeeId='{1}' RequestType='{2}' Remark='{3}' CurrentStatus='{4}' VerifierId='{5}' ApproverId='{6}'"
            + " HRId='{7}' VerifiedDate='{8}' ApprovedDate='{9}' HRDate='{10}' VerifiedStatus='{10}' ApprovedStatus='{11}'"
            + " HRStatus='{11}' CreatedBy='{12}' CreatedDate='{13}' ModifiedBy='{14}' ModifiedDate='{15}' ReworkCount='{16}'></Request>"
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
		,[VerifierId] INT
		,[ApproverId] INT
		,[HRId] INT
		,[VerifiedDate] DATETIME
		,[ApprovedDate] DATETIME
		,[HRDate] DATETIME
		,[VerifiedStatus] TINYINT
		,[ApprovedStatus] TINYINT
		,[HRStatus] TINYINT
		,[CreatedBy] INT
		,[CreatedDate] DATETIME
		,[ModifiedBy] INT
		,[ModifiedDate] DATETIME
		,[ReworkCount] INT)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@RequestXml

		INSERT INTO #Request (
			  [RequestId]
			  ,[EmployeeId]
			  ,[RequestType]
			  ,[Remark]
			  ,[CurrentStatus]
			  ,[VerifierId]
			  ,[ApproverId]
			  ,[HRId]
			  ,[VerifiedDate]
			  ,[ApprovedDate]
			  ,[HRDate]
			  ,[VerifiedStatus]
			  ,[ApprovedStatus]
			  ,[HRStatus]
			  ,[CreatedBy]
			  ,[CreatedDate]
			  ,[ModifiedBy]
			  ,[ModifiedDate]
			  ,[ReworkCount])
		SELECT [RequestId]
			  ,[EmployeeId]
			  ,[RequestType]
			  ,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([Remark],'&amp;','&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			  ,[CurrentStatus]
			  ,[VerifierId]
			  ,[ApproverId]
			  ,[HRId]
			  ,CASE WHEN DATEDIFF(D, [VerifiedDate], '1900-01-01') = 0 THEN NULL ELSE [VerifiedDate] END
			  ,CASE WHEN DATEDIFF(D, [ApprovedDate], '1900-01-01') = 0 THEN NULL ELSE [ApprovedDate] END
			  ,CASE WHEN DATEDIFF(D, [HRDate], '1900-01-01') = 0 THEN NULL ELSE [HRDate] END
			  ,[VerifiedStatus]
			  ,[ApprovedStatus]
			  ,[HRStatus]
			  ,[CreatedBy]
			  ,[CreatedDate]
			  ,[ModifiedBy]
			  ,[ModifiedDate]
			  ,[ReworkCount]
		FROM OPENXML(@DocHandler, '/ROOT/Request', 1) WITH (
				[RequestId] BIGINT
				,[EmployeeId] INT
				,[RequestType] INT
				,[Remark] NVARCHAR(2000)
				,[CurrentStatus] TINYINT
				,[VerifierId] INT
				,[ApproverId] INT
				,[HRId] INT
				,[VerifiedDate] DATETIME
				,[ApprovedDate] DATETIME
				,[HRDate] DATETIME
				,[VerifiedStatus] TINYINT
				,[ApprovedStatus] TINYINT
				,[HRStatus] TINYINT
				,[CreatedBy] INT
				,[CreatedDate] DATETIME
				,[ModifiedBy] INT
				,[ModifiedDate] DATETIME
				,[ReworkCount] INT
				) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;
	 
	INSERT INTO [dbo].[HRMEmployeeRequest]
           ( [RequestId]
			  ,[EmployeeId]
			  ,[RequestType]
			  ,[Remark]
			  ,[CurrentStatus]
			  ,[VerifierId]
			  ,[ApproverId]
			  ,[HRId]
			  ,[VerifiedDate]
			  ,[ApprovedDate]
			  ,[HRDate]
			  ,[VerifiedStatus]
			  ,[ApprovedStatus]
			  ,[HRStatus]
			  ,[CreatedBy]
			  ,[CreatedDate]
			  ,[ModifiedBy]
			  ,[ModifiedDate]
			  ,[ReworkCount]
			  ,SyncDate)
	SELECT new.[RequestId]
			,new.[EmployeeId]
			,new.[RequestType]
			,new.[Remark]
			,new.[CurrentStatus]
			,new.[VerifierId]
			,new.[ApproverId]
			,new.[HRId]
			,new.[VerifiedDate]
			,new.[ApprovedDate]
			,new.[HRDate]
			,new.[VerifiedStatus]
			,new.[ApprovedStatus]
			,new.[HRStatus]
			,new.[CreatedBy]
			,new.[CreatedDate]
			,new.[ModifiedBy]
			,new.[ModifiedDate]
			,new.[ReworkCount]
			,@SyncDate
	FROM #Request new
		LEFT JOIN  [dbo].[HRMEmployeeRequest] curr WITH (NOLOCK) 
			ON new.[RequestId] = curr.[RequestId]
	WHERE curr.[RequestId] IS NULL 
	
	UPDATE curr  
	SET curr.[EmployeeId] = new.[EmployeeId],
		curr.[RequestType] = new.[RequestType],
		curr.[Remark] = new.[Remark],
		curr.[CurrentStatus] = new.[CurrentStatus],
		curr.[VerifierId] = new.[VerifierId],
		curr.[ApproverId] = new.[ApproverId],
		curr.[HRId] = new.[HRId],
		curr.[VerifiedDate] = new.[VerifiedDate],
		curr.[ApprovedDate] = new.[ApprovedDate],
		curr.[HRDate] = new.[HRDate],
		curr.[VerifiedStatus] = new.[VerifiedStatus],
		curr.[ApprovedStatus] = new.[ApprovedStatus],
		curr.[HRStatus] = new.[HRStatus],
		curr.[CreatedBy] = new.[CreatedBy],
		curr.[CreatedDate] = new.[CreatedDate],
		curr.[ModifiedBy] = new.[ModifiedBy],
		curr.[ModifiedDate] = new.[ModifiedDate],
		curr.[ReworkCount] = new.[ReworkCount],
		curr.SyncDate = @SyncDate
	FROM [dbo].[HRMEmployeeRequest] curr WITH (ROWLOCK, UPDLOCK) 
		INNER JOIN #Request new ON curr.[RequestId] = new.[RequestId]

	SELECT @ErrCode = 0, @ErrMsg = ''
	
	IF OBJECT_ID('tempdb..#Request') IS NOT NULL
	BEGIN
		DROP TABLE #Request
	END 
END