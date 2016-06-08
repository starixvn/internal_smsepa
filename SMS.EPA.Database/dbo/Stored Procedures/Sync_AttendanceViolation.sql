/****** Object:  Stored Procedure dbo.Sync_AttendanceViolation    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.Sync_AttendanceViolation    Script Date: 5/16/2016 2:15:32 PM ******/
/****** Object:  Stored Procedure dbo.Sync_AttendanceViolation    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.Sync_AttendanceViolation    Script Date: 4/6/2016 11:27:48 AM ******/

CREATE PROCEDURE [dbo].[Sync_AttendanceViolation] @VialationXml NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20150914@Quinn.Vo
Task: Synchronize data attendances violation from hrm.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
- @AttendanceXml: "<Violation EmpId='{0}' WorkDate='{1}' MissingCheckInOut='{2}' LateIn='{3}' EarlyOut='{4}' ChangeShift='{5}' WorkingBelowTime='{6}'"
            + " Absent='{7}' UnpaidLeaveAbsent='{8}' UnpaidLeave='{9}' ShiftName='{10}' InFixed='{11}' OutFixed='{12}'"
            + " LateInValue='{13}' EarlyOutValue='{14}' TotalWorkHours='{15}' LateInApproverComment='{16}' LateInEmployeeComment='{17}'"
            + " EarlyOutApproverComment='{18}' EarlyOutEmployeeComment='{19}' ChangeShiftApproverComment='{20}' ChangeShiftEmployeeComment='{21}'"
            + " LeaveApproverComment='{22}' LeaveEmployeeComment='{22}' ModifiedDate='{23}' IsDeleted='{24}'></Violation>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME
		,@Xml XML

	SET @SyncDate = GETDATE()

	DECLARE @DocHandler INT

	IF OBJECT_ID('tempdb..#Vialation') IS NOT NULL
	BEGIN
		DROP TABLE #Vialation
	END

	CREATE TABLE #Vialation (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[MissingCheckInOut] INT
		,[LateIn] INT
		,[EarlyOut] INT
		,[ChangeShift] INT
		,[WorkingBelowTime] INT
		,[Absent] INT
		,[UnpaidLeaveAbsent] INT
		,[UnpaidLeave] FLOAT
		,[ShiftName] VARCHAR(50)
		,[InFixed] DATETIME
		,[OutFixed] DATETIME
		,[LateInValue] INT
		,[EarlyOutValue] INT
		,[TotalWorkHours] DECIMAL(18, 2)
		,[LateInApproverComment] NVARCHAR(500)
		,[LateInEmployeeComment] NVARCHAR(500)
		,[EarlyOutApproverComment] NVARCHAR(500)
		,[EarlyOutEmployeeComment] NVARCHAR(500)
		,[ChangeShiftApproverComment] NVARCHAR(500)
		,[ChangeShiftEmployeeComment] NVARCHAR(500)
		,[LeaveApproverComment] NVARCHAR(500)
		,[LeaveEmployeeComment] NVARCHAR(500)
		,[ModifiedDate] DATETIME
		,[IsDeleted] BIT
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@VialationXml

		INSERT INTO #Vialation (
			[WorkDate]
			,[EmpId]
			,[MissingCheckInOut]
			,[LateIn]
			,[EarlyOut]
			,[ChangeShift]
			,[WorkingBelowTime]
			,[Absent]
			,[UnpaidLeaveAbsent]
			,[UnpaidLeave]
			,[ShiftName]
			,[InFixed]
			,[OutFixed]
			,[LateInValue]
			,[EarlyOutValue]
			,[TotalWorkHours]
			,[LateInApproverComment]
			,[LateInEmployeeComment]
			,[EarlyOutApproverComment]
			,[EarlyOutEmployeeComment]
			,[ChangeShiftApproverComment]
			,[ChangeShiftEmployeeComment]
			,[LeaveApproverComment]
			,[LeaveEmployeeComment]
			,[ModifiedDate]
			,[IsDeleted]
			)
		SELECT [WorkDate]
			,[EmpId]
			,[MissingCheckInOut]
			,[LateIn]
			,[EarlyOut]
			,[ChangeShift]
			,[WorkingBelowTime]
			,[Absent]
			,[UnpaidLeaveAbsent]
			,[UnpaidLeave]
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([ShiftName], '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,CASE 
				WHEN DATEDIFF(D, [InFixed], '1900-01-01') = 0 THEN NULL
				ELSE [InFixed]
				END
			,CASE 
				WHEN DATEDIFF(D, [OutFixed], '1900-01-01') = 0 THEN NULL
				ELSE [OutFixed]
				END
			,[LateInValue]
			,[EarlyOutValue]
			,[TotalWorkHours]
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([LateInApproverComment], '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([LateInEmployeeComment], '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([EarlyOutApproverComment], '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([EarlyOutEmployeeComment], '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([ChangeShiftApproverComment], '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([ChangeShiftEmployeeComment], '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([LeaveApproverComment], '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE([LeaveEmployeeComment], '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
			,[ModifiedDate]
			,[IsDeleted]
		FROM OPENXML(@DocHandler, '/ROOT/Violation', 1) WITH ([WorkDate] DATETIME, [EmpId] INT, [MissingCheckInOut] INT, [LateIn] INT, [EarlyOut] INT, [ChangeShift] INT, [WorkingBelowTime] INT, [Absent] INT, [UnpaidLeaveAbsent] INT, [UnpaidLeave] FLOAT, [ShiftName] VARCHAR(50), [InFixed] DATETIME, [OutFixed] DATETIME, [LateInValue] INT, [EarlyOutValue] INT, [TotalWorkHours] DECIMAL(18, 2), [LateInApproverComment] NVARCHAR(500), [LateInEmployeeComment] NVARCHAR(500), [EarlyOutApproverComment] NVARCHAR(500), [EarlyOutEmployeeComment] NVARCHAR(500), [ChangeShiftApproverComment] NVARCHAR(500), [ChangeShiftEmployeeComment] NVARCHAR(500), [LeaveApproverComment] NVARCHAR(500), [LeaveEmployeeComment] NVARCHAR(500), [ModifiedDate] DATETIME, [IsDeleted] BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		-- ,@ErrMsg = @VialationXml
		RETURN
	END CATCH;

	INSERT INTO [dbo].[HRMEmployeeAttendanceViolation] (
		[WorkDate]
		,[EmpId]
		,[MissingCheckInOut]
		,[MissingCheckInOutAutoIgnore]
		,[LateIn]
		,[LateInAutoIgnore]
		,[EarlyOut]
		,[EarlyOutAutoIgnore]
		,[ChangeShift]
		,[ChangeShiftAutoIgnore]
		,[WorkingBelowTime]
		,[WorkingBelowTimeAutoIgnore]
		,[Absent]
		,[AbsentAutoIgnore]
		,[UnpaidLeaveAbsent]
		,[UnpaidLeaveAbsentAutoIgnore]
		,[UnpaidLeave]
		,[UnpaidLeaveAutoIgnore]
		,[ShiftName]
		,[InFixed]
		,[OutFixed]
		,[LateInValue]
		,[EarlyOutValue]
		,[TotalWorkHours]
		,[LateInApproverComment]
		,[LateInEmployeeComment]
		,[EarlyOutApproverComment]
		,[EarlyOutEmployeeComment]
		,[ChangeShiftApproverComment]
		,[ChangeShiftEmployeeComment]
		,[LeaveApproverComment]
		,[LeaveEmployeeComment]
		,[ModifiedDate]
		,[SyncDate]
		,[IsDeleted]
		)
	SELECT new.[WorkDate]
		,new.[EmpId]
		,new.[MissingCheckInOut]
		,CASE 
			WHEN new.[MissingCheckInOut] > 0
				AND (
					(
						new.[ChangeShift] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 1 -- Change Shift
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[WorkingBelowTime] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 7 -- Working Under Time
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[EarlyOut] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 4 -- Early Out
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[LateIn] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 3 -- Late In
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeave] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 6 -- Unpaid Leave
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[Absent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 5 -- Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeaveAbsent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 8 -- Unpaid Leave Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					) THEN 1
			ELSE 0
			END
		,new.[LateIn]
		,CASE 
			WHEN new.[LateIn] > 0
				AND (
					(
						new.[UnpaidLeave] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 6 -- Unpaid Leave
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[Absent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 5 -- Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeaveAbsent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 8 -- Unpaid Leave Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					) THEN 1
			ELSE 0
			END
		,new.[EarlyOut]
		,CASE 
			WHEN new.[EarlyOut] > 0
				AND (
					(
						new.[LateIn] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 3 -- Late In
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeave] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 6 -- Unpaid Leave
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[Absent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 5 -- Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeaveAbsent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 8 -- Unpaid Leave Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					) THEN 1
			ELSE 0
			END
		,new.[ChangeShift]
		,CASE 
			WHEN new.[ChangeShift] > 0
				AND (
					(
						new.[WorkingBelowTime] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 7 -- Working Under Time
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[EarlyOut] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 4 -- Early Out
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[LateIn] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 3 -- Late In
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeave] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 6 -- Unpaid Leave
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[Absent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 5 -- Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeaveAbsent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 8 -- Unpaid Leave Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					) THEN 1
			ELSE 0
			END
		,new.[WorkingBelowTime]
		,CASE 
			WHEN new.[WorkingBelowTime] > 0
				AND (
					(
						new.[EarlyOut] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 4 -- Early Out
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[LateIn] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 3 -- Late In
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeave] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 6 -- Unpaid Leave
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[Absent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 5 -- Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeaveAbsent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 8 -- Unpaid Leave Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					) THEN 1
			ELSE 0
			END
		,new.[Absent]
		,CASE 
			WHEN new.[Absent] > 0
				AND (
					new.[UnpaidLeaveAbsent] > 0
					AND EXISTS (
						SELECT TOP 1 1
						FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
						WHERE [ViolationId] = 8 -- Unpaid Leave Absent
							AND [Value] = 1
							AND EXISTS (
								SELECT TOP 1 1
								FROM [dbo].[Employee] WITH (NOLOCK)
								WHERE [GroupId] = attS.[GroupId]
									AND [EmployeeId] = new.[EmpId]
									AND [IsLocal] = attS.[IsLocal]
								)
						)
					) THEN 1
			ELSE 0
			END
		,new.[UnpaidLeaveAbsent]
		,0
		,new.[UnpaidLeave]
		,CASE 
			WHEN new.[UnpaidLeave] > 0
				AND (
					(
						new.[Absent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 5 -- Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeaveAbsent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 8 -- Unpaid Leave Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					) THEN 1
			ELSE 0
			END
		,new.[ShiftName]
		,new.[InFixed]
		,new.[OutFixed]
		,new.[LateInValue]
		,new.[EarlyOutValue]
		,new.[TotalWorkHours]
		,new.[LateInApproverComment]
		,new.[LateInEmployeeComment]
		,new.[EarlyOutApproverComment]
		,new.[EarlyOutEmployeeComment]
		,new.[ChangeShiftApproverComment]
		,new.[ChangeShiftEmployeeComment]
		,new.[LeaveApproverComment]
		,new.[LeaveEmployeeComment]
		,new.[ModifiedDate]
		,@SyncDate
		,new.[IsDeleted]
	FROM #Vialation new
		LEFT JOIN [dbo].[HRMEmployeeAttendanceViolation] curr WITH (NOLOCK) ON new.[EmpId] = curr.[EmpId]
		AND new.[WorkDate] = curr.[WorkDate]
	WHERE curr.[EmpId] IS NULL

	UPDATE curr
	SET curr.[EmpId] = new.[EmpId]
		,curr.[WorkDate] = new.[WorkDate]
		,curr.[MissingCheckInOut] = new.[MissingCheckInOut]
		,curr.[MissingCheckInOutAutoIgnore] = CASE 
			WHEN new.[MissingCheckInOut] > 0
				AND (
					(
						new.[ChangeShift] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 1 -- Change Shift
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[WorkingBelowTime] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 7 -- Working Under Time
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[EarlyOut] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 4 -- Early Out
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[LateIn] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 3 -- Late In
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeave] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 6 -- Unpaid Leave
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[Absent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 5 -- Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeaveAbsent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 8 -- Unpaid Leave Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					) THEN 1
			ELSE 0
			END
		,curr.[LateIn] = new.[LateIn]
		,curr.[LateInAutoIgnore] = CASE 
			WHEN new.[LateIn] > 0
				AND (
					(
						new.[UnpaidLeave] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 6 -- Unpaid Leave
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[Absent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 5 -- Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeaveAbsent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 8 -- Unpaid Leave Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					) THEN 1
			ELSE 0
			END
		,curr.[EarlyOut] = new.[EarlyOut]
		,curr.[EarlyOutAutoIgnore] = CASE 
			WHEN new.[EarlyOut] > 0
				AND (
					(
						new.[LateIn] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 3 -- Late In
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeave] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 6 -- Unpaid Leave
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[Absent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 5 -- Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeaveAbsent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 8 -- Unpaid Leave Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					) THEN 1
			ELSE 0
			END
		,curr.[ChangeShift] = new.[ChangeShift]
		,curr.[ChangeShiftAutoIgnore] = CASE 
			WHEN new.[ChangeShift] > 0
				AND (
					(
						new.[WorkingBelowTime] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 7 -- Working Under Time
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[EarlyOut] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 4 -- Early Out
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[LateIn] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 3 -- Late In
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeave] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 6 -- Unpaid Leave
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[Absent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 5 -- Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeaveAbsent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 8 -- Unpaid Leave Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					) THEN 1
			ELSE 0
			END
		,curr.[WorkingBelowTime] = new.[WorkingBelowTime]
		,curr.[WorkingBelowTimeAutoIgnore] = CASE 
			WHEN new.[WorkingBelowTime] > 0
				AND (
					(
						new.[EarlyOut] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 4 -- Early Out
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[LateIn] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 3 -- Late In
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeave] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 6 -- Unpaid Leave
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[Absent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 5 -- Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeaveAbsent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 8 -- Unpaid Leave Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					) THEN 1
			ELSE 0
			END
		,curr.[Absent] = new.[Absent]
		,curr.[AbsentAutoIgnore] = CASE 
			WHEN new.[Absent] > 0
				AND (
					new.[UnpaidLeaveAbsent] > 0
					AND EXISTS (
						SELECT TOP 1 1
						FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
						WHERE [ViolationId] = 8 -- Unpaid Leave Absent
							AND [Value] = 1
							AND EXISTS (
								SELECT TOP 1 1
								FROM [dbo].[Employee] WITH (NOLOCK)
								WHERE [GroupId] = attS.[GroupId]
									AND [EmployeeId] = new.[EmpId]
									AND [IsLocal] = attS.[IsLocal]
								)
						)
					) THEN 1
			ELSE 0
			END
		,curr.[UnpaidLeaveAbsent] = new.[UnpaidLeaveAbsent]
		,curr.[UnpaidLeaveAbsentAutoIgnore] = 0
		,curr.[UnpaidLeave] = new.[UnpaidLeave]
		,curr.[UnpaidLeaveAutoIgnore] = CASE 
			WHEN new.[UnpaidLeave] > 0
				AND (
					(
						new.[Absent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 5 -- Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					OR (
						new.[UnpaidLeaveAbsent] > 0
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
							WHERE [ViolationId] = 8 -- Unpaid Leave Absent
								AND [Value] = 1
								AND EXISTS (
									SELECT TOP 1 1
									FROM [dbo].[Employee] WITH (NOLOCK)
									WHERE [GroupId] = attS.[GroupId]
										AND [EmployeeId] = new.[EmpId]
										AND [IsLocal] = attS.[IsLocal]
									)
							)
						)
					) THEN 1
			ELSE 0
			END
		,curr.[ShiftName] = new.[ShiftName]
		,curr.[InFixed] = new.[InFixed]
		,curr.[OutFixed] = new.[OutFixed]
		,curr.[LateInValue] = new.[LateInValue]
		,curr.[EarlyOutValue] = new.[EarlyOutValue]
		,curr.[TotalWorkHours] = new.[TotalWorkHours]
		,curr.[LateInApproverComment] = new.[LateInApproverComment]
		,curr.[LateInEmployeeComment] = new.[LateInEmployeeComment]
		,curr.[EarlyOutApproverComment] = new.[EarlyOutApproverComment]
		,curr.[EarlyOutEmployeeComment] = new.[EarlyOutEmployeeComment]
		,curr.[ChangeShiftApproverComment] = new.[ChangeShiftApproverComment]
		,curr.[ChangeShiftEmployeeComment] = new.[ChangeShiftEmployeeComment]
		,curr.[LeaveApproverComment] = new.[LeaveApproverComment]
		,curr.[LeaveEmployeeComment] = new.[LeaveEmployeeComment]
		,curr.[ModifiedDate] = new.[ModifiedDate]
		,curr.SyncDate = @SyncDate
		,curr.IsDeleted = new.IsDeleted
	FROM [dbo].[HRMEmployeeAttendanceViolation] curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #Vialation new ON new.[EmpId] = curr.[EmpId]
		AND new.[WorkDate] = curr.[WorkDate]

	DELETE
	FROM [dbo].[AttendanceDetailIgnored]
	WHERE EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[HRMEmployeeAttendanceViolation] AS empA WITH (NOLOCK)
				INNER JOIN #Vialation new ON new.[EmpId] = empA.[EmpId]
				AND new.[WorkDate] = empA.[WorkDate]
			WHERE [dbo].[AttendanceDetailIgnored].[EmployeeId] = empA.[EmpId]
				AND [dbo].[AttendanceDetailIgnored].[WorkDate] = empA.[WorkDate]
				AND (
					(
						[dbo].[AttendanceDetailIgnored].[ViolationId] = 1
						AND empA.[ChangeShift] = 0
						)
					OR (
						[dbo].[AttendanceDetailIgnored].[ViolationId] = 2
						AND empA.[MissingCheckInOut] = 0
						)
					OR (
						[dbo].[AttendanceDetailIgnored].[ViolationId] = 3
						AND empA.[LateIn] = 0
						)
					OR (
						[dbo].[AttendanceDetailIgnored].[ViolationId] = 4
						AND empA.[EarlyOut] = 0
						)
					OR (
						[dbo].[AttendanceDetailIgnored].[ViolationId] = 5
						AND empA.[Absent] = 0
						)
					OR (
						[dbo].[AttendanceDetailIgnored].[ViolationId] = 6
						AND empA.[UnpaidLeave] = 0
						)
					OR (
						[dbo].[AttendanceDetailIgnored].[ViolationId] = 7
						AND empA.[WorkingBelowTime] = 0
						)
					OR (
						[dbo].[AttendanceDetailIgnored].[ViolationId] = 8
						AND empA.[UnpaidLeaveAbsent] = 0
						)
					)
			)
	
	DELETE FROM [dbo].[HRMEmployeeAttendanceViolation] WHERE IsDeleted = 1

	SELECT @ErrCode = 0
		,@ErrMsg = ''

	IF OBJECT_ID('tempdb..#Vialation') IS NOT NULL
	BEGIN
		DROP TABLE #Vialation
	END
END