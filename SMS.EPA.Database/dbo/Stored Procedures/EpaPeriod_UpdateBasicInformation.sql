/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateBasicInformation    Script Date: 5/16/2016 2:20:08 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateBasicInformation    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateBasicInformation    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateBasicInformation    Script Date: 4/6/2016 11:27:45 AM ******/

CREATE PROCEDURE [dbo].[EpaPeriod_UpdateBasicInformation] @PeriodId INT
	,@Name NVARCHAR(200) = NULL
	,@TargetTypeId INT
	,@StartDate DATETIME = NULL
	,@EndDate DATETIME = NULL
	,@AttendanceStartDate DATETIME = NULL
	,@AttendanceEndDate DATETIME = NULL
	,@IsActive BIT = 1
	,@Description NVARCHAR(MAX) = NULL
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Update basic information of evaluation period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM [dbo].[Period] WITH (NOLOCK)
			WHERE [PeriodId] = @PeriodId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Period ''' + @Name + ''' does not exists.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM [dbo].[Period] WITH (NOLOCK)
			WHERE [PeriodId] <> @PeriodId
				AND [Name] = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Period ''' + @Name + ''' has already existed.'

		RETURN
	END

	IF @TargetTypeId = 2 -- monthly
		AND EXISTS (
			SELECT 1
			FROM dbo.Period WITH (NOLOCK)
			WHERE [PeriodId] <> @PeriodId
				AND TargetTypeId = 2
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

	DECLARE @Data NVARCHAR(MAX)
	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog] @ObjectId = @PeriodId
		,@ObjectType = 8

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE [dbo].[Period] WITH (ROWLOCK, UPDLOCK)
		SET [Name] = @Name
			,[TargetTypeId] = @TargetTypeId
			,[StartDate] = @StartDate
			,[EndDate] = @EndDate
			,[AttendanceStartDate] = @AttendanceStartDate
			,[AttendanceEndDate] = @AttendanceEndDate
			,[IsActive] = @IsActive
			,[Description] = @Description
			,[ModifiedBy] = @ModifiedBy
			,[ModifiedDate] = GETDATE()
		WHERE [PeriodId] = @PeriodId

		COMMIT TRANSACTION

		EXEC @Data = [dbo].[fncGetDataForWriteLog] @ObjectId = @PeriodId
			,@ObjectType = 8

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'EpaPeriod_UpdateBasicInformation'
			,@ObjectId = @PeriodId
			,@ObjectTypeId = 8
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Period unsuccessfully.'
	END CATCH
END