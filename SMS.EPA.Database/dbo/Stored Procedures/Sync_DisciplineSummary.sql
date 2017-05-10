CREATE PROCEDURE [dbo].[Sync_DisciplineSummary] @DisciplineXml NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20160916@Quinn.Vo
Task: Synchronize data discipline summary from hrm.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
- @DisciplineXml:  "<Discipline EmpId='{0}' Period='{1}' ChangeShift='{2}' MissingCheckInOut='{3}' LateIn='{4}' EarlyOut='{5}'"
            + " Absent='{6}' UnpaidLeave='{7}' UnplannedLeave='{8}' WeekendLeave='{9}' WorkingUndertime='{10}' UnpaidLeaveAbsent='{11}'"
            + " OtherDisciplines='{12}' SystemSubtractScore='{13}' ModifiedDate='{14}'></Discipline>";;
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME
		,@Xml XML

	SET @SyncDate = GETDATE()

	DECLARE @DocHandler INT

	IF OBJECT_ID('tempdb..#DisciplineSummary') IS NOT NULL
	BEGIN
		DROP TABLE #DisciplineSummary
	END

	CREATE TABLE #DisciplineSummary (
		[EmpId] INT
		,[Period] DATETIME
		,[ChangeShift] DECIMAL(5, 2)
		,[MissingCheckInOut] DECIMAL(5, 2)
		,[LateIn] DECIMAL(5, 2)
		,[EarlyOut] DECIMAL(5, 2)
		,[Absent] DECIMAL(5, 2)
		,[UnpaidLeave] DECIMAL(5, 2)
		,[UnplannedLeave] DECIMAL(5, 2)
		,[WeekendLeave] DECIMAL(5, 2)
		,[WorkingUndertime] DECIMAL(5, 2)
		,[UnpaidLeaveAbsent] DECIMAL(5, 2)
		,[OtherDisciplines] DECIMAL(5, 2)
		,[SystemSubtractScore] DECIMAL(5, 2)
		,[ModifiedDate] DATETIME
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@DisciplineXml

		INSERT INTO #DisciplineSummary (
			[EmpId]
			,[Period]
			,[ChangeShift]
			,[MissingCheckInOut]
			,[LateIn]
			,[EarlyOut]
			,[Absent]
			,[UnpaidLeave]
			,[UnplannedLeave]
			,[WeekendLeave]
			,[WorkingUndertime]
			,[UnpaidLeaveAbsent]
			,[OtherDisciplines]
			,[SystemSubtractScore]
			,[ModifiedDate]
			)
		SELECT [EmpId]
			,[Period]
			,[ChangeShift]
			,[MissingCheckInOut]
			,[LateIn]
			,[EarlyOut]
			,[Absent]
			,[UnpaidLeave]
			,[UnplannedLeave]
			,[WeekendLeave]
			,[WorkingUndertime]
			,[UnpaidLeaveAbsent]
			,[OtherDisciplines]
			,[SystemSubtractScore]
			,[ModifiedDate]
		FROM OPENXML(@DocHandler, '/ROOT/Discipline', 1) WITH ([EmpId] INT, [Period] DATETIME, [ChangeShift] DECIMAL(5, 2), [MissingCheckInOut] DECIMAL(5, 2), [LateIn] DECIMAL(5, 2), [EarlyOut] DECIMAL(5, 2), [Absent] DECIMAL(5, 2), [UnpaidLeave] DECIMAL(5, 2), [UnplannedLeave] DECIMAL(5, 2), [WeekendLeave] DECIMAL(5, 2), [WorkingUndertime] DECIMAL(5, 2), [UnpaidLeaveAbsent] DECIMAL(5, 2), [OtherDisciplines] DECIMAL(5, 2), [SystemSubtractScore] DECIMAL(5, 2), [ModifiedDate] DATETIME) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	INSERT INTO [dbo].[EmployeeDisciplineSummary] (
		[EmployeeId]
		,[Period]
		,[ChangeShift]
		,[MissingCheckInOut]
		,[LateIn]
		,[EarlyOut]
		,[Absent]
		,[UnpaidLeave]
		,[UnplannedLeave]
		,[WeekendLeave]
		,[WorkingUndertime]
		,[UnpaidLeaveAbsent]
		,[OtherDisciplines]
		,[SystemSubtractScore]
		,[ModifiedDate]
		,[SyncDate]
		)
	SELECT [EmpId]
		,new.[Period]
		,new.[ChangeShift]
		,new.[MissingCheckInOut]
		,new.[LateIn]
		,new.[EarlyOut]
		,new.[Absent]
		,new.[UnpaidLeave]
		,new.[UnplannedLeave]
		,new.[WeekendLeave]
		,new.[WorkingUndertime]
		,new.[UnpaidLeaveAbsent]
		,new.[OtherDisciplines]
		,new.[SystemSubtractScore]
		,new.[ModifiedDate]
		,@SyncDate
	FROM #DisciplineSummary new
		LEFT JOIN [dbo].[EmployeeDisciplineSummary] curr WITH (NOLOCK) ON new.[EmpId] = curr.[EmployeeId]
		AND new.[Period] = curr.[Period]
	WHERE curr.[EmployeeId] IS NULL

	UPDATE curr
	SET curr.[ChangeShift] = new.[ChangeShift]
		,curr.[MissingCheckInOut] = new.[MissingCheckInOut]
		,curr.[LateIn] = new.[LateIn]
		,curr.[EarlyOut] = new.[EarlyOut]
		,curr.[Absent] = new.[Absent]
		,curr.[UnpaidLeave] = new.[UnpaidLeave]
		,curr.[UnplannedLeave] = new.UnplannedLeave
		,curr.[WeekendLeave] = new.WeekendLeave
		,curr.[WorkingUndertime] = new.[WorkingUndertime]
		,curr.[UnpaidLeaveAbsent] = new.[UnpaidLeaveAbsent]
		,curr.[OtherDisciplines] = new.[OtherDisciplines]
		,curr.[SystemSubtractScore] = new.[SystemSubtractScore]
		,curr.[ModifiedDate] = new.[ModifiedDate]
		,curr.[SyncDate] = @SyncDate
	FROM [dbo].[EmployeeDisciplineSummary] curr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #DisciplineSummary new ON new.[EmpId] = curr.[EmployeeId]
		AND new.[Period] = curr.[Period]

	IF OBJECT_ID('tempdb..#DisciplineSummary') IS NOT NULL
	BEGIN
		DROP TABLE #DisciplineSummary
	END
END