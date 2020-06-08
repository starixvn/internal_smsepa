
CREATE PROCEDURE [dbo].[SettingDepartmentFlow_Set] @DepartmentId INT
	,@FlowTemplateId INT
	,@IsGroup BIT
	,@FlowDetails VARCHAR(5000)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150720@Tim.Trinh
Task: Save department flow setting.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
	• @ProcessDetails: 
		N'<ROOT>
			  <Process ProcessId="1" Duration="1" HasSendMail="True" RemindDay="1">
				  <Evaluator EvaluatorId="125" Priority="1" />
			  </Process>
		</ROOT>'
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @DepartmentFlows TABLE (
		Duration INT
		,HasSendMail BIT
		,RemindDay TINYINT
		,ProcessId INT
		,EvaluatorId INT
		,[Priority] TINYINT
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@FlowDetails

		INSERT INTO @DepartmentFlows (
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
			,@ErrMsg = N'Save department flow setting unsuccessfully. Paramter invalid.'

		RETURN
	END CATCH

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_Flow_DepartmentSetting] @DepartmentId = @DepartmentId
		,@FlowTemplateId = @FlowTemplateId
		,@IsGroup = @IsGroup

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE
		FROM dbo.SettingDepartmentFlow
		WHERE DepartmentId = @DepartmentId
			AND FlowTemplateId = @FlowTemplateId
			AND IsGroup = @IsGroup

		INSERT INTO dbo.SettingDepartmentFlow (
			DepartmentId
			,ProcessId
			,EvaluatorId
			,IsGroup
			,FlowTemplateId
			,[Priority]
			,Duration
			,HasSendMail
			,RemindDay
			,ModifiedBy
			,ModifiedDate
			)
		SELECT @DepartmentId
			,df.ProcessId
			,df.EvaluatorId
			,@IsGroup
			,@FlowTemplateId
			,df.[Priority]
			,df.Duration
			,df.HasSendMail
			,df.RemindDay
			,@ModifiedBy
			,GETDATE()
		FROM @DepartmentFlows AS df

		EXEC @Data = [dbo].[fncGetDataForWriteLog_Flow_DepartmentSetting] @DepartmentId = @DepartmentId
			,@FlowTemplateId = @FlowTemplateId
			,@IsGroup = @IsGroup

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'SettingDepartmentFlow_Set'
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
			,@ErrMsg = N'Save department flow setting unsuccessfully.'
	END CATCH
END