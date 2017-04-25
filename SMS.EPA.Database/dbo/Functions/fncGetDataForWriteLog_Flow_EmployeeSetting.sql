

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Flow_EmployeeSetting] (
	@FlowTemplateId INT
	,@EmployeeIds NVARCHAR(MAX)
	)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
		,@Flows NVARCHAR(MAX)
		,@EmpIds IntId

	IF ISNULL(@EmployeeIds, '') <> ''
	BEGIN
		INSERT INTO @EmpIds (Id)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@EmployeeIds, ',')
	END

	SET @Flows = ''

	SELECT @Flows = @Flows + '<fl><pr>' + CAST(fs.ProcessId AS NVARCHAR(2)) + '</pr><eva>' + CAST(fs.EvaluatorId AS NVARCHAR(10)) + '</eva></fl>'
	FROM dbo.SettingEmployeeFlow AS fs WITH (NOLOCK)
		INNER JOIN @EmpIds AS emp ON fs.EmployeeId = emp.Id
	WHERE fs.FlowTemplateId = @FlowTemplateId

	SELECT @Data = CONCAT ('<r><fid>', CAST(@FlowTemplateId AS NVARCHAR(10)), '</fid><eid>', CAST(@EmployeeIds AS NVARCHAR(10)), '</eid><fls>', @Flows, '</fls></r>')

	RETURN @Data
END