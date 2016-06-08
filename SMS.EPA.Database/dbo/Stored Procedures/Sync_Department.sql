/****** Object:  Stored Procedure dbo.Sync_Department    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Department    Script Date: 5/16/2016 2:15:33 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Department    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Department    Script Date: 4/6/2016 11:27:48 AM ******/
CREATE PROCEDURE [dbo].[Sync_Department]	
	@DepartmentXml NVARCHAR(MAX)

	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg nvarchar(200) = '' OUTPUT
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

	DECLARE 
		@SyncDate	DATETIME,
		@Xml	XML		 

	SET @SyncDate = GETDATE()
	DECLARE @DocHandler INT
	
	IF OBJECT_ID('tempdb..#Department') IS NOT NULL
	BEGIN
		DROP TABLE #Department
	END 
	
	CREATE TABLE #Department(
		[DepartmentId] [int] NULL,
		[Code] [varchar](50) NULL,
		[Name] [nvarchar](200) NULL,
		[ParentId] [int] NULL,
		[ManagerId] [int] NULL,
		[DepartmentLevel] [int] NULL,
		[IsActive] [bit] NULL,
		[ModifiedDate] DATETIME)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@DepartmentXml

		INSERT INTO #Department (
			[DepartmentId]
			  ,[Code]
			  ,[Name]
			  ,[ParentId]
			  ,[ManagerId]
			  ,[DepartmentLevel]
			  ,[IsActive]
			  ,[ModifiedDate])
		SELECT [DepartmentId]
			  ,[Code]
			  ,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Name,'&amp;','&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			  ,CASE WHEN [ParentId] = 0 THEN NULL ELSE [ParentId] END
			  ,CASE WHEN [ManagerId] = 0 THEN NULL ELSE [ManagerId] END
			  ,CASE WHEN [DepartmentLevel] = 0 THEN NULL ELSE [DepartmentLevel] END
			  ,[IsActive]
			  ,[ModifiedDate]
		FROM OPENXML(@DocHandler, '/ROOT/Department', 1) WITH (
				DepartmentId [int],
				Code [nvarchar](50),
				Name [nvarchar](200),
				ParentId [int],
				ManagerId [int],
				[DepartmentLevel] [int],
				[IsActive] [bit],
				ModifiedDate DATETIME
				) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;
	 
	INSERT INTO [dbo].[Department]
           ([DepartmentId]
			,[Code]
			,[Name]
			,[ParentId]
			,[ManagerId]
			,[DepartmentLevel]
			,[IsActive]
			,[ModifiedDate]
		   ,[SyncDate])
	SELECT new.[DepartmentId]
			,new.[Code]
			,new.[Name]
			,new.[ParentId]
			,new.[ManagerId]
			,new.[DepartmentLevel]
			,new.[IsActive]
			,new.[ModifiedDate]
			,@SyncDate
	FROM #Department new
		LEFT JOIN  [dbo].[Department] curr WITH (NOLOCK) ON new.[DepartmentId] = curr.[DepartmentId]
	WHERE curr.[DepartmentId] IS NULL 
	
	UPDATE curr  
	SET curr.[Code] = new.[Code],
		curr.[Name] = new.[Name],
		curr.[ParentId] = new.[ParentId],
		curr.[ManagerId] = new.[ManagerId],
		curr.[DepartmentLevel] = new.[DepartmentLevel],
		curr.[IsActive] = ISNULL(new.[IsActive], 0),
		curr.[ModifiedDate]=new.[ModifiedDate],
		curr.SyncDate = @SyncDate
	FROM [dbo].[Department] curr WITH (ROWLOCK, UPDLOCK) 
		INNER JOIN #Department new ON curr.[DepartmentId] = new.[DepartmentId]
	
	-- update left, right for team/department

	SELECT @ErrCode = 0, @ErrMsg = ''
	
	IF OBJECT_ID('tempdb..#Department') IS NOT NULL
	BEGIN
		DROP TABLE #Department
	END 
END