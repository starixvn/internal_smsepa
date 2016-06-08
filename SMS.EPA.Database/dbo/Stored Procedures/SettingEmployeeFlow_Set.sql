/****** Object:  Stored Procedure dbo.SettingEmployeeFlow_Set    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.SettingEmployeeFlow_Set    Script Date: 5/16/2016 2:15:32 PM ******/
/****** Object:  Stored Procedure dbo.SettingEmployeeFlow_Set    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.SettingEmployeeFlow_Set    Script Date: 4/6/2016 11:27:47 AM ******/

CREATE PROCEDURE [dbo].[SettingEmployeeFlow_Set] @EmployeeId INT
	,@FlowTemplateId INT
	,@FlowDetails VARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150722@Tim.Trinh
Task: Save employee flow setting.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
• @FlowDetails:
    N'<ROOT>
		  <Process ProcessId="1" Duration="1" HasSendMail="True" RemindDay="1">
		      <Evaluator EvaluatorId="125"  Priority="1" />
		  </Process>
    </ROOT>'
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmployeeFlows TABLE (
		ProcessId INT
		,EvaluatorId INT
		,Duration INT
		,HasSendMail BIT
		,RemindDay TINYINT
		,[Priority] TINYINT
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@FlowDetails

		INSERT INTO @EmployeeFlows (
			ProcessId
			,EvaluatorId
			,[Priority]
			,Duration
			,HasSendMail
			,RemindDay
			)
		SELECT ProcessId
			,EvaluatorId
			,[Priority]
			,Duration
			,HasSendMail
			,RemindDay
		FROM OPENXML(@DocHandler, '/ROOT/Process/Evaluator', 1) WITH (ProcessId INT '../@ProcessId', EvaluatorId INT '@EvaluatorId', [Priority] TINYINT '@Priority', Duration INT '../@Duration', HasSendMail BIT '../@HasSendMail', RemindDay TINYINT '../@RemindDay') AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Save employee flow setting unsuccessfully. Paramter invalid.'

		RETURN
	END CATCH

	DECLARE @Data NVARCHAR(MAX)
	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_Flow_EmployeeSetting] @FlowTemplateId = @FlowTemplateId, @EmployeeId = @EmployeeId

	BEGIN TRY
		BEGIN TRANSACTION;

		DELETE
		FROM dbo.SettingEmployeeFlow
		WHERE EmployeeId = @EmployeeId
			AND FlowTemplateId = @FlowTemplateId

		INSERT INTO dbo.SettingEmployeeFlow (
			EmployeeId
			,ProcessId
			,EvaluatorId
			,FlowTemplateId
			,[Priority]
			,Duration
			,HasSendMail
			,RemindDay
			,ModifiedBy
			,ModifiedDate
			)
		SELECT @EmployeeId
			,ef.ProcessId
			,ef.EvaluatorId
			,@FlowTemplateId
			,ef.[Priority]
			,ef.Duration
			,ef.HasSendMail
			,ef.RemindDay
			,@ModifiedBy
			,GETDATE()
		FROM @EmployeeFlows AS ef

		COMMIT TRANSACTION;

		EXEC @Data = [dbo].[fncGetDataForWriteLog_Flow_EmployeeSetting] @FlowTemplateId = @FlowTemplateId, @EmployeeId = @EmployeeId
		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy, @SpName = 'SettingEmployeeFlow_Set', @ObjectId = @FlowTemplateId, @ObjectTypeId = 6, @Data = @Data, @OldData = @OldData, @ObjectIds = NULL

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Save employee flow setting unsuccessfully.'
	END CATCH
END