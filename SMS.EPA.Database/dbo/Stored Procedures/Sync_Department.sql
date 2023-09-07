
CREATE PROCEDURE [dbo].[Sync_Department] @DepartmentXml NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150828@Quinn.Vo
Task: Synchronize data departments from hrm.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
- @DepartmentXml: "<Department DepartmentId='{0}' Code='{1}' "
            + "Name='{2}' ParentId='{3}' ManagerId='{4}' DepartmentLevel='{5}' IsActive='{6}' ModifiedDate='{7}'></Department>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME = GETDATE()
		,@Xml XML
	DECLARE @DocHandler INT

	CREATE TABLE #tblDepartment (
		DepartmentId INT NULL
		,Code VARCHAR(50) NULL
		,[Name] NVARCHAR(200) NULL
		,ParentId INT NULL
		,ManagerId INT NULL
		,DepartmentLevel INT NULL
		,IsActive BIT NULL
		,ModifiedDate DATETIME
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@DepartmentXml

		INSERT INTO #tblDepartment (
			DepartmentId
			,Code
			,[Name]
			,ParentId
			,ManagerId
			,DepartmentLevel
			,IsActive
			,ModifiedDate
			)
		SELECT DepartmentId
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Code, '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([Name], '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,CASE 
				WHEN ParentId = 0 THEN NULL
				ELSE ParentId
				END
			,CASE 
				WHEN ManagerId = 0 THEN NULL
				ELSE ManagerId
				END
			,CASE 
				WHEN DepartmentLevel = 0 THEN NULL
				ELSE DepartmentLevel
				END
			,IsActive
			,ModifiedDate
		FROM OPENXML(@DocHandler, '/ROOT/Department', 1) WITH (DepartmentId INT, Code VARCHAR(50), [Name] NVARCHAR(200), ParentId INT, ManagerId INT, DepartmentLevel INT, IsActive BIT, ModifiedDate DATETIME) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	INSERT INTO dbo.Department (
		DepartmentId
		,Code
		,[Name]
		,ParentId
		,ManagerId
		,DepartmentLevel
		,IsActive
		,ModifiedDate
		,SyncDate
		)
	SELECT new.DepartmentId
		,new.Code
		,new.[Name]
		,new.ParentId
		,new.ManagerId
		,new.DepartmentLevel
		,new.IsActive
		,new.ModifiedDate
		,@SyncDate
	FROM #tblDepartment AS new WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.Department AS curr WITH (NOLOCK)
			WHERE new.DepartmentId = curr.DepartmentId
			)

	UPDATE curr
	SET curr.Code = new.Code
		,curr.[Name] = new.[Name]
		,curr.ParentId = new.ParentId
		,curr.ManagerId = new.ManagerId
		,curr.DepartmentLevel = new.DepartmentLevel
		,curr.IsActive = ISNULL(new.IsActive, 0)
		,curr.ModifiedDate = new.ModifiedDate
		,curr.SyncDate = @SyncDate
	FROM dbo.Department AS curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblDepartment AS new WITH (NOLOCK) ON curr.DepartmentId = new.DepartmentId

	-- update left, right for team/department
	EXEC dbo.Sync_CalculateLeftRightGroupDepartment

	-- section
	INSERT INTO dbo.Section (
		SectionId
		,Code
		,[Name]
		,DepartmentId
		,IsActive
		,ModifiedDate
		,SyncDate
		)
	SELECT new.DepartmentId
		,new.Code
		,new.[Name]
		,new.ParentId
		,new.IsActive
		,new.ModifiedDate
		,@SyncDate
	FROM dbo.Department AS new WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS team WITH (NOLOCK) ON team.DepartmentId = new.DepartmentId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.Section AS curr WITH (NOLOCK)
			WHERE new.DepartmentId = curr.SectionId
			)

	UPDATE curr
	SET curr.Code = new.Code
		,curr.[Name] = new.[Name]
		,curr.DepartmentId = new.ParentId
		,curr.IsActive = ISNULL(new.IsActive, 0)
		,curr.ModifiedDate = new.ModifiedDate
		,curr.SyncDate = @SyncDate
	FROM dbo.Section AS curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.[Group] AS team WITH (NOLOCK) ON team.DepartmentId = curr.SectionId
		INNER JOIN dbo.Department AS new WITH (NOLOCK) ON curr.SectionId = new.DepartmentId

	-- department
	INSERT INTO dbo.Department_New (
		DepartmentId
		,Code
		,[Name]
		,UnitId
		,IsActive
		,ModifiedDate
		,SyncDate
		)
	SELECT new.DepartmentId
		,new.Code
		,new.[Name]
		,new.ParentId
		,new.IsActive
		,new.ModifiedDate
		,@SyncDate
	FROM dbo.Department AS new WITH (NOLOCK)
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON new.DepartmentId = sec.DepartmentId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.Department_New AS curr WITH (NOLOCK)
			WHERE new.DepartmentId = curr.DepartmentId
			)

	UPDATE curr
	SET curr.Code = new.Code
		,curr.[Name] = new.[Name]
		,curr.UnitId = new.ParentId
		,curr.IsActive = ISNULL(new.IsActive, 0)
		,curr.ModifiedDate = new.ModifiedDate
		,curr.SyncDate = @SyncDate
	FROM dbo.Department_New AS curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON curr.DepartmentId = sec.DepartmentId
		INNER JOIN dbo.Department AS new WITH (NOLOCK) ON curr.DepartmentId = new.DepartmentId

	-- unit
	INSERT INTO dbo.Unit (
		UnitId
		,Code
		,[Name]
		,IsActive
		,ModifiedDate
		,SyncDate
		)
	SELECT new.DepartmentId
		,new.Code
		,new.[Name]
		,new.IsActive
		,new.ModifiedDate
		,@SyncDate
	FROM dbo.Department AS new WITH (NOLOCK)
		INNER JOIN dbo.Department_New AS dept WITH (NOLOCK) ON new.DepartmentId = dept.UnitId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.Unit AS curr WITH (NOLOCK)
			WHERE new.DepartmentId = curr.UnitId
			)

	UPDATE curr
	SET curr.Code = new.Code
		,curr.[Name] = new.[Name]
		,curr.IsActive = ISNULL(new.IsActive, 0)
		,curr.ModifiedDate = new.ModifiedDate
		,curr.SyncDate = @SyncDate
	FROM dbo.Unit AS curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Department_New AS dept WITH (NOLOCK) ON curr.UnitId = dept.UnitId
		INNER JOIN dbo.Department AS new WITH (NOLOCK) ON curr.UnitId = new.DepartmentId

	DROP TABLE #tblDepartment
END