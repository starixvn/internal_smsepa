/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateStatus    Script Date: 5/16/2016 2:20:09 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateStatus    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateStatus    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateStatus    Script Date: 4/6/2016 11:27:45 AM ******/

CREATE PROCEDURE [dbo].[EpaPeriod_UpdateStatus] @PeriodId INT
	,@IsActive BIT = 1
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
			SELECT TOP 1 1
			FROM [dbo].[Period] WITH (NOLOCK)
			WHERE [PeriodId] = @PeriodId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The Period does not exists.'

		RETURN
	END

	DECLARE @Data NVARCHAR(MAX)
	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_UpdateStatus] @ObjectId = @PeriodId, @ObjectType = 8

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE [dbo].[Period] WITH (ROWLOCK, UPDLOCK)
		SET [IsActive] = @IsActive
			,[ModifiedBy] = @ModifiedBy
			,[ModifiedDate] = GETDATE()
		WHERE [PeriodId] = @PeriodId

		COMMIT TRANSACTION

		EXEC @Data = [dbo].[fncGetDataForWriteLog_UpdateStatus] @ObjectId = @PeriodId, @ObjectType = 8
		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy, @SpName = 'EpaPeriod_UpdateStatus', @ObjectId = @PeriodId, @ObjectTypeId = 8, @Data = @Data, @OldData = @OldData, @ObjectIds = NULL

	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Period unsuccessfully.'
	END CATCH
END