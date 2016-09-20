
CREATE PROCEDURE [dbo].[Sync_Group] @TeamXML NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150828@Quinn.Vo
Task: Synchronize data groups from hrm.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
- @TeamXML: "<Team TeamId='{0}' Code='{1}' "
            + "Name='{2}' LeaderId='{3}' AssistantLeaderId='{4}' Rank='{5}' IsDeleted='{6}' ModifiedDate='{7}' DepartmentId='{8}'></Team>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME
		,@Xml XML

	SET @SyncDate = GETDATE()

	DECLARE @DocHandler INT

	IF OBJECT_ID('tempdb..#Group') IS NOT NULL
	BEGIN
		DROP TABLE #Group
	END

	CREATE TABLE #Group (
		[GroupId] [int] NULL
		,[Code] [varchar](20) NULL
		,[Name] [nvarchar](150) NULL
		,[LeaderId] [int] NULL
		,[AssistantLeaderId] [int] NULL
		,[DepartmentId] [int] NULL
		,[Rank] [int] NULL
		,[IsDeleted] [bit] NULL
		,[ModifiedDate] DATETIME
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@TeamXML

		INSERT INTO #Group (
			[GroupId]
			,[Code]
			,[Name]
			,[LeaderId]
			,[AssistantLeaderId]
			,[DepartmentId]
			,[Rank]
			,[IsDeleted]
			,[ModifiedDate]
			)
		SELECT TeamId
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([Code], '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(NAME, '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,CASE 
				WHEN [LeaderId] = 0 THEN NULL
				ELSE [LeaderId]
				END
			,CASE 
				WHEN [AssistantLeaderId] = 0 THEN NULL
				ELSE [AssistantLeaderId]
				END
			,CASE 
				WHEN [DepartmentId] = 0 THEN NULL
				ELSE [DepartmentId]
				END
			,[Rank]
			,ISNULL(IsDeleted, 0) IsDeleted
			,ModifiedDate [ModifiedDate]
		FROM OPENXML(@DocHandler, '/ROOT/Team', 1) WITH (TeamId [int], Code [nvarchar](520), NAME [nvarchar](520), LeaderId [int], AssistantLeaderId [int], [DepartmentId] [int], [Rank] [int], IsDeleted [bit], ModifiedDate DATETIME) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	INSERT INTO [dbo].[Group] (
		[GroupId]
		,[Code]
		,[Name]
		,[LeaderId]
		,[AssistantLeaderId]
		,[DepartmentId]
		,[Rank]
		,[IsDeleted]
		,lft
		,rgt
		,[ModifiedDate]
		,[SyncDate]
		)
	SELECT new.[GroupId]
		,new.[Code]
		,new.[Name]
		,new.LeaderId
		,new.AssistantLeaderId
		,new.[DepartmentId]
		,new.[Rank]
		,new.[IsDeleted]
		,0
		,0
		,new.[ModifiedDate]
		,@SyncDate
	FROM #Group new
		LEFT JOIN [dbo].[Group] curr WITH (NOLOCK) ON new.[GroupId] = curr.[GroupId]
	WHERE curr.[GroupId] IS NULL

	UPDATE curr
	SET curr.[Code] = new.[Code]
		,curr.[Name] = new.[Name]
		,curr.[LeaderId] = new.[LeaderId]
		,curr.[AssistantLeaderId] = new.[AssistantLeaderId]
		,curr.[DepartmentId] = new.[DepartmentId]
		,curr.[Rank] = new.[Rank]
		,curr.[IsDeleted] = ISNULL(new.[IsDeleted], 0)
		,curr.[ModifiedDate] = new.[ModifiedDate]
		,curr.SyncDate = @SyncDate
	FROM [dbo].[Group] curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #Group new ON curr.[GroupId] = new.[GroupId]

	-- update left, right for team/department
	EXEC [dbo].[Sync_CalculateLeftRightGroupDepartment]

	IF OBJECT_ID('tempdb..#Group') IS NOT NULL
	BEGIN
		DROP TABLE #Group
	END
END