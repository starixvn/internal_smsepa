
CREATE PROCEDURE [dbo].[Sync_Employee] @UserXML NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150828@Quinn.Vo
Task: Synchronize data Users from hrm.
DB: SMSEPA
Original:

Revisions:
20160414@Razor.nguyen: Create EPA Service API
Param's Explanation:
- @UserXML: <ROOT><Row EmployeeId='{0}' DomainUsername='{1}' Code='{2}' "
		NickName='{3}' FullName='{4}' TeamId='{5}' JobTitleId='{6}' Phone='{7}' Email='{8}'"
		CardNumber='{9}' IsEnabled='{10}' IsDeleted='{11}' Password='{12}' JoinedDate='{13}' ModifiedDate='{14}' IsLocal='{15}'"
		ResignedDate = '{16}' ConfirmedDate='{17}' JobLevelId='{18}' UserId='{19}' MilitaryFromDate='{20}' MilitaryToDate='{21}'/></ROOT>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME = GETDATE()
		,@Xml XML
	DECLARE @DocHandler INT

	CREATE TABLE #tmpEmployee (
		EmployeeId INT NULL
		,UserId INT NULL
		,DomainUsername NVARCHAR(50) NULL
		,Code NVARCHAR(20) NULL
		,FullName NVARCHAR(150) NULL
		,NickName NVARCHAR(20) NULL
		,GroupId INT NULL
		,JobTitle INT NULL
		,Phone VARCHAR(50) NULL
		,Email VARCHAR(50) NULL
		,CardNumber VARCHAR(20) NULL
		,[Enabled] BIT NULL
		,IsDeleted BIT NULL
		,[Password] NVARCHAR(100) NULL
		,JoinedDate DATE NULL
		,ModifiedDate DATETIME NULL
		,IsLocal BIT NULL
		,ResignedDate DATE NULL
		,ConfirmedDate DATE NULL
		,JobLevelId INT NULL
		,MilitaryFromDate DATE NULL
		,MilitaryToDate DATE NULL
		)

	CREATE TABLE #tmpChangedEmployee (
		UserId INT
		,EmployeeId INT
		,DomainUsername NVARCHAR(50)
		,Code VARCHAR(20)
		,Nickname NVARCHAR(20)
		,FullName NVARCHAR(150)
		,[Password] VARCHAR(100)
		,Email VARCHAR(50)
		,IsEnabled BIT
		,IsDeleted BIT
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@UserXML

		INSERT INTO #tmpEmployee (
			UserId
			,EmployeeId
			,DomainUsername
			,Code
			,FullName
			,NickName
			,GroupId
			,JobTitle
			,Phone
			,Email
			,CardNumber
			,[Enabled]
			,IsDeleted
			,[Password]
			,IsLocal
			,JobLevelId
			,JoinedDate
			,ModifiedDate
			,ResignedDate
			,ConfirmedDate
			,MilitaryFromDate
			,MilitaryToDate
			)
		SELECT UserId
			,EmployeeId
			,DomainUsername
			,Code
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(FullName, '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(NickName, '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,TeamId
			,JobTitleId
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Phone, '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Email, '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,CardNumber
			,ISNULL(IsEnabled, 1) IsEnabled
			,ISNULL(IsDeleted, 0) IsDeleted
			,[Password]
			,IsLocal
			,JobLevelId
			,CASE 
				WHEN JoinedDate = CONVERT(DATE, '1900-01-01') THEN NULL
				ELSE JoinedDate
				END
			,CASE 
				WHEN ModifiedDate = CONVERT(DATETIME, '1900-01-01 00:00:00.000') THEN NULL
				ELSE ModifiedDate
				END
			,CASE 
				WHEN ResignedDate = CONVERT(DATE, '1900-01-01') THEN NULL
				ELSE ResignedDate
				END
			,CASE 
				WHEN ConfirmedDate = CONVERT(DATETIME, '1900-01-01') THEN NULL
				ELSE ConfirmedDate
				END
			,CASE 
				WHEN MilitaryFromDate = CONVERT(DATETIME, '1900-01-01') THEN NULL
				ELSE MilitaryFromDate
				END
			,CASE 
				WHEN MilitaryToDate = CONVERT(DATETIME, '1900-01-01') THEN NULL
				ELSE MilitaryToDate
				END
		FROM OPENXML(@DocHandler, '/ROOT/Row', 1) WITH (UserId INT, EmployeeId INT, DomainUsername NVARCHAR(50)
			,Code NVARCHAR(20), FullName NVARCHAR(150), NickName NVARCHAR(20)
			,TeamId INT, JobTitleId INT, Phone VARCHAR(50), Email VARCHAR(50)
			,CardNumber VARCHAR(20), IsEnabled BIT, IsDeleted BIT, [Password] NVARCHAR(100)
			,JoinedDate DATE, ModifiedDate DATE, IsLocal BIT, ResignedDate DATE
			,ConfirmedDate DATE, JobLevelId INT, MilitaryFromDate DATE, MilitaryToDate DATE) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	INSERT INTO dbo.Employee (
		UserId
		,EmployeeId
		,DomainUsername
		,Code
		,FullName
		,NickName
		,GroupId
		,JobTitle
		,Phone
		,Email
		,CardNumber
		,[Enabled]
		,IsDeleted
		,IsSpecialUser
		,[Password]
		,JoinedDate
		,ModifiedDate
		,SyncDate
		,IsLocal
		,ResignedDate
		,ConfirmedDate
		,JobLevelId
		,MilitaryFromDate
		,MilitaryToDate
		)
	OUTPUT INSERTED.UserId
		,INSERTED.EmployeeId
		,INSERTED.DomainUsername
		,INSERTED.Code
		,INSERTED.NickName
		,INSERTED.FullName
		,INSERTED.[Password]
		,INSERTED.Email
		,INSERTED.[Enabled]
		,INSERTED.IsDeleted
	INTO #tmpChangedEmployee(UserId, EmployeeId, DomainUsername, Code, Nickname, FullName, [Password], Email, IsEnabled, IsDeleted)
	SELECT new.UserId
		,new.EmployeeId
		,new.DomainUsername
		,new.Code
		,new.FullName
		,new.NickName
		,new.GroupId
		,new.JobTitle
		,new.Phone
		,new.Email
		,new.CardNumber
		,new.[Enabled]
		,ISNULL(new.isDeleted, 0)
		,0
		,new.[Password]
		,new.JoinedDate
		,new.ModifiedDate
		,@SyncDate
		,new.IsLocal
		,new.ResignedDate
		,new.ConfirmedDate
		,new.JobLevelId
		,new.MilitaryFromDate
		,new.MilitaryToDate
	FROM #tmpEmployee AS new WITH (NOLOCK)
	WHERE NOT EXISTS (
		SELECT 1
		FROM dbo.Employee AS curr WITH (NOLOCK)
		WHERE new.EmployeeId = curr.EmployeeId
		)

	INSERT INTO #tmpChangedEmployee(UserId, EmployeeId, DomainUsername, Code, Nickname, FullName, [Password], Email, IsEnabled, IsDeleted)
	SELECT CASE 
			WHEN ISNULL(new.UserId, 0) > 0 THEN new.UserId
			ELSE curr.UserId
			END
		,new.EmployeeId
		,new.DomainUsername
		,new.Code
		,new.NickName
		,new.FullName
		,new.[Password]
		,new.Email
		,new.[Enabled]
		,new.IsDeleted
	FROM dbo.Employee AS curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpEmployee AS new WITH (NOLOCK) ON new.EmployeeId = curr.EmployeeId
	WHERE ISNULL(curr.DomainUsername, '') != ISNULL(new.DomainUsername, '')
		OR ISNULL(curr.Code, '') != ISNULL(new.Code, '')
		OR ISNULL(curr.NickName, '') != ISNULL(new.NickName, '')
		OR ISNULL(curr.FullName, '') != ISNULL(new.FullName, '')
		OR ISNULL(curr.[Password], '') != ISNULL(new.[Password], '')
		OR ISNULL(curr.Email, '') != ISNULL(new.Email, '')
		OR curr.[Enabled] != new.[Enabled]
		OR curr.IsDeleted != new.IsDeleted

	UPDATE curr
	SET curr.UserId = CASE 
			WHEN ISNULL(new.UserId, 0) > 0 THEN new.UserId
			ELSE curr.UserId
			END
		,curr.DomainUsername = ISNULL(new.DomainUsername, '')
		,curr.Code = ISNULL(new.Code, '')
		,curr.FullName = ISNULL(new.FullName, '')
		,curr.NickName = ISNULL(new.NickName, '')
		,curr.GroupId = ISNULL(new.GroupId, 0)
		,curr.JobTitle = new.JobTitle
		,curr.Phone = new.Phone
		,curr.Email = new.Email
		,curr.CardNumber = new.CardNumber
		,curr.[Enabled] = ISNULL(new.[Enabled], 0)
		,curr.IsDeleted = ISNULL(new.IsDeleted, 0)
		,curr.[Password] = new.[Password]
		,curr.JoinedDate = ISNULL(new.JoinedDate, '2011-01-01')
		,curr.ModifiedDate = new.ModifiedDate
		,curr.SyncDate = @SyncDate
		,curr.IsLocal = new.IsLocal
		,curr.ResignedDate = new.ResignedDate
		,curr.ConfirmedDate = new.ConfirmedDate
		,curr.JobLevelId = new.JobLevelId
		,curr.MilitaryFromDate = new.MilitaryFromDate
		,curr.MilitaryToDate = new.MilitaryToDate
	FROM dbo.Employee AS curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpEmployee AS new WITH (NOLOCK) ON new.EmployeeId = curr.EmployeeId

	INSERT INTO dbo.EmployeeRole (
		EmployeeId
		,RoleId
		)
	SELECT e.EmployeeId
		,6
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON e.EmployeeId = er.EmployeeId
		AND er.RoleId = 6
	WHERE er.EmployeeId IS NULL

	SELECT UserId
		,EmployeeId
		,DomainUsername
		,Code
		,Nickname
		,FullName
		,[Password]
		,Email
		,IsEnabled
		,IsDeleted
	FROM #tmpChangedEmployee WITH (NOLOCK)

	DROP TABLE #tmpEmployee

	DROP TABLE #tmpChangedEmployee
END