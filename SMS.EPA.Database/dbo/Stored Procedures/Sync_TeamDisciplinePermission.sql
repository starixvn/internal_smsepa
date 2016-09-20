
CREATE PROCEDURE [dbo].[Sync_TeamDisciplinePermission] @SupervisorXml NVARCHAR(MAX)
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

	IF OBJECT_ID('tempdb..#TeamDisciplinePermission') IS NOT NULL
	BEGIN
		DROP TABLE #TeamDisciplinePermission
	END

	CREATE TABLE #TeamDisciplinePermission (
		[OwnerId] INT NOT NULL
		,[TeamId] INT NOT NULL
		,[ModifiedDate] DATETIME
		,[IsDeleted] BIT
		,[SyncDate] DATETIME
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@SupervisorXml

		INSERT INTO #TeamDisciplinePermission (
			[OwnerId]
			,[TeamId]
			,[ModifiedDate]
			,[IsDeleted]
			,[SyncDate]
			)
		SELECT [OwnerId]
			,[GroupId]
			,[ModifiedDate]
			,[IsDeleted]
			,@SyncDate
		FROM OPENXML(@DocHandler, '/ROOT/Supervisor', 1) WITH ([OwnerId] INT, [GroupId] INT, [ModifiedDate] DATETIME, [IsDeleted] BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	DELETE curr
	FROM [dbo].[TeamDisciplinePermission] AS curr
		INNER JOIN #TeamDisciplinePermission AS new ON curr.[TeamId] = new.[TeamId]
		AND curr.OwnerId = new.OwnerId
		AND new.IsDeleted = 1

	UPDATE curr
	SET curr.[ModifiedDate] = new.[ModifiedDate]
		,curr.SyncDate = @SyncDate
	FROM [dbo].[TeamDisciplinePermission] curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #TeamDisciplinePermission new ON new.OwnerId = curr.OwnerId
		AND new.[TeamId] = curr.[TeamId]

	INSERT INTO [dbo].[TeamDisciplinePermission] (
		[OwnerId]
		,[TeamId]
		,[ModifiedDate]
		,[IsDeleted]
		,[SyncDate]
		)
	SELECT new.[OwnerId]
		,new.[TeamId]
		,new.[ModifiedDate]
		,new.[IsDeleted]
		,@SyncDate
	FROM #TeamDisciplinePermission new
		LEFT JOIN [dbo].[TeamDisciplinePermission] curr WITH (NOLOCK) ON new.OwnerId = curr.OwnerId
		AND new.[TeamId] = curr.[TeamId]
	WHERE curr.OwnerId IS NULL
		AND ISNULL(new.[IsDeleted], 0) = 0

	IF OBJECT_ID('tempdb..#TeamDisciplinePermission') IS NOT NULL
	BEGIN
		DROP TABLE #TeamDisciplinePermission
	END
END