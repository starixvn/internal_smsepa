/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_AddOtherViolations    Script Date: 5/16/2016 2:20:12 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_AddOtherViolations    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_AddOtherViolations    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_AddOtherViolations    Script Date: 4/6/2016 11:27:42 AM ******/

CREATE PROCEDURE [dbo].[EpaAttendanceSummary_AddOtherViolations] @EmployeeId INT
	,@WorkDate DATETIME
	,@Name NVARCHAR(50)
	,@Score FLOAT
	,@Comment NVARCHAR(500) = NULL
	,@CreatedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Add other violations to employee.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[EmployeeRole] WITH (NOLOCK)
			WHERE [EmployeeId] = @CreatedBy
				AND [RoleId] = 1 -- SA
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Invalid!'

		RETURN
	END

	IF EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[EmployeeOtherViolation] WITH (NOLOCK)
			WHERE [WorkDate] = CAST(@WorkDate AS DATE)
				AND [EmpId] = @EmployeeId
				AND [Name] = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The violation ' + @Name + ' has already exists for employee in violation date!'

		RETURN
	END

	IF EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[Period] AS pe WITH (NOLOCK)
				INNER JOIN [dbo].[PeriodTargetSetting] AS peT WITH (NOLOCK) ON pe.[PeriodId] = peT.[PeriodId]
				INNER JOIN [dbo].[EPA] AS epa WITH (NOLOCK) ON peT.[PeriodId] = epa.[PeriodId]
				AND peT.[EmployeeId] = epa.[EmployeeId]
			WHERE peT.[EmployeeId] = @EmployeeId
				AND CAST(pe.[EndDate] AS DATE) < CAST(GETDATE() AS DATE)
				AND CAST(@WorkDate AS DATE) <= CAST(pe.[AttendanceEndDate] AS DATE)
				AND CAST(@WorkDate AS DATE) >= CAST(pe.[AttendanceStartDate] AS DATE)
				AND NOT EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[PeriodExcludedEmployee] WITH (NOLOCK)
					WHERE [PeriodId] = pe.[PeriodId]
						AND [EmployeeId] = peT.[EmployeeId]
					)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The violation can not be inserted because violation date is belong to closed period!'

		RETURN
	END

	DECLARE @Data NVARCHAR(MAX)
	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_Discipline_OtherViolations] @EmployeeId = @EmployeeId

	BEGIN TRY
		BEGIN TRANSACTION

		INSERT INTO [dbo].[EmployeeOtherViolation] (
			[WorkDate]
			,[EmpId]
			,[Name]
			,[Score]
			,[Comment]
			,[CreatedBy]
			,[CreatedDate]
			)
		VALUES (
			CAST(@WorkDate AS DATE)
			,@EmployeeId
			,@Name
			,@Score
			,@Comment
			,@CreatedBy
			,GETDATE()
			)

		COMMIT TRANSACTION

		EXEC @Data = [dbo].[fncGetDataForWriteLog_Discipline_OtherViolations] @EmployeeId = @EmployeeId
		EXEC [dbo].[LogDetail_Insert] @ActorId = @CreatedBy, @SpName = 'EpaAttendanceSummary_AddOtherViolations', @ObjectId = @EmployeeId, @ObjectTypeId = 9, @Data = @Data, @OldData = @OldData, @ObjectIds = NULL

	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH
END