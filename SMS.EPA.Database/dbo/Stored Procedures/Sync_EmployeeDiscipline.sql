
CREATE PROCEDURE [dbo].[Sync_EmployeeDiscipline] @VialationXml NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20150914@Quinn.Vo
Task: Synchronize data attendances violation from hrm.
DB: SMSEPA
Original: 

Revisions:  
20160524@Razor.Nguyen: Change Discipline
Param's Explanation:
- @AttendanceXml:  "<Violation EmpId='{0}' WorkDate='{1}' ShiftName='{2}' CheckIn='{3}' CheckOut='{4}' TotalHours='{5}'"
            + " LateIn='{6}' EarlyOut='{7}' Type='{8}' DisciplineScore='{9}'"
            + " EmployeeComment='{10}' ApproverComment='{11}' IsMissingCheckInOut='{12}' IsIgnoredMissingCheckInOut='{13}'"
            + " IsLateIn='{14}' IsIgnoredLateIn='{15}' IsEarlyOut='{16}' IsIgnoredEarlyOut='{17}' IsWorkingUndertime='{18}'"
            + " IsIgnoredWorkingUndertime='{19}' IsChangeShift='{20}' IsIgnoredChangeShift='{21}' IsAbsent='{22}' IsIgnoredAbsent='{23}'"
            + " IsUnplannedLeave='{24}' IsIgnoredUnplannedLeave='{25}' IsWeekendLeave='{26}' IsIgnoredWeekendLeave='{27}'"
            + " IsUnpaidLeave='{28}' IsIgnoredUnpaidLeave='{29}' IsUnpaidLeaveAbsent='{30}' IsIgnoredUnpaidLeaveAbsent='{31}'"
            + " ModifiedDate='{32}' IsDeleted='{33}'></Violation>";
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME
		,@Xml XML

	SET @SyncDate = GETDATE()

	DECLARE @DocHandler INT

	IF OBJECT_ID('tempdb..#Violation') IS NOT NULL
	BEGIN
		DROP TABLE #Violation
	END

	CREATE TABLE #Violation (
		[WorkDate] DATE
		,[EmpId] INT
		,[ShiftName] VARCHAR(50)
		,[CheckIn] DATETIME NULL
		,[CheckOut] DATETIME NULL
		,[TotalHours] DECIMAL(5, 2)
		,[LateIn] INT
		,[EarlyOut] INT
		,[Type] INT
		,[DisciplineScore] DECIMAL(5, 2)
		,[EmployeeComment] NVARCHAR(500)
		,[ApproverComment] NVARCHAR(500)
		,[IsMissingCheckInOut] BIT
		,[IsIgnoredMissingCheckInOut] BIT
		,[IsLateIn] BIT
		,[IsIgnoredLateIn] BIT
		,[IsEarlyOut] BIT
		,[IsIgnoredEarlyOut] BIT
		,[IsWorkingUndertime] BIT
		,[IsIgnoredWorkingUndertime] BIT
		,[IsChangeShift] BIT
		,[IsIgnoredChangeShift] BIT
		,[IsAbsent] BIT
		,[IsIgnoredAbsent] BIT
		,[IsUnplannedLeave] BIT
		,[IsIgnoredUnplannedLeave] BIT
		,[IsWeekendLeave] BIT
		,[IsIgnoredWeekendLeave] BIT
		,[IsUnpaidLeave] BIT
		,[IsIgnoredUnpaidLeave] BIT
		,[IsUnpaidLeaveAbsent] BIT
		,[IsIgnoredUnpaidLeaveAbsent] BIT
		,[ModifiedDate] DATETIME
		,[IsDeleted] BIT
		,[SyncDate] DATETIME
		)

	BEGIN TRY
		PRINT @VialationXml

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@VialationXml

		INSERT INTO #Violation (
			[WorkDate]
			,[EmpId]
			,[ShiftName]
			,[CheckIn]
			,[CheckOut]
			,[TotalHours]
			,[LateIn]
			,[EarlyOut]
			,[Type]
			,[DisciplineScore]
			,[EmployeeComment]
			,[ApproverComment]
			,[IsMissingCheckInOut]
			,[IsIgnoredMissingCheckInOut]
			,[IsLateIn]
			,[IsIgnoredLateIn]
			,[IsEarlyOut]
			,[IsIgnoredEarlyOut]
			,[IsWorkingUndertime]
			,[IsIgnoredWorkingUndertime]
			,[IsChangeShift]
			,[IsIgnoredChangeShift]
			,[IsAbsent]
			,[IsIgnoredAbsent]
			,[IsUnplannedLeave]
			,[IsIgnoredUnplannedLeave]
			,[IsWeekendLeave]
			,[IsIgnoredWeekendLeave]
			,[IsUnpaidLeave]
			,[IsIgnoredUnpaidLeave]
			,[IsUnpaidLeaveAbsent]
			,[IsIgnoredUnpaidLeaveAbsent]
			,[ModifiedDate]
			,[IsDeleted]
			,[SyncDate]
			)
		SELECT [WorkDate]
			,[EmpId]
			,[ShiftName]
			,CASE 
				WHEN CheckIn = Convert(DATETIME, '1900-01-01 00:00:00.000') THEN NULL
				ELSE CheckIn
				END
			,CASE 
				WHEN CheckOut = Convert(DATETIME, '1900-01-01 00:00:00.000') THEN NULL
				ELSE CheckOut
				END
			,[TotalHours]
			,[LateIn]
			,[EarlyOut]
			,[Type]
			,[DisciplineScore]
			,[EmployeeComment]
			,[ApproverComment]
			,[IsMissingCheckInOut]
			,[IsIgnoredMissingCheckInOut]
			,[IsLateIn]
			,[IsIgnoredLateIn]
			,[IsEarlyOut]
			,[IsIgnoredEarlyOut]
			,[IsWorkingUndertime]
			,[IsIgnoredWorkingUndertime]
			,[IsChangeShift]
			,[IsIgnoredChangeShift]
			,[IsAbsent]
			,[IsIgnoredAbsent]
			,[IsUnplannedLeave]
			,[IsIgnoredUnplannedLeave]
			,[IsWeekendLeave]
			,[IsIgnoredWeekendLeave]
			,[IsUnpaidLeave]
			,[IsIgnoredUnpaidLeave]
			,[IsUnpaidLeaveAbsent]
			,[IsIgnoredUnpaidLeaveAbsent]
			,[ModifiedDate]
			,[IsDeleted]
			,@SyncDate
		FROM OPENXML(@DocHandler, '/ROOT/Violation', 1) WITH ([WorkDate] DATETIME, [EmpId] INT, [ShiftName] VARCHAR(50), [CheckIn] DATETIME, [CheckOut] DATETIME, [TotalHours] DECIMAL(5, 2), [LateIn] INT, [EarlyOut] INT, [Type] INT, [DisciplineScore] DECIMAL(5, 2), [EmployeeComment] NVARCHAR(500), [ApproverComment] NVARCHAR(500), [IsMissingCheckInOut] BIT, [IsIgnoredMissingCheckInOut] BIT, [IsLateIn] BIT, [IsIgnoredLateIn] BIT, [IsEarlyOut] BIT, [IsIgnoredEarlyOut] BIT, [IsWorkingUndertime] BIT, [IsIgnoredWorkingUndertime] BIT, [IsChangeShift] BIT, [IsIgnoredChangeShift] BIT, [IsAbsent] BIT, [IsIgnoredAbsent] BIT, 
			[IsUnplannedLeave] BIT
			,[IsIgnoredUnplannedLeave] BIT
			,[IsWeekendLeave] BIT
			,[IsIgnoredWeekendLeave] BIT
		,[IsUnpaidLeave] BIT, [IsIgnoredUnpaidLeave] BIT, [IsUnpaidLeaveAbsent] BIT, [IsIgnoredUnpaidLeaveAbsent] BIT, [ModifiedDate] DATETIME, [IsDeleted] BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler

	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	INSERT INTO [dbo].[EmployeeDiscipline] (
		[WorkDate]
		,[EmpId]
		,[ShiftName]
		,[CheckIn]
		,[CheckOut]
		,[TotalHours]
		,[LateIn]
		,[EarlyOut]
		,[DisciplineTypeId]
		,[DisciplineScore]
		,[EmployeeComment]
		,[ApproverComment]
		,[IsMissingCheckInOut]
		,[IsIgnoredMissingCheckInOut]
		,[IsLateIn]
		,[IsIgnoredLateIn]
		,[IsEarlyOut]
		,[IsIgnoredEarlyOut]
		,[IsWorkingUndertime]
		,[IsIgnoredWorkingUndertime]
		,[IsChangeShift]
		,[IsIgnoredChangeShift]
		,[IsAbsent]
		,[IsIgnoredAbsent]
		,[IsUnplannedLeave]
		,[IsIgnoredUnplannedLeave]
		,[IsWeekendLeave]
		,[IsIgnoredWeekendLeave]
		,[IsUnpaidLeave]
		,[IsIgnoredUnpaidLeave]
		,[IsUnpaidLeaveAbsent]
		,[IsIgnoredUnpaidLeaveAbsent]
		,[ModifiedDate]
		,[IsDeleted]
		,[SyncDate]
		)
	SELECT new.[WorkDate]
		,new.[EmpId]
		,new.[ShiftName]
		,new.[CheckIn]
		,new.[CheckOut]
		,new.[TotalHours]
		,new.[LateIn]
		,new.[EarlyOut]
		,new.[Type]
		,new.[DisciplineScore]
		,new.[EmployeeComment]
		,new.[ApproverComment]
		,new.[IsMissingCheckInOut]
		,new.[IsIgnoredMissingCheckInOut]
		,new.[IsLateIn]
		,new.[IsIgnoredLateIn]
		,new.[IsEarlyOut]
		,new.[IsIgnoredEarlyOut]
		,new.[IsWorkingUndertime]
		,new.[IsIgnoredWorkingUndertime]
		,new.[IsChangeShift]
		,new.[IsIgnoredChangeShift]
		,new.[IsAbsent]
		,new.[IsIgnoredAbsent]
		,new.[IsUnplannedLeave]
		,new.[IsIgnoredUnplannedLeave]
		,new.[IsWeekendLeave]
		,new.[IsIgnoredWeekendLeave]
		,new.[IsUnpaidLeave]
		,new.[IsIgnoredUnpaidLeave]
		,new.[IsUnpaidLeaveAbsent]
		,new.[IsIgnoredUnpaidLeaveAbsent]
		,new.[ModifiedDate]
		,new.[IsDeleted]
		,@SyncDate
	FROM #Violation new
		LEFT JOIN [dbo].[EmployeeDiscipline] curr WITH (NOLOCK) ON new.[EmpId] = curr.[EmpId]
		AND new.[WorkDate] = curr.[WorkDate]
	WHERE curr.[EmpId] IS NULL
	AND new.IsDeleted = 0

	DELETE curr 
	FROM #Violation new
		INNER JOIN [dbo].[EmployeeDiscipline] curr WITH (NOLOCK) ON new.[EmpId] = curr.[EmpId]
		AND new.[WorkDate] = curr.[WorkDate]
	WHERE curr.[EmpId] IS NULL
	AND new.IsDeleted = 1

	UPDATE curr
	SET curr.[WorkDate] = new.[WorkDate]
		,curr.[EmpId] = new.[EmpId]
		,curr.[ShiftName] = new.[ShiftName]
		,curr.[CheckIn] = new.[CheckIn]
		,curr.[CheckOut] = new.[CheckOut]
		,curr.[TotalHours] = new.[TotalHours]
		,curr.[LateIn] = new.[LateIn]
		,curr.[EarlyOut] = new.[EarlyOut]
		,curr.[DisciplineTypeId] = new.[Type]
		,curr.[DisciplineScore] = new.[DisciplineScore]
		,curr.[EmployeeComment] = new.[EmployeeComment]
		,curr.[ApproverComment] = new.[ApproverComment]
		,curr.[IsMissingCheckInOut] = new.[IsMissingCheckInOut]
		,curr.[IsIgnoredMissingCheckInOut] = new.[IsIgnoredMissingCheckInOut]
		,curr.[IsLateIn] = new.[IsLateIn]
		,curr.[IsIgnoredLateIn] = new.[IsIgnoredLateIn]
		,curr.[IsEarlyOut] = new.[IsEarlyOut]
		,curr.[IsIgnoredEarlyOut] = new.[IsIgnoredEarlyOut]
		,curr.[IsWorkingUndertime] = new.[IsWorkingUndertime]
		,curr.[IsIgnoredWorkingUndertime] = new.[IsIgnoredWorkingUndertime]
		,curr.[IsChangeShift] = new.[IsChangeShift]
		,curr.[IsIgnoredChangeShift] = new.[IsIgnoredChangeShift]
		,curr.[IsAbsent] = new.[IsAbsent]
		,curr.[IsIgnoredAbsent] = new.[IsIgnoredAbsent]
		,curr.[IsUnplannedLeave] = new.[IsUnplannedLeave]
		,curr.[IsIgnoredUnplannedLeave] = new.[IsIgnoredUnplannedLeave]
		,curr.[IsWeekendLeave] = new.[IsWeekendLeave]
		,curr.[IsIgnoredWeekendLeave] = new.[IsIgnoredWeekendLeave]
		,curr.[IsUnpaidLeave] = new.[IsUnpaidLeave]
		,curr.[IsIgnoredUnpaidLeave] = new.[IsIgnoredUnpaidLeave]
		,curr.[IsUnpaidLeaveAbsent] = new.[IsUnpaidLeaveAbsent]
		,curr.[IsIgnoredUnpaidLeaveAbsent] = new.[IsIgnoredUnpaidLeaveAbsent]
		,curr.[ModifiedDate] = new.[ModifiedDate]
		,curr.[IsDeleted] = new.[IsDeleted]
		,curr.SyncDate = @SyncDate
	FROM [dbo].[EmployeeDiscipline] curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #Violation new ON new.[EmpId] = curr.[EmpId]
		AND new.[WorkDate] = curr.[WorkDate]

	IF OBJECT_ID('tempdb..#Violation') IS NOT NULL
	BEGIN
		DROP TABLE #Violation
	END
END