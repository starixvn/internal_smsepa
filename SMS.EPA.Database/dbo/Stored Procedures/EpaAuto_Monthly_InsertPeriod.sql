/****** Object:  Stored Procedure dbo.EpaAuto_Monthly_InsertPeriod    Script Date: 7/21/2016 3:01:36 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_Monthly_InsertPeriod    Script Date: 7/21/2016 2:45:48 PM ******/

CREATE PROCEDURE [dbo].[EpaAuto_Monthly_InsertPeriod] @PeriodId INT OUTPUT
	,@Name NVARCHAR(200)
	,@StartDate DATETIME
	,@EndDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160223@Tim.Trinh
Task: Insert monthly period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM [dbo].[Period] WITH (NOLOCK)
			WHERE [Name] = @Name
				AND TargetTypeId = 2 -- monthly
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Period ''' + @Name + ''' has already existed.'

		RETURN
	END

	IF EXISTS (
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

		INSERT INTO [dbo].[Period] (
			[Name]
			,[TargetTypeId]
			,[StartDate]
			,[EndDate]
			,[IsActive]
			,[CreatedDate]
			)
		VALUES (
			@Name
			,2 -- monthly
			,@StartDate
			,@EndDate
			,1
			,GETDATE()
			)

		SET @PeriodId = SCOPE_IDENTITY()

		INSERT INTO [dbo].[PeriodProcessSetting] (
			[PeriodId]
			,[ProcessId]
			,[Duration]
			,[NoticeAtBegin]
			,[RemindDay]
			)
		SELECT @PeriodId
			,[ProcessId]
			,[Duration]
			,[NoticeAtBegin]
			,[RemindDay]
		FROM [dbo].[EvaluationGlobalSetting] WITH (NOLOCK)
		WHERE TargetTypeId = 2 -- monthly

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert Period unsuccessfully.'
	END CATCH
END