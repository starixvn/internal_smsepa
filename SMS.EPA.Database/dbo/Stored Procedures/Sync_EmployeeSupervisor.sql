CREATE PROCEDURE [dbo].[Sync_EmployeeSupervisor]	
	@SupervisorXml NVARCHAR(MAX)

	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 20150828@Quinn.Vo
Task: Synchronize data employee supervisors from hrm.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
- @SupervisorXml: "<Supervisor OwnerId='{0}' EmpId='{1}' ActionType='{2}' ModifiedDate='{3}' OrderNo='{4}' OrderCode='{5}' IsDeleted='{7}'></Supervisor>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE 
		@SyncDate	DATETIME,
		@Xml	XML		 

	SET @SyncDate = GETDATE()
	DECLARE @DocHandler INT
	
	IF OBJECT_ID('tempdb..#EmployeeSupervisor') IS NOT NULL
	BEGIN
		DROP TABLE #EmployeeSupervisor
	END 
	
	CREATE TABLE #EmployeeSupervisor(
		[OwnerId] INT NOT NULL,
		[EmpId] INT NOT NULL,
		[ActionType] TINYINT NOT NULL,
		[RequestType] TINYINT NOT NULL,
		[ModifiedDate] DATETIME,
		[OrderNo] SMALLINT NULL,
		[OrderCode] VARCHAR(10) NULL,
		[IsDeleted] BIT NULL)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@SupervisorXml

		INSERT INTO #EmployeeSupervisor (
			[OwnerId]
			  ,[EmpId]
			  ,[ActionType]
			  ,[RequestType]
			  ,[ModifiedDate]
			  ,[OrderNo]
			  ,[OrderCode]
			  ,[IsDeleted])
		SELECT [OwnerId]
			  ,[EmpId]
			  ,[ActionType]
			  ,[RequestType]
			  ,[ModifiedDate]
			  ,CASE WHEN [OrderNo] = 0 THEN NULL ELSE [OrderNo] END
			  ,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([OrderCode],'&amp;','&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			  ,[IsDeleted]
		FROM OPENXML(@DocHandler, '/ROOT/Supervisor', 1) WITH (
				[OwnerId] INT,
				[EmpId] INT,
				[ActionType] TINYINT,
				[RequestType] TINYINT,
				[ModifiedDate] DATETIME,
				[OrderNo] SMALLINT,
				[OrderCode] VARCHAR(10),
				[IsDeleted] BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;
	 
	INSERT INTO [dbo].[HRMEmployeeSupervisor]
           ([OwnerId]
			,[EmpId]
			,[ActionType]
			,[RequestType]
			,[ModifiedDate]
			,[OrderNo]
			,[OrderCode]
		    ,[SyncDate])
	SELECT new.[OwnerId]
			,new.[EmpId]
			,new.[ActionType]
			,new.[RequestType]
			,new.[ModifiedDate]
			,new.[OrderNo]
			,new.[OrderCode]
			,@SyncDate
	FROM #EmployeeSupervisor new
		LEFT JOIN  [dbo].[HRMEmployeeSupervisor] curr WITH (NOLOCK) 
		ON new.OwnerId = curr.OwnerId 
		AND new.[EmpId] = curr.[EmpId]
		AND new.[ActionType] = curr.[ActionType]
		AND new.[RequestType] = curr.[RequestType]
	WHERE curr.OwnerId IS NULL 
	
	DELETE curr 
	FROM [dbo].[HRMEmployeeSupervisor] AS curr
	INNER JOIN #EmployeeSupervisor AS new 
	ON curr.EmpId = new.EmpId
	AND curr.OwnerId = new.OwnerId
	AND new.[ActionType] = curr.[ActionType]
	AND new.[RequestType] = curr.[RequestType]
	AND new.IsDeleted = 1

	UPDATE curr  
	SET curr.[OwnerId] = new.[OwnerId],
		curr.[EmpId] = new.[EmpId],
		curr.[ActionType] = new.[ActionType],
		curr.[RequestType] = new.[RequestType],
		curr.[ModifiedDate] = new.[ModifiedDate],
		curr.[OrderNo] = new.[OrderNo],
		curr.[OrderCode] = new.[OrderCode],
		
		curr.SyncDate = @SyncDate
	FROM [dbo].[HRMEmployeeSupervisor] curr WITH (ROWLOCK, UPDLOCK) 
		INNER JOIN #EmployeeSupervisor new 
		ON new.OwnerId = curr.OwnerId 
		AND new.[EmpId] = curr.[EmpId]
		AND new.[ActionType] = curr.[ActionType]
		AND new.[RequestType] = curr.[RequestType]

	SELECT @ErrCode = 0, @ErrMsg = ''
	
	IF OBJECT_ID('tempdb..#EmployeeSupervisor') IS NOT NULL
	BEGIN
		DROP TABLE #EmployeeSupervisor
	END 
END
