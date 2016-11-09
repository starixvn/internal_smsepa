
CREATE PROCEDURE [dbo].[EpaPeriod_Insert] @PeriodId INT = 0 OUTPUT
	,@Name NVARCHAR(200) = NULL
	,@TargetTypeId INT
	,@StartDate DATETIME = NULL
	,@EndDate DATETIME = NULL
	,@AttendanceStartDate DATETIME = NULL
	,@AttendanceEndDate DATETIME = NULL
	,@IsActive BIT = 1
	,@Description NVARCHAR(MAX) = NULL
	,@CreatedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Insert new evaluation period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	IF EXISTS (
			SELECT 1
			FROM dbo.Period WITH (NOLOCK)
			WHERE [Name] = @Name
				AND TargetTypeId = @TargetTypeId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Period ''' + @Name + ''' has already existed.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.Period WITH (NOLOCK)
			WHERE TargetTypeId = 1 -- Probation
				AND TargetTypeId = @TargetTypeId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The Target Type Probation has already existed period.'

		RETURN
	END

	IF @TargetTypeId = 2 -- monthly
		AND EXISTS (
			SELECT 1
			FROM dbo.Period WITH (NOLOCK)
			WHERE TargetTypeId = 2
				AND (
					CAST(@StartDate AS DATE) BETWEEN CAST(StartDate AS DATE)
						AND CAST(EndDate AS DATE)
					OR CAST(@EndDate AS DATE) BETWEEN CAST(StartDate AS DATE)
						AND CAST(EndDate AS DATE)
					OR CAST(StartDate AS DATE) BETWEEN CAST(@StartDate AS DATE)
						AND CAST(@EndDate AS DATE)
					OR CAST(EndDate AS DATE) BETWEEN CAST(@StartDate AS DATE)
						AND CAST(@EndDate AS DATE)
					)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The Start Date/End Date overlap on another period.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION

		INSERT INTO dbo.Period (
			[Name]
			,TargetTypeId
			,StartDate
			,EndDate
			,AttendanceStartDate
			,AttendanceEndDate
			,IsActive
			,[Description]
			,EvaluationTimes
			,CreatedBy
			,CreatedDate
			)
		VALUES (
			@Name
			,@TargetTypeId
			,@StartDate
			,@EndDate
			,@AttendanceStartDate
			,@AttendanceEndDate
			,@IsActive
			,@Description
			,CASE 
				WHEN @TargetTypeId = 1 THEN 1 -- probation
				ELSE NULL
				END
			,@CreatedBy
			,GETDATE()
			)

		SET @PeriodId = SCOPE_IDENTITY()

		INSERT INTO dbo.PeriodProcessSetting (
			PeriodId
			,ProcessId
			,Duration
			,NoticeAtBegin
			,RemindDay
			)
		SELECT @PeriodId
			,ProcessId
			,Duration
			,NoticeAtBegin
			,RemindDay
		FROM dbo.EvaluationGlobalSetting WITH (NOLOCK)
		WHERE TargetTypeId = @TargetTypeId

		DECLARE @Data NVARCHAR(MAX)

		EXEC @Data = dbo.fncGetDataForWriteLog @ObjectId = @PeriodId
			,@ObjectType = 8

		EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
			,@SpName = 'EpaPeriod_Insert'
			,@ObjectId = @PeriodId
			,@ObjectTypeId = 8
			,@Data = @Data
			,@OldData = NULL
			,@ObjectIds = NULL

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert Period unsuccessfully.'
	END CATCH
END