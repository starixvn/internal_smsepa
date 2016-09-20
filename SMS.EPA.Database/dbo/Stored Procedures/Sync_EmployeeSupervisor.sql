
CREATE PROCEDURE [dbo].[Sync_EmployeeSupervisor] @SupervisorXml NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150828@Quinn.Vo
Task: Synchronize data employee supervisors from hrm.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
- @SupervisorXml: "<Supervisor OwnerId='{0}' EmployeeId='{1}' ModifiedDate='{2}' IsDeleted='{3}'></Supervisor>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME
		,@Xml XML

	SET @SyncDate = GETDATE()

	DECLARE @DocHandler INT

	IF OBJECT_ID('tempdb..#EmployeeDisciplinePermission') IS NOT NULL
	BEGIN
		DROP TABLE #EmployeeDisciplinePermission
	END

	CREATE TABLE #EmployeeDisciplinePermission (
		[OwnerId] INT NOT NULL
		,[EmpId] INT NOT NULL
		,[ModifiedDate] DATETIME
		,[IsDeleted] BIT
		,[SyncDate] DATETIME
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@SupervisorXml

		INSERT INTO #EmployeeDisciplinePermission (
			[OwnerId]
			,[EmpId]
			,[ModifiedDate]
			,[IsDeleted]
			,[SyncDate]
			)
		SELECT [OwnerId]
			,[EmployeeId]
			,[ModifiedDate]
			,[IsDeleted]
			,@SyncDate
		FROM OPENXML(@DocHandler, '/ROOT/Supervisor', 1) WITH ([OwnerId] INT, [EmployeeId] INT, [ModifiedDate] DATETIME, [IsDeleted] BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	INSERT INTO [dbo].[EmployeeDisciplinePermission] (
		[OwnerId]
		,[EmpId]
		,[ModifiedDate]
		,[IsDeleted]
		,[SyncDate]
		)
	SELECT new.[OwnerId]
		,new.[EmpId]
		,new.[ModifiedDate]
		,new.[IsDeleted]
		,@SyncDate
	FROM #EmployeeDisciplinePermission new
		LEFT JOIN [dbo].[EmployeeDisciplinePermission] curr WITH (NOLOCK) ON new.OwnerId = curr.OwnerId
		AND new.[EmpId] = curr.[EmpId]
	WHERE curr.OwnerId IS NULL

	UPDATE curr
	SET curr.[OwnerId] = new.[OwnerId]
		,curr.[EmpId] = new.[EmpId]
		,curr.[ModifiedDate] = new.[ModifiedDate]
		,curr.[IsDeleted] = new.[IsDeleted]
		,curr.SyncDate = @SyncDate
	FROM [dbo].[EmployeeDisciplinePermission] curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #EmployeeDisciplinePermission new ON new.OwnerId = curr.OwnerId
		AND new.[EmpId] = curr.[EmpId]

	IF OBJECT_ID('tempdb..#EmployeeDisciplinePermission') IS NOT NULL
	BEGIN
		DROP TABLE #EmployeeDisciplinePermission
	END
END