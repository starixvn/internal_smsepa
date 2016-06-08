
CREATE PROCEDURE [dbo].[Sync_GroupSupervisor]	
	@SupervisorXml NVARCHAR(MAX)

	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 20150828@Quinn.Vo
Task: Synchronize data group supervisors from hrm.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
- @SupervisorXml: "<Supervisor OwnerId='{0}' GroupId='{1}' ActionType='{2}' RequestType='{3}' ModifiedDate='{4}' IsDeleted='{5}'></Supervisor>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE 
		@SyncDate	DATETIME,
		@Xml	XML		 

	SET @SyncDate = GETDATE()
	DECLARE @DocHandler INT
	
	IF OBJECT_ID('tempdb..#GroupSupervisor') IS NOT NULL
	BEGIN
		DROP TABLE #GroupSupervisor
	END 
	
	CREATE TABLE #GroupSupervisor(
		[OwnerId] INT NOT NULL,
		[GroupId] INT NOT NULL,
		[ActionType] TINYINT NOT NULL,
		[RequestType] TINYINT NOT NULL,
		[ModifiedDate] DATETIME,
		[IsDeleted] BIT NULL)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@SupervisorXml

		INSERT INTO #GroupSupervisor (
			[OwnerId]
			  ,[GroupId]
			  ,[ActionType]
			  ,[RequestType]
			  ,[ModifiedDate]
			  ,[IsDeleted])
		SELECT [OwnerId]
			  ,[GroupId]
			  ,[ActionType]
			  ,[RequestType]
			  ,[ModifiedDate]
			  ,[IsDeleted]
		FROM OPENXML(@DocHandler, '/ROOT/Supervisor', 1) WITH (
				[OwnerId] INT,
				[GroupId] INT,
				[ActionType] TINYINT,
				[RequestType] TINYINT,
				[ModifiedDate] DATETIME,
				[IsDeleted] BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;
	 
	INSERT INTO [dbo].[HRMGroupSupervisor]
           ([OwnerId]
			,[GroupId]
			,[ActionType]
			,[RequestType]
			,[ModifiedDate]
		    ,[SyncDate])
	SELECT new.[OwnerId]
			,new.[GroupId]
			,new.[ActionType]
			,new.[RequestType]
			,new.[ModifiedDate]
			,@SyncDate
	FROM #GroupSupervisor new
		LEFT JOIN  [dbo].[HRMGroupSupervisor] curr WITH (NOLOCK) 
		ON new.OwnerId = curr.OwnerId 
		AND new.[GroupId] = curr.[GroupId]
		AND new.[ActionType] = curr.[ActionType]
		AND new.[RequestType] = curr.[RequestType]
	WHERE curr.OwnerId IS NULL 
	
	DELETE curr 
	FROM [dbo].[HRMGroupSupervisor] AS curr
	INNER JOIN #GroupSupervisor AS new 
	ON curr.GroupId = new.GroupId
	AND curr.OwnerId = new.OwnerId
	AND new.[ActionType] = curr.[ActionType]
	AND new.[RequestType] = curr.[RequestType]
	AND new.IsDeleted = 1

	UPDATE curr  
	SET curr.[OwnerId] = new.[OwnerId],
		curr.[GroupId] = new.[GroupId],
		curr.[ActionType] = new.[ActionType],
		curr.[RequestType] = new.[RequestType],
		curr.[ModifiedDate] = new.[ModifiedDate],
		curr.SyncDate = @SyncDate
	FROM [dbo].[HRMGroupSupervisor] curr WITH (ROWLOCK, UPDLOCK) 
		INNER JOIN #GroupSupervisor new 
		ON new.OwnerId = curr.OwnerId 
		AND new.[GroupId] = curr.[GroupId]
		AND new.[ActionType] = curr.[ActionType]
		AND new.[RequestType] = curr.[RequestType]

	SELECT @ErrCode = 0, @ErrMsg = ''
	
	IF OBJECT_ID('tempdb..#GroupSupervisor') IS NOT NULL
	BEGIN
		DROP TABLE #GroupSupervisor
	END 
END