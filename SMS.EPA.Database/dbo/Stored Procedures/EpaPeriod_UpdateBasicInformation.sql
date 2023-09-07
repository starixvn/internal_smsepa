
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

	DECLARE @Today DATE = GETDATE()
		,@MonthlyId INT = 2
		,@MonthlyTarget TINYINT = 2
		,@SemiTarget TINYINT = 3
		,@PromotionTarget TINYINT = 5
		,@OpenStatus TINYINT = 0
		,@ClosedStatus TINYINT = 1
		,@RejectedStatus TINYINT = 2

	IF EXISTS (
			SELECT 1
			FROM dbo.Period WITH (NOLOCK)
			WHERE PeriodId != @PeriodId
				AND [Name] = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Period ''' + @Name + ''' has already existed.'

		RETURN
	END

	IF @TargetTypeId = @MonthlyId
		AND EXISTS (
			SELECT 1
			FROM dbo.Period WITH (NOLOCK)
			WHERE PeriodId != @PeriodId
				AND TargetTypeId = @MonthlyId
				AND (
					@StartDate BETWEEN StartDate
						AND EndDate
					OR @EndDate BETWEEN StartDate
						AND EndDate
					OR StartDate BETWEEN @StartDate
						AND @EndDate
					OR EndDate BETWEEN @StartDate
						AND @EndDate
					)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Start Date/End Date overlap with another period.'

		RETURN
	END

	DECLARE @data NVARCHAR(MAX)
		,@oldData NVARCHAR(MAX)
		,@xml XML
		,@OldStartDate DATE
		,@OldEndDate DATE

	SET @xml = (
			SELECT [Name] AS '@Name'
				,TargetTypeId AS '@TargetTypeId'
				,StartDate AS '@StartDate'
				,EndDate AS '@EndDate'
				,AttendanceStartDate AS '@AttendanceStartDate'
				,AttendanceEndDate AS '@AttendanceEndDate'
				,IsActive AS '@IsActive'
				,[Description] AS '@Description'
				,ModifiedBy AS '@ModifiedBy'
			FROM dbo.Period WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	BEGIN TRY
		BEGIN TRANSACTION

		SELECT @OldStartDate = StartDate
			,@OldEndDate = EndDate
		FROM dbo.Period WITH (NOLOCK)
		WHERE PeriodId = @PeriodId

		UPDATE dbo.Period WITH (ROWLOCK, UPDLOCK)
		SET [Name] = @Name
			,TargetTypeId = @TargetTypeId
			,StartDate = @StartDate
			,EndDate = @EndDate
			,AttendanceStartDate = @AttendanceStartDate
			,AttendanceEndDate = @AttendanceEndDate
			,IsActive = @IsActive
			,[Description] = @Description
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		WHERE PeriodId = @PeriodId

		IF @TargetTypeId IN (@MonthlyTarget, @SemiTarget, @PromotionTarget)
		BEGIN
			IF ISNULL(@OldStartDate, '') != ISNULL(@StartDate, '')
			BEGIN
				UPDATE pe
				SET pe.StartDate = DATEADD(DAY, ISNULL(aa.Duration - peP.Duration, 0), @StartDate)
					,pe.EndDate = DATEADD(DAY, (CASE 
							WHEN ISNULL(aa.Duration, 0) - 1 < 0 THEN 0
							ELSE ISNULL(aa.Duration, 0) - 1 END
							), @StartDate)
				FROM dbo.PeriodFlowSetting AS pe WITH (NOLOCK)
					INNER JOIN dbo.PeriodProcessSetting AS peP WITH (NOLOCK) ON pe.PeriodId = peP.PeriodId
					AND pe.ProcessId = peP.ProcessId
					INNER JOIN (
					SELECT set1.PeriodId
						,set1.ProcessId
						,SUM(CASE 
								WHEN set2.ProcessId <= set1.ProcessId THEN set2.Duration
								ELSE 0
								END) AS Duration
					FROM dbo.PeriodProcessSetting AS set1 WITH (NOLOCK)
						INNER JOIN dbo.PeriodProcessSetting AS set2 WITH (NOLOCK) ON set1.PeriodId = set2.PeriodId
					WHERE set1.PeriodId = @PeriodId
					GROUP BY set1.PeriodId
						,set1.ProcessId
					) AS aa ON pe.PeriodId = aa.PeriodId
					AND pe.ProcessId = aa.ProcessId
				WHERE pe.PeriodId = @PeriodId

				UPDATE epaP WITH (ROWLOCK, UPDLOCK)
				SET epaP.StartDate = empP.StartDate
					,epaP.RemindDate = DATEADD(DAY, DATEDIFF(DAY, epaP.EndDate, empP.EndDate), epaP.RemindDate)
					,epaP.EndDate = empP.EndDate
					,epaP.[Priority] = empP.[Priority]
					,epaP.IsEndProcess = CASE 
						WHEN @TargetTypeId = @SemiTarget
							AND epaP.ProcessId IN (2, 3) -- self
							AND epa.HasMonthlyEPA = 1 THEN 1 -- end progress
						ELSE CASE 
								WHEN empP.EndDate < @Today THEN 1 -- end progress
								ELSE 0 -- in progress
								END
						END
				FROM dbo.EPAProcess AS epaP
					INNER JOIN dbo.EPA AS epa ON epaP.EPAId = epa.EPAId
					INNER JOIN [dbo].[PeriodFlowSetting] AS empP WITH (NOLOCK) ON epaP.ProcessId = empP.ProcessId
					AND epaP.EvaluatorId = empP.EvaluatorId
					AND empP.PeriodId = epa.PeriodId
					AND empP.EmployeeId = epa.EmployeeId
			END
		END

		IF ISNULL(@OldStartDate, '') != ISNULL(@StartDate, '')
			OR ISNULL(@OldEndDate, '') != ISNULL(@EndDate, '')
		BEGIN
			UPDATE epa
			SET epa.[Status] = (CASE 
					WHEN period.TargetTypeId IN (@MonthlyTarget, @SemiTarget)
						AND (
							period.EndDate > pc.EndDate
							AND period.EndDate < @Today
							) THEN @ClosedStatus
					WHEN period.TargetTypeId IN (@MonthlyTarget, @SemiTarget)
						AND (
							period.EndDate < pc.EndDate
							AND pc.EndDate < @Today
							) THEN @ClosedStatus
					WHEN period.TargetTypeId NOT IN (@MonthlyTarget, @SemiTarget)
						AND pc.EndDate < @Today THEN @ClosedStatus
					WHEN period.EndDate IS NOT NULL
						AND period.EndDate > @Today THEN @OpenStatus
					WHEN pc.EndDate >= @Today THEN @OpenStatus END
					)
			FROM dbo.EPA AS epa WITH (NOLOCK)
				INNER JOIN (
				SELECT DISTINCT EPAId
					,MAX(EndDate) AS EndDate
				FROM dbo.EPAProcess WITH (NOLOCK)
				GROUP BY EPAId
				) AS pc ON epa.EPAId = pc.EPAId
				INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			WHERE period.PeriodId = @PeriodId
				AND ISNULL(epa.[Status], 0) IN (@OpenStatus, @ClosedStatus)
		END

		SET @xml = (
				SELECT [Name] AS '@Name'
					,TargetTypeId AS '@TargetTypeId'
					,StartDate AS '@StartDate'
					,EndDate AS '@EndDate'
					,AttendanceStartDate AS '@AttendanceStartDate'
					,AttendanceEndDate AS '@AttendanceEndDate'
					,IsActive AS '@IsActive'
					,[Description] AS '@Description'
					,ModifiedBy AS '@ModifiedBy'
				FROM dbo.Period WITH (NOLOCK)
				WHERE PeriodId = @PeriodId
				FOR XML PATH('r')
					,TYPE
				)
		SET @data = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'EpaPeriod_UpdateBasicInformation'
			,@ObjectId = @PeriodId
			,@ObjectTypeId = 8
			,@Data = @data
			,@OldData = @oldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Period unsuccessfully.'
	END CATCH
END