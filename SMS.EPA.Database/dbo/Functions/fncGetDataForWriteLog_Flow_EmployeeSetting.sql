
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Flow_EmployeeSetting] (
	@FlowTemplateId INT
	,@EmployeeId INT
	)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
		,@Flows NVARCHAR(MAX)

	SET @Flows = ''

	SELECT @Flows = @Flows + '<fl><pr>' + CAST([ProcessId] AS NVARCHAR) + '</pr><eva>' + CAST([EvaluatorId] AS NVARCHAR(10)) + '</eva></fl>'
	FROM [dbo].[SettingEmployeeFlow] WITH (NOLOCK)
	WHERE [FlowTemplateId] = @FlowTemplateId
		AND [EmployeeId] = @EmployeeId

	SELECT @Data = CONCAT ('<r><fid>', CAST(@FlowTemplateId AS NVARCHAR(10)), '</fid><eid>', CAST(@EmployeeId AS NVARCHAR(10)), '</eid><fls>', @Flows, '</fls></r>')

	RETURN @Data
END