/****** Object:  Stored Procedure dbo.Sync_Calendar    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Calendar    Script Date: 5/16/2016 2:15:33 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Calendar    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Calendar    Script Date: 4/6/2016 11:27:48 AM ******/
CREATE PROCEDURE [dbo].[Sync_Calendar]	
	@CalendarXML NVARCHAR(MAX)

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
- @CalendarXML: "<Calendar EmpId='{0}' WorkDate='{1}' ShiftId='{2}' IsHoliday='{3}' IsOffDay='{4}' IsLeaveDay='{5}' IsKeepInView='{6}'"
            + " LeaveTypeId='{7}' LeaveTypeId2='{8}' LeaveDay='{9}' LateIn='{10}' EarlyOut='{10}' RequestedCheckinTime='{11}'" 
            + " RequestedCheckoutTime='{11}' ChangeShiftId='{12}' ModifiedDate='{13}'></Calendar>"
*/
BEGIN
	DECLARE 
		@SyncDate	DATETIME,
		@Xml	XML		 

	SET @SyncDate = GETDATE()
	DECLARE @DocHandler INT
	SELECT @CalendarXML = ISNULL(@CalendarXML,'')
	
	IF OBJECT_ID('tempdb..#EmpCalendar') IS NOT NULL
	BEGIN
		DROP TABLE #EmpCalendar
	END 
	
	CREATE TABLE #EmpCalendar(
		[EmployeeId] [int] NOT NULL,
		[WorkDate] [datetime] NOT NULL,
		[ShiftId] [int] NULL,
		[IsHoliday] [bit] NULL,
		[IsOffDay] [bit] NULL,
		[IsLeaveDay] [bit] NULL,
		[IsKeepInView] [bit] NULL,
		[LeaveTypeId] [int] NULL,
		[LeaveTypeId2] [int] NULL,
		[LeaveDay] [decimal](18, 1) NULL,
		[LateIn] [int] NULL,
		[EarlyOut] [int] NULL,
		[RequestedCheckinTime] [datetime] NULL,
		[RequestedCheckoutTime] [datetime] NULL,
		[ChangeShiftId] [int] NULL,
		[ModifiedDate] [datetime] NULL,
	)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@CalendarXML

		INSERT INTO #EmpCalendar (
		  	[EmployeeId],
			[WorkDate],
			[ShiftId],
			[IsHoliday],
			[IsOffDay],
			[IsLeaveDay],
			[IsKeepInView],
			[LeaveTypeId],
			[LeaveTypeId2],
			[LeaveDay],
			[LateIn],
			[EarlyOut],
			[RequestedCheckinTime],
			[RequestedCheckoutTime],
			[ChangeShiftId],
			[ModifiedDate]
			)
		SELECT 	[EmpId],
				[WorkDate],
				CASE WHEN [ShiftId] = 0 THEN NULL ELSE [ShiftId] END,
				[IsHoliday],
				[IsOffDay],
				[IsLeaveDay],
				[IsKeepInView],
				CASE WHEN [LeaveTypeId] = -1 THEN NULL ELSE [LeaveTypeId] END,
			    CASE WHEN [LeaveTypeId2] = -1 THEN NULL ELSE [LeaveTypeId2] END,
				[LeaveDay],
				[LateIn],
				[EarlyOut],
				CASE WHEN ISDATE(RequestedCheckinTime ) = 1 THEN RequestedCheckinTime  ELSE NULL END [RequestedCheckinTime],
				CASE WHEN ISDATE(RequestedCheckoutTime ) = 1 THEN RequestedCheckoutTime  ELSE NULL END [RequestedCheckoutTime],
				CASE WHEN [ChangeShiftId] = 0 THEN NULL ELSE [ChangeShiftId] END,
				[ModifiedDate]
		FROM OPENXML(@DocHandler, '/ROOT/Calendar', 1) WITH (
					EmpId [int],
					WorkDate [datetime],
					ShiftId [int] ,
					IsHoliday [bit] ,
					IsOffDay [bit] ,
					IsLeaveDay [bit],
					IsKeepInView [bit],
					LeaveTypeId [int] ,
					LeaveTypeId2 [int] ,
					LeaveDay [decimal](18, 1),
					LateIn [int],
					EarlyOut [int],
					RequestedCheckinTime varchar(100),
					RequestedCheckoutTime varchar(100),
					ChangeShiftId [int],
					ModifiedDate [datetime]
				) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	INSERT INTO [dbo].HRMEmployeeCalendar(
		   [EmployeeId],
			[WorkDate],
			[ShiftId],
			[IsHoliday],
			[IsOffDay],
			[IsLeaveDay],
			[IsKeepInView],
			[LeaveTypeId],
			[LeaveTypeId2],
			[LeaveDay],
			[LateIn],
			[EarlyOut],
			[RequestedCheckinTime],
			[RequestedCheckoutTime],
			[ChangeShiftId],
			[ModifiedDate],
			[SyncDate])
	SELECT 	
			new.[EmployeeId],
			new.[WorkDate],
			new.[ShiftId],
			new.[IsHoliday],
			new.[IsOffDay],
			new.[IsLeaveDay],
			new.[IsKeepInView],
			new.[LeaveTypeId],
			new.[LeaveTypeId2],
			new.[LeaveDay],
			new.[LateIn],
			new.[EarlyOut],
			new.[RequestedCheckinTime],
			new.[RequestedCheckoutTime],
			new.[ChangeShiftId],
			new.[ModifiedDate],
			@SyncDate
	FROM #EmpCalendar new 
		LEFT JOIN [dbo].HRMEmployeeCalendar curr WITH (NOLOCK) ON
			new.[WorkDate] = curr.[WorkDate] AND new.[EmployeeId] = curr.[EmployeeId] 
	WHERE curr.[EmployeeId] IS NULL

	UPDATE curr  
	SET curr.[ShiftId] = new.[ShiftId],
		curr.[IsHoliday] = new.[IsHoliday],
		curr.[IsOffDay] = new.[IsOffDay],
		curr.[IsLeaveDay] = new.[IsLeaveDay],
		curr.[IsKeepInView] = new.[IsKeepInView],
		curr.[LeaveTypeId] = new.[LeaveTypeId],
		curr.[LeaveTypeId2] = new.[LeaveTypeId2],
		curr.[LeaveDay] = new.[LeaveDay],
		curr.[LateIn] = new.[LateIn],
		curr.[EarlyOut] = new.[EarlyOut],
		curr.[RequestedCheckinTime] = new.[RequestedCheckinTime],
		curr.[RequestedCheckoutTime] = new.[RequestedCheckoutTime],
		curr.[ChangeShiftId] = new.[ChangeShiftId],
		curr.[ModifiedDate] = new.[ModifiedDate],
		curr.[SyncDate] = @SyncDate
	FROM [dbo].HRMEmployeeCalendar curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #EmpCalendar new  ON
			new.[WorkDate] = curr.[WorkDate] AND new.[EmployeeId] = curr.[EmployeeId]

	SELECT @ErrCode = 0, @ErrMsg = ''
	
	IF OBJECT_ID('tempdb..#EmpCalendar') IS NOT NULL
	BEGIN
		DROP TABLE #EmpCalendar
	END
END