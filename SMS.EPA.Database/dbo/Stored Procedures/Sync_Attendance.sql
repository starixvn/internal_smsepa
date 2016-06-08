/****** Object:  Stored Procedure dbo.Sync_Attendance    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Attendance    Script Date: 5/16/2016 2:15:32 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Attendance    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Attendance    Script Date: 4/6/2016 11:27:48 AM ******/
CREATE PROCEDURE [dbo].[Sync_Attendance]
	@AttendanceXml NVARCHAR(MAX)

	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 20150828@Quinn.Vo
Task: Synchronize data attendances from hrm.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
- @AttendanceXml: "<Attendance EmployeeId='{0}' WorkDate='{1}' ShiftId='{2}' InFixed='{3}' OutFixed='{4}' LateIn='{5}' EarlyOut='{6}'"
            + " LeaveTypeId='{7}' LeaveTypeId2='{8}' Absence='{9}' TotalWorkHours='{10}' LeaveDay='{10}' ModifiedDate='{11}'></Attendance>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE 
		@SyncDate	DATETIME,
		@Xml	XML		 

	SET @SyncDate = GETDATE()
	DECLARE @DocHandler INT
	
	IF OBJECT_ID('tempdb..#Attendance') IS NOT NULL
	BEGIN
		DROP TABLE #Attendance
	END 
	
	CREATE TABLE #Attendance(
		 [WorkDate] DATETIME
		  ,[EmployeeId] INT
		  ,[ShiftId] INT
		  ,[InFixed] DATETIME
		  ,[OutFixed] DATETIME
		  ,[LateIn] INT
		  ,[EarlyOut] INT
		  ,[LeaveTypeId] INT
		  ,[LeaveTypeId2] INT
		  ,[Absence] BIT
		  ,[TotalWorkHours] DECIMAL(18,2)
		  ,[LeaveDay] DECIMAL(18,1)
		  ,[ModifiedDate] DATETIME)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@AttendanceXml

		INSERT INTO #Attendance (
			  [WorkDate]
			  ,[EmployeeId]
			  ,[ShiftId]
			  ,[InFixed]
			  ,[OutFixed]
			  ,[LateIn]
			  ,[EarlyOut]
			  ,[LeaveTypeId]
			  ,[LeaveTypeId2]
			  ,[Absence]
			  ,[TotalWorkHours]
			  ,[LeaveDay]
			  ,[ModifiedDate])
		SELECT [WorkDate]
			  ,[EmployeeId]
			  ,CASE WHEN [ShiftId] = 0 THEN NULL ELSE [ShiftId] END
			  ,CASE WHEN DATEDIFF(D, [InFixed], '1900-01-01') = 0 THEN NULL ELSE [InFixed] END
			  ,CASE WHEN DATEDIFF(D, [OutFixed], '1900-01-01') = 0 THEN NULL ELSE [OutFixed] END
			  ,[LateIn]
			  ,[EarlyOut]
			  ,CASE WHEN [LeaveTypeId] = -1 THEN NULL ELSE [LeaveTypeId] END
			  ,CASE WHEN [LeaveTypeId2] = -1 THEN NULL ELSE [LeaveTypeId2] END
			  ,[Absence]
			  ,[TotalWorkHours]
			  ,[LeaveDay]
			  ,[ModifiedDate]
		FROM OPENXML(@DocHandler, '/ROOT/Attendance', 1) WITH (
				[WorkDate] DATETIME
				,[EmployeeId] INT
				,[ShiftId] INT
				,[InFixed] DATETIME
				,[OutFixed] DATETIME
				,[LateIn] INT
				,[EarlyOut] INT
				,[LeaveTypeId] INT
				,[LeaveTypeId2] INT
				,[Absence] BIT
				,[TotalWorkHours] DECIMAL(18,2)
				,[LeaveDay] DECIMAL(18,1)
				,[ModifiedDate] DATETIME
				) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;
	 
	INSERT INTO [dbo].[HRMEmployeeAttendance]
           (  [WorkDate]
			  ,[EmployeeId]
			  ,[ShiftId]
			  ,[InFixed]
			  ,[OutFixed]
			  ,[LateIn]
			  ,[EarlyOut]
			  ,[LeaveTypeId]
			  ,[LeaveTypeId2]
			  ,[Absence]
			  ,[TotalWorkHours]
			  ,[LeaveDay]
			  ,[ModifiedDate]
			  ,SyncDate)
	SELECT new.[WorkDate]
			,new.[EmployeeId]
			,new.[ShiftId]
			,new.[InFixed]
			,new.[OutFixed]
			,new.[LateIn]
			,new.[EarlyOut]
			,new.[LeaveTypeId]
			,new.[LeaveTypeId2]
			,new.[Absence]
			,new.[TotalWorkHours]
			,new.[LeaveDay]
			,new.[ModifiedDate]
			,@SyncDate
	FROM #Attendance new
		LEFT JOIN  [dbo].[HRMEmployeeAttendance] curr WITH (NOLOCK) 
			ON new.[EmployeeId] = curr.[EmployeeId] AND new.[WorkDate] = curr.[WorkDate]
	WHERE curr.[EmployeeId] IS NULL 
	
	UPDATE curr  
	SET curr.[EmployeeId] = new.[EmployeeId],
		curr.[WorkDate] = new.[WorkDate],
		curr.[ShiftId] = new.[ShiftId],
		curr.[InFixed] = new.[InFixed],
		curr.[OutFixed] = new.[OutFixed],
		curr.[LateIn] = new.[LateIn],
		curr.[EarlyOut] = new.[EarlyOut],
		curr.[LeaveTypeId] = new.[LeaveTypeId],
		curr.[LeaveTypeId2] = new.[LeaveTypeId2],
		curr.[Absence] = new.[Absence],
		curr.[TotalWorkHours] = new.[TotalWorkHours],
		curr.[LeaveDay] = new.[LeaveDay],
		curr.[ModifiedDate] = new.[ModifiedDate],
		curr.SyncDate = @SyncDate
	FROM [dbo].[HRMEmployeeAttendance] curr WITH (ROWLOCK, UPDLOCK) 
		INNER JOIN #Attendance new 
		ON new.[EmployeeId] = curr.[EmployeeId] AND new.[WorkDate] = curr.[WorkDate]

	SELECT @ErrCode = 0, @ErrMsg = ''
	
	IF OBJECT_ID('tempdb..#Attendance') IS NOT NULL
	BEGIN
		DROP TABLE #Attendance
	END 
END