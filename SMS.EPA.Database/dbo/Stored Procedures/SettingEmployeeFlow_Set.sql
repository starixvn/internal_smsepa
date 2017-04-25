

CREATE PROCEDURE [dbo].[SettingEmployeeFlow_Set] @EmployeeIds NVARCHAR(MAX)
	,@FlowTemplateId INT
	,@FlowDetails NVARCHAR(MAX)
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
	SET NOCOUNT ON

	DECLARE @EmpIds IntId

	CREATE TABLE #EmployeeFlows (
		ProcessId INT
		,EvaluatorId INT
		,Duration INT
		,HasSendMail BIT
		,RemindDay TINYINT
		,[Priority] TINYINT
		)

	IF ISNULL(@EmployeeIds, '') <> ''
	BEGIN
		INSERT INTO @EmpIds (Id)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@EmployeeIds, ',')
	END

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@FlowDetails

		INSERT INTO #EmployeeFlows (
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
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_Flow_EmployeeSetting] @FlowTemplateId = @FlowTemplateId
		,@EmployeeIds = @EmployeeIds

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE fs
		FROM dbo.SettingEmployeeFlow AS fs
			INNER JOIN @EmpIds AS emp ON fs.EmployeeId = emp.Id
		WHERE fs.FlowTemplateId = @FlowTemplateId

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
		SELECT emp.Id
			,ef.ProcessId
			,ef.EvaluatorId
			,@FlowTemplateId
			,ef.[Priority]
			,ef.Duration
			,ef.HasSendMail
			,ef.RemindDay
			,@ModifiedBy
			,GETDATE()
		FROM #EmployeeFlows AS ef
			INNER JOIN @EmpIds AS emp ON 1 = 1

		EXEC @Data = [dbo].[fncGetDataForWriteLog_Flow_EmployeeSetting] @FlowTemplateId = @FlowTemplateId
			,@EmployeeIds = @EmployeeIds

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'SettingEmployeeFlow_Set'
			,@ObjectId = @FlowTemplateId
			,@ObjectTypeId = 6
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Save employee flow setting unsuccessfully.'
	END CATCH

	DROP TABLE #EmployeeFlows
END