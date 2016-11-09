
CREATE PROCEDURE [dbo].[EpaPeriod_UpdateBasicInformation] @PeriodId INT
	,@Name NVARCHAR(200) = NULL
	,@TargetTypeId INT
	,@StartDate DATE = NULL
	,@EndDate DATE = NULL
	,@AttendanceStartDate DATE = NULL
	,@AttendanceEndDate DATE = NULL
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
		,@OldData NVARCHAR(MAX)
		,@OldEndDate DATE

	EXEC @OldData = [dbo].[fncGetDataForWriteLog] @ObjectId = @PeriodId
		,@ObjectType = 8

	BEGIN TRY
		BEGIN TRANSACTION

		SELECT @OldEndDate = EndDate
		FROM dbo.Period WITH (NOLOCK)
		WHERE PeriodId = @PeriodId

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

		IF @TargetTypeId IN (2, 3)
			AND (
				(
					ISNULL(@OldEndDate, '') <> ''
					AND ISNULL(@EndDate, '') <> ''
					AND @OldEndDate <> @EndDate
					)
				OR (
					ISNULL(@OldEndDate, '') = ''
					AND ISNULL(@EndDate, '') <> ''
					)
				)
		BEGIN
			UPDATE epa
			SET epa.[Status] = (CASE 
					WHEN CAST(period.EndDate AS DATE) > CAST(pc.EndDate AS DATE) THEN 0
					WHEN CAST(period.EndDate AS DATE) < CAST(GETDATE() AS DATE)
						AND CAST(pc.EndDate AS DATE) < CAST(GETDATE() AS DATE) THEN 1 END
					)
			FROM dbo.EPA AS epa
				INNER JOIN (
				SELECT DISTINCT EPAId
					,MAX(EndDate) AS EndDate
				FROM dbo.EPAProcess
				GROUP BY EPAId
				) AS pc ON epa.EPAId = pc.EPAId
				INNER JOIN dbo.Period AS period ON epa.PeriodId = period.PeriodId
			WHERE period.PeriodId = @PeriodId
				AND (
					epa.[Status] = 0
					OR epa.[Status] = 1
					)
		END

		EXEC @Data = [dbo].[fncGetDataForWriteLog] @ObjectId = @PeriodId
			,@ObjectType = 8

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'EpaPeriod_UpdateBasicInformation'
			,@ObjectId = @PeriodId
			,@ObjectTypeId = 8
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Period unsuccessfully.'
	END CATCH
END