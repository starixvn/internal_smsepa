/****** Object:  Stored Procedure dbo.Sync_Employee    Script Date: 5/16/2016 2:20:12 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Employee    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Employee    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Employee    Script Date: 4/6/2016 11:27:48 AM ******/
CREATE PROCEDURE [dbo].[Sync_Employee]	
	@UserXML NVARCHAR(MAX)

	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 20150828@Quinn.Vo
Task: Synchronize data Users from hrm.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
- @UserXML: <User EmpId='{0}' DomainUsername='{1}' Code='{2}' "
            + "NickName='{3}' FullName='{4}' TeamId='{5}' JobTitleId='{6}' Phone='{7}' "
            + " Email='{8}' CardNumber='{9}' IsEnabled='{10}' IsDeleted='{11}' Password='{12}' "
            + " JoinedDate='{13}' ModifiedDate='{14}' IsLocal='{15}'></User>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE 
		@SyncDate	DATETIME,
		@Xml	XML

	SET @SyncDate = GETDATE()
	DECLARE @DocHandler INT
	
	PRINT LEN(@UserXML)

	IF OBJECT_ID('tempdb..#Employee') IS NOT NULL
	BEGIN
		DROP TABLE #Employee
	END
	PRINT @UserXML
	CREATE TABLE #Employee (
		[EmployeeId] [int]  NULL,
		[DomainUsername] [varchar](50) NULL,
		[Code] [nvarchar](20)  NULL,
		[FullName] [nvarchar](150)  NULL,
		[NickName] [nvarchar](20) NULL,
		[GroupId] [int] NULL,
		[JobTitle] [int] NULL,
		[Phone] [varchar](50) NULL,
		[Email] [varchar](50) NULL,
		[CardNumber] [nvarchar](20) NULL,
		[Enabled] [bit]  NULL,
		[IsDeleted] [bit] NULL,
		[Password] [nvarchar](100) NULL,
		[JoinedDate] [datetime] NULL,
		[ModifiedDate] [datetime] NULL,
		[IsLocal] [bit] NULL
	)
 
	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@UserXML

		INSERT INTO #Employee ([EmployeeId]
			  ,[DomainUsername]
			  ,[Code]
			  ,[FullName]
			  ,[NickName]
			  ,[GroupId]
			  ,[JobTitle]
			  ,[Phone]
			  ,[Email]
			  ,[CardNumber]
			  ,[Enabled]
			  ,[IsDeleted]
			  ,[Password]
			  ,[JoinedDate]
			  ,[ModifiedDate]
			  ,[IsLocal])
		SELECT [EmpId]
			  ,DomainUsername
			  ,[Code]
			  ,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([FullName],'&amp;','&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			  ,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([NickName],'&amp;','&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			  ,TeamId
			  ,[JobTitleId]
			  ,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([Phone],'&amp;','&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			  ,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([Email],'&amp;','&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			  ,[CardNumber]
			  ,ISNULL(IsEnabled,1) IsEnabled
			  ,ISNULL(IsDeleted,0) IsDeleted
			  ,[Password]
			  ,CASE WHEN ISDATE([JoinedDate])=1 THEN [JoinedDate] ELSE NULL END [JoinedDate]
			  ,CASE WHEN ISDATE(ModifiedDate)= 1 THEN ModifiedDate ELSE NULL END ModifiedDate
			  ,[IsLocal]
		FROM OPENXML(@DocHandler, '/ROOT/User', 1) WITH (
				EmpId [int],
				DomainUsername varchar(50),
				Code [nvarchar](20),
				FullName [nvarchar](150),
				NickName [nvarchar](20),
				TeamId [int],
				JobTitleId [int], 
				Phone VARCHAR(50),
				Email VARCHAR(50),
				CardNumber NVARCHAR(20),
				IsEnabled [bit],
				IsDeleted [bit],
				Password [nvarchar](100),
				JoinedDate [nvarchar](100),
				ModifiedDate [datetime],
				[IsLocal] [bit]
				) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'
		PRINT 1
		RETURN
	END CATCH;
	 
	INSERT INTO [dbo].[Employee] (
		[EmployeeId]
		,[DomainUsername]
		,[Code]
		,[FullName]
		,[NickName]
		,[GroupId]
		,[JobTitle]
		,[Phone]
		,[Email]
		,[CardNumber]
		,[Enabled]
		,[IsDeleted]
		,[IsSpecialUser]
		,[Password]
		,[JoinedDate]
		,[ModifiedDate]
		,[SyncDate]
		,[IsLocal]
		)
	SELECT new.[EmployeeId]
		,new.[DomainUsername]
		,new.[Code]
		,new.[FullName]
		,new.[NickName]
		,new.[GroupId]
		,new.[JobTitle]
		,new.[Phone]
		,new.[Email]
		,new.[CardNumber]
		,new.[Enabled]
		,ISNULL(new.[isDeleted], 0) [isDeleted]
		,0
		,new.[Password]
		,new.[JoinedDate]
		,new.ModifiedDate
		,@SyncDate
		,new.IsLocal
	FROM #Employee new
		LEFT JOIN [dbo].[Employee] curr WITH (NOLOCK) ON new.[EmployeeId] = curr.[EmployeeId]
	WHERE curr.[EmployeeId] IS NULL 

	UPDATE curr  
	SET curr.DomainUsername= ISNULL(new.DomainUsername, '') 
		, curr.Code = ISNULL(new.Code,'')	
		, curr.FullName = ISNULL(new.FullName,'')	
		, curr.NickName = ISNULL(new.NickName,'')
		, curr.GroupId = ISNULL(new.GroupId, 0)
		, curr.JobTitle	= new.[JobTitle]
		, curr.Phone	= new.[Phone]
		, curr.Email	= new.[Email]
		, curr.CardNumber	= new.[CardNumber]
		, curr.Enabled = ISNULL(new.Enabled, '')
		, curr.IsDeleted =ISNULL(new.IsDeleted, 0)	
		, curr.Password = new.Password
		, curr.JoinedDate = ISNULL(new.JoinedDate,'2011-01-01') 
		, curr.[ModifiedDate]=new.[ModifiedDate]
		, curr.SyncDate = @SyncDate
		,curr.IsLocal = new.IsLocal
	FROM [dbo].[Employee] curr WITH (ROWLOCK, UPDLOCK)
			INNER JOIN  #Employee new  ON new.[EmployeeId] = curr.[EmployeeId]

	INSERT INTO [dbo].[EmployeeRole](EmployeeId, RoleId)
	SELECT ee.EmployeeId, 6
	FROM [dbo].[Employee] ee 
	LEFT JOIN [dbo].[EmployeeRole] er
		ON ee.EmployeeId = er.EmployeeId
				AND ER.RoleId = 6
	WHERE er.EmployeeId IS NULL 

	SELECT @ErrCode = 0, @ErrMsg = ''
END