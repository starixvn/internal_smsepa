/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_IgnoredViolation    Script Date: 5/16/2016 2:20:11 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_IgnoredViolation    Script Date: 5/16/2016 2:15:33 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_IgnoredViolation    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_IgnoredViolation    Script Date: 4/6/2016 11:27:43 AM ******/

CREATE PROCEDURE [dbo].[EpaAttendanceSummary_IgnoredViolation] @IgnoredViolations NVARCHAR(MAX) = NULL
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Ignored violations.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
	@IgnoredViolations = '<ROOT>
							<Violation WorkDate='' EmployeeId='' ViolationId='' Ignored=''></Violation>
						</ROOT>'
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @DocHandler INT

	CREATE TABLE #Violations (
		[WorkDate] DATETIME
		,[EmployeeId] INT
		,[ViolationId] INT
		,[Ignored] BIT
		)

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@IgnoredViolations

	IF EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[EmployeeRole] AS empR WITH (NOLOCK)
				INNER JOIN [dbo].[RoleFunction] AS rf WITH (NOLOCK) ON empR.[RoleId] = rf.[RoleId]
			WHERE empR.[EmployeeId] = @ModifiedBy
				AND rf.[FunctionId] = 26 -- Discipline 
				AND rf.[IsHost] = 1
			)
	BEGIN
		INSERT INTO #Violations (
			[WorkDate]
			,[EmployeeId]
			,[ViolationId]
			,[Ignored]
			)
		SELECT [WorkDate]
			,[EmployeeId]
			,[ViolationId]
			,[Ignored]
		FROM OPENXML(@DocHandler, '/ROOT/Violation', 1) WITH (WorkDate DATETIME, EmployeeId INT, ViolationId INT, Ignored BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler

		DELETE
		FROM #Violations
		WHERE EXISTS (
				SELECT TOP 1 1
				FROM [dbo].[Period] AS pe WITH (NOLOCK)
					INNER JOIN [dbo].[PeriodTargetSetting] AS peT WITH (NOLOCK) ON pe.[PeriodId] = peT.[PeriodId]
					INNER JOIN [dbo].[EPA] AS epa WITH (NOLOCK) ON peT.[PeriodId] = epa.[PeriodId]
					AND peT.[EmployeeId] = epa.[EmployeeId]
				WHERE peT.[EmployeeId] = #Violations.[EmployeeId]
					AND CAST(pe.[EndDate] AS DATE) < CAST(GETDATE() AS DATE)
					AND CAST(#Violations.[WorkDate] AS DATE) <= CAST(pe.[AttendanceEndDate] AS DATE)
					AND CAST(#Violations.[WorkDate] AS DATE) >= CAST(pe.[AttendanceStartDate] AS DATE)
					AND NOT EXISTS (
						SELECT TOP 1 1
						FROM [dbo].[PeriodExcludedEmployee] WITH (NOLOCK)
						WHERE [PeriodId] = pe.[PeriodId]
							AND [EmployeeId] = peT.[EmployeeId]
						)
				)
			OR (
				NOT EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
					WHERE empS.[OwnerId] = @ModifiedBy
						AND empS.[EmpId] = #Violations.[EmployeeId]
						AND empS.[ActionType] = 3
					)
				AND NOT EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
						INNER JOIN [dbo].[Employee] AS emp WITH (NOLOCK) ON grS.[GroupId] = emp.[GroupId]
					WHERE grS.[OwnerId] = @ModifiedBy
						AND grS.[ActionType] = 3
						AND emp.[EmployeeId] = #Violations.[EmployeeId]
						AND emp.[EmployeeId] <> @ModifiedBy
						--AND NOT EXISTS (
						--	SELECT TOP 1 1
						--	FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
						--	WHERE empS.[OwnerId] <> @ModifiedBy
						--		AND empS.[EmpId] = #Violations.[EmployeeId]
						--		AND empS.[ActionType] = 3
						--	)
					)
				)

		BEGIN TRY
			BEGIN TRANSACTION

			DELETE
			FROM [dbo].[AttendanceDetailIgnored]
			WHERE EXISTS (
					SELECT TOP 1 1
					FROM #Violations AS vi
					WHERE vi.[WorkDate] = [dbo].[AttendanceDetailIgnored].[WorkDate]
						AND vi.[EmployeeId] = [dbo].[AttendanceDetailIgnored].[EmployeeId]
						AND vi.[ViolationId] = [dbo].[AttendanceDetailIgnored].[ViolationId]
					)

			INSERT INTO [dbo].[AttendanceDetailIgnored] (
				[WorkDate]
				,[EmployeeId]
				,[ViolationId]
				,[ModifiedBy]
				,[ModifiedDate]
				)
			SELECT [WorkDate]
				,[EmployeeId]
				,[ViolationId]
				,@ModifiedBy
				,GETDATE()
			FROM #Violations
			WHERE [Ignored] = 1

			COMMIT TRANSACTION

			DECLARE @Data NVARCHAR(MAX)

			EXEC @Data = [dbo].[fncGetDataForWriteLog_Discipline_IgnoredViolations] @IgnoredViolations = @IgnoredViolations
			EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy, @SpName = 'EpaAttendanceSummary_IgnoredViolation', @ObjectId = NULL, @ObjectTypeId = 9, @Data = @Data, @OldData = NULL, @ObjectIds = NULL

		END TRY

		BEGIN CATCH
			COMMIT TRANSACTION

			SELECT @ErrCode = 1
				,@ErrMsg = N'Ignored violations unsuccessfully.'
		END CATCH
	END
END