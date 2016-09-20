
CREATE PROCEDURE [dbo].[EpaPeriod_UpdateIncludeStatusEmployeeToPeriod] @PeriodId INT
	,@EmployeeId INT
	,@IsIncluded BIT = NULL
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Include employee out of period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)
		,@TargetTypeId INT

	SELECT @TargetTypeId = TargetTypeId
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	IF @IsIncluded = 1
		AND @TargetTypeId = 2 -- monthly
		AND EXISTS (
			SELECT 1
			FROM dbo.PeriodExcludedEmployee AS pEx WITH (NOLOCK)
				INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pEx.EmployeeId = emp.EmployeeId
				AND emp.IsDeleted = 0
				AND emp.[Enabled] = 1
				AND emp.HasMonthlyEPA <> 1
			WHERE pEx.PeriodId = @PeriodId
				AND emp.EmployeeId = @EmployeeId
			)
	BEGIN
		DECLARE @NickName NVARCHAR(20)

		SELECT @NickName = NickName
		FROM dbo.Employee WITH (NOLOCK)
		WHERE EmployeeId = @EmployeeId

		SELECT @ErrCode = 1
			,@ErrMsg = 'Cannot include <b>' + @NickName + '</b> to monthly period because <b>' + @NickName + '</b> has not been set monthly evaluation!'

		RETURN
	END

	EXEC @OldData = dbo.fncGetDataForWriteLog_Period_IncludeExcludeEmployee @PeriodId = @PeriodId
		,@EmployeeId = @EmployeeId

	BEGIN TRY
		BEGIN TRANSACTION

		IF @IsIncluded IS NULL
			OR @IsIncluded = 0
		BEGIN
			IF NOT EXISTS (
					SELECT 1
					FROM dbo.PeriodExcludedEmployee WITH (NOLOCK)
					WHERE PeriodId = @PeriodId
						AND EmployeeId = @EmployeeId
					)
			BEGIN
				INSERT INTO dbo.PeriodExcludedEmployee (
					PeriodId
					,EmployeeId
					,ModifiedBy
					,ModifiedDate
					)
				VALUES (
					@PeriodId
					,@EmployeeId
					,@ModifiedBy
					,GETDATE()
					)

				IF EXISTS (
						SELECT 1
						FROM dbo.EPA WITH (NOLOCK)
						WHERE PeriodId = @PeriodId
							AND EmployeeId = @EmployeeId
						)
				BEGIN
					UPDATE dbo.EPA WITH (ROWLOCK, UPDLOCK)
					SET [Status] = 2 -- Rejected
					WHERE PeriodId = @PeriodId
						AND EmployeeId = @EmployeeId
				END
			END
		END
		ELSE IF @IsIncluded = 1
		BEGIN
			DELETE
			FROM dbo.PeriodExcludedEmployee
			WHERE PeriodId = @PeriodId
				AND EmployeeId = @EmployeeId

			IF EXISTS (
					SELECT 1
					FROM dbo.EPA WITH (NOLOCK)
					WHERE PeriodId = @PeriodId
						AND EmployeeId = @EmployeeId
					)
			BEGIN
				DECLARE @EPAId INT

				SELECT @EPAId = EPAId
				FROM dbo.EPA WITH (NOLOCK)
				WHERE PeriodId = @PeriodId
					AND EmployeeId = @EmployeeId

				IF EXISTS (
						SELECT 1
						FROM dbo.EPAProcess WITH (NOLOCK)
						WHERE EPAId = @EPAId
							AND CAST(EndDate AS DATE) >= CAST(GETDATE() AS DATE)
						)
				BEGIN
					UPDATE dbo.EPA WITH (ROWLOCK, UPDLOCK)
					SET [Status] = 0 -- Opened
					WHERE EPAId = @EPAId
				END
				ELSE
				BEGIN
					UPDATE dbo.EPA WITH (ROWLOCK, UPDLOCK)
					SET [Status] = 1 -- Closed
					WHERE EPAId = @EPAId
				END
			END
		END

		EXEC @Data = dbo.fncGetDataForWriteLog_Period_IncludeExcludeEmployee @PeriodId = @PeriodId
			,@EmployeeId = @EmployeeId

		EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
			,@SpName = 'EpaPeriod_UpdateIncludeStatusEmployeeToPeriod'
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