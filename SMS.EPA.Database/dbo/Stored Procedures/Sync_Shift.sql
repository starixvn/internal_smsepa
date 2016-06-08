/****** Object:  Stored Procedure dbo.Sync_Shift    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Shift    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Shift    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Shift    Script Date: 4/6/2016 11:27:49 AM ******/
CREATE PROCEDURE [dbo].[Sync_Shift]	
	@ShiftXML NVARCHAR(MAX)

	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20150828@Quinn.Vo
Task: Synchronize data shift from hrm.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
- @ShiftXML: "<Shift ShiftId='{0}' Code='{1}' Name='{2}' "
            + "BeginCheckInTime='{3}' EndCheckInTime='{4}' BeginCheckOutTime='{5}' EndCheckOutTime='{6}' IsInAfterDay='{7}' "
            + " IsOutAfterDay='{8}' ModifiedDate='{9}' IsDisabled='{10}'></Shift>"
*/

BEGIN
	SET NOCOUNT ON;

	DECLARE 
		@SyncDate	DATETIME,
		@Xml	XML		 

	SET @SyncDate = GETDATE()
	
	DECLARE @DocHandler INT
	
	IF OBJECT_ID('tempdb..#Shift') IS NOT NULL
	BEGIN
		DROP TABLE #Shift
	END 
	
	CREATE TABLE #Shift(
		[ShiftID] [int] NOT NULL,
		[Code] [varchar](20) NOT NULL,
		[Name] [nvarchar](100) NOT NULL,
		[BeginCheckInTime] [nchar](10) NULL,
		[EndCheckInTime] [nchar](10) NULL,
		[BeginCheckOutTime] [nchar](10) NULL,
		[EndCheckOutTime] [nchar](10) NULL,
		[IsInAfterDay] [bit] NULL,
		[IsOutAfterDay] [bit] NULL,
		IsDisabled [bit] NULL,
		[ModifiedDate] datetime)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@ShiftXML

		INSERT INTO #Shift (
		  [ShiftID]
		  ,[Code]
		  ,[Name]
		  ,[BeginCheckInTime]
		  ,[EndCheckInTime]
		  ,[BeginCheckOutTime]
		  ,[EndCheckOutTime]
		  ,[IsInAfterDay]
		  ,[IsOutAfterDay]
		  ,[ModifiedDate]
		  ,IsDisabled)
		SELECT [ShiftID]
			  ,[Code]
			  ,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Name,'&amp;','&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			  ,[BeginCheckInTime]
			  ,[EndCheckInTime]
			  ,[BeginCheckOutTime]
			  ,[EndCheckOutTime]
			  ,ISNULL([IsInAfterDay],0) [IsInAfterDay]
			  ,ISNULL([IsOutAfterDay],0) [IsOutAfterDay]
			  ,CASE WHEN ISDATE(ModifiedDate )=1 THEN ModifiedDate  ELSE NULL END [ModifiedDate]
			  ,ISNULL(IsDisabled,0) IsDisabled
		FROM OPENXML(@DocHandler, '/ROOT/Shift', 1) WITH (
					ShiftId [int],
					Code [varchar](20),
					Name [nvarchar](100),
					BeginCheckInTime [nchar](10) ,
					EndCheckInTime [nchar](10) ,
					BeginCheckOutTime [nchar](10) ,
					EndCheckOutTime [nchar](10) ,
					IsInAfterDay [bit] ,
					IsOutAfterDay [bit],
					ModifiedDate varchar(100),
					IsDisabled [bit]
				) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	INSERT INTO [dbo].HRMShift(
		  [ShiftID]
		  ,[Code]
		  ,[Name]
		  ,[BeginCheckInTime]
		  ,[EndCheckInTime]
		  ,[BeginCheckOutTime]
		  ,[EndCheckOutTime]
		  ,[IsInAfterDay]
		  ,[IsOutAfterDay]
		  ,IsDisabled
		  ,[ModifiedDate]
		  ,SyncDate)
	SELECT new.[ShiftID]
		  ,new.[Code]
		  ,new.[Name]
		  ,new.[BeginCheckInTime]
		  ,new.[EndCheckInTime]
		  ,new.[BeginCheckOutTime]
		  ,new.[EndCheckOutTime]
		  ,new.[IsInAfterDay]
		  ,new.[IsOutAfterDay]
		  ,new.IsDisabled
		  ,new.[ModifiedDate]
		  ,@SyncDate
	FROM #Shift new
		LEFT JOIN  [dbo].[HRMShift] curr WITH (NOLOCK) ON new.[ShiftID] = curr.[ShiftID]
	WHERE curr.[ShiftID] IS NULL 
	
	UPDATE curr  
	SET curr.[Code] = new.[Code],
		curr.[Name] = new.[Name],
		curr.[BeginCheckInTime] = new.[BeginCheckInTime],
		curr.[EndCheckInTime] = new.[EndCheckInTime],
		curr.[BeginCheckOutTime] = new.[BeginCheckOutTime],
		curr.[EndCheckOutTime] = new.[EndCheckOutTime],
		curr.[IsInAfterDay] = new.[IsInAfterDay],
		curr.[IsOutAfterDay] = new.[IsOutAfterDay],
		curr.IsDisabled = new.IsDisabled,
		curr.[ModifiedDate]=new.[ModifiedDate],
		curr.SyncDate = @SyncDate
	FROM [dbo].[HRMShift] curr WITH (ROWLOCK, UPDLOCK) 
		INNER JOIN #Shift new ON new.[ShiftID] = curr.[ShiftID]
		
	DELETE curr 
	FROM [dbo].[HRMShift] curr WITH (NOLOCK)
		LEFT JOIN #Shift new ON new.[ShiftID] = curr.[ShiftID]
	WHERE new.ShiftID IS NULL

	SELECT @ErrCode = 0, @ErrMsg = ''
END