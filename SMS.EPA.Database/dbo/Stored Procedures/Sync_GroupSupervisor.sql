
CREATE PROCEDURE [dbo].[Sync_GroupSupervisor] @SupervisorXml NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150828@Quinn.Vo
Task: Synchronize data group supervisors from hrm.
DB: SMSEPA
Original: 

Revisions:  
20160414@Razor.Nguyen: Implement EPA Service API
Param's Explanation:
- @SupervisorXml: "<Supervisor OwnerId='{0}' GroupId='{1}' ModifiedDate='{2}' IsDeleted='{3}'></Supervisor>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME
		,@Xml XML

	SET @SyncDate = GETDATE()

	DECLARE @DocHandler INT

	IF OBJECT_ID('tempdb..#GroupSupervisor') IS NOT NULL
	BEGIN
		DROP TABLE #GroupSupervisor
	END

	CREATE TABLE #GroupSupervisor (
		[OwnerId] INT NOT NULL
		,[GroupId] INT NOT NULL
		,[ModifiedDate] DATETIME
		,[IsDeleted] BIT
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@SupervisorXml

		INSERT INTO #GroupSupervisor (
			[OwnerId]
			,[GroupId]
			,[ModifiedDate]
			,[IsDeleted]
			)
		SELECT [OwnerId]
			,[GroupId]
			,[ModifiedDate]
			,[IsDeleted]
		FROM OPENXML(@DocHandler, '/ROOT/Supervisor', 1) WITH ([OwnerId] INT, [GroupId] INT, [ModifiedDate] DATETIME, [IsDeleted] BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	INSERT INTO [dbo].[HRMGroupSupervisor] (
		[OwnerId]
		,[GroupId]
		,[ModifiedDate]
		,[IsDeleted]
		,[SyncDate]
		)
	SELECT new.[OwnerId]
		,new.[GroupId]
		,new.[ModifiedDate]
		,new.[IsDeleted]
		,@SyncDate
	FROM #GroupSupervisor new
		LEFT JOIN [dbo].[HRMGroupSupervisor] curr WITH (NOLOCK) ON new.OwnerId = curr.OwnerId
		AND new.[GroupId] = curr.[GroupId]
	WHERE curr.OwnerId IS NULL

	UPDATE curr
	SET curr.[OwnerId] = new.[OwnerId]
		,curr.[GroupId] = new.[GroupId]
		,curr.[ModifiedDate] = new.[ModifiedDate]
		,curr.[IsDeleted] = new.[IsDeleted]
		,curr.SyncDate = @SyncDate
	FROM [dbo].[HRMGroupSupervisor] curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #GroupSupervisor new ON new.OwnerId = curr.OwnerId
		AND new.[GroupId] = curr.[GroupId]

	IF OBJECT_ID('tempdb..#GroupSupervisor') IS NOT NULL
	BEGIN
		DROP TABLE #GroupSupervisor
	END
END