/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateProcessSetting    Script Date: 5/16/2016 2:20:09 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateProcessSetting    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateProcessSetting    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateProcessSetting    Script Date: 4/6/2016 11:27:45 AM ******/

CREATE PROCEDURE [dbo].[EpaPeriod_UpdateProcessSetting] @PeriodId INT
	,@ProcessSettings NVARCHAR(MAX)
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Update period process setting.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
	@ProcessSettings = '<ROOT>
							<ProcessSetting ProcessId='' Duration='' NoticeAtBegin='' RemindDay=''></ProcessSetting>
						</ROOT>'
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @DocHandler INT

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@ProcessSettings

	DECLARE @Data NVARCHAR(MAX)
	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_Period_ProcessSetting] @PeriodId = @PeriodId

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE
		FROM [dbo].[PeriodProcessSetting]
		WHERE [PeriodId] = @PeriodId

		INSERT INTO [dbo].[PeriodProcessSetting] (
			[PeriodId]
			,[ProcessId]
			,[Duration]
			,[RemindDay]
			,[NoticeAtBegin]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT @PeriodId
			,[ProcessId]
			,[Duration]
			,[RemindDay]
			,[NoticeAtBegin]
			,@ModifiedBy
			,GETDATE()
		FROM OPENXML(@DocHandler, '/ROOT/ProcessSetting', 1) WITH ([ProcessId] INT, [Duration] INT, [RemindDay] INT, [NoticeAtBegin] BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler

		COMMIT TRANSACTION

		EXEC @Data = [dbo].[fncGetDataForWriteLog_Period_ProcessSetting] @PeriodId = @PeriodId

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'EpaPeriod_UpdateProcessSetting'
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