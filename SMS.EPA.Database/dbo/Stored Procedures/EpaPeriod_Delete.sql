/****** Object:  Stored Procedure dbo.EpaPeriod_Delete    Script Date: 5/16/2016 2:20:08 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_Delete    Script Date: 5/16/2016 2:15:30 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_Delete    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_Delete    Script Date: 4/6/2016 11:27:45 AM ******/

CREATE PROCEDURE [dbo].[EpaPeriod_Delete] @PeriodId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Delete evaluation period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[Period] WITH (NOLOCK)
			WHERE [PeriodId] = @PeriodId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The Period does not exists.'

		RETURN
	END

	IF EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[Period] AS pe WITH (NOLOCK)
				INNER JOIN [dbo].[EPA] AS epa WITH (NOLOCK) ON pe.[PeriodId] = epa.[PeriodId]
			WHERE pe.[PeriodId] = @PeriodId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete Period unsuccessfully. It has been already existed in EPA.'

		RETURN
	END

	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog] @ObjectId = @PeriodId, @ObjectType = 8

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE
		FROM [dbo].[Period]
		WHERE [PeriodId] = @PeriodId
		
		DELETE
		FROM [dbo].[PeriodExcludedEmployee]
		WHERE [PeriodId] = @PeriodId
		
		DELETE
		FROM [dbo].[PeriodFlowSetting]
		WHERE [PeriodId] = @PeriodId
		
		DELETE
		FROM [dbo].[PeriodProcessSetting]
		WHERE [PeriodId] = @PeriodId
		
		DELETE
		FROM [dbo].[PeriodTargetSetting]
		WHERE [PeriodId] = @PeriodId

		COMMIT TRANSACTION

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy, @SpName = 'EpaPeriod_Delete', @ObjectId = @PeriodId, @ObjectTypeId = 8, @Data = NULL, @OldData = @OldData, @ObjectIds = NULL
		
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete Period unsuccessfully.'
	END CATCH
END