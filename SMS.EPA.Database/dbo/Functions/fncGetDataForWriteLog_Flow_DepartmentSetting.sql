
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Flow_DepartmentSetting] (
	@DepartmentId INT
	,@FlowTemplateId INT
	,@IsGroup BIT
	)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
		,@Flows NVARCHAR(MAX)

	SET @Flows = ''

	SELECT @Flows = @Flows + '<fl><pr>' + CAST([ProcessId] AS NVARCHAR) + '</pr><eva>' + CAST([EvaluatorId] AS NVARCHAR(10)) + '</eva></fl>'
	FROM [dbo].[SettingDepartmentFlow] WITH (NOLOCK)
	WHERE [FlowTemplateId] = @FlowTemplateId
		AND [DepartmentId] = @DepartmentId
		AND [IsGroup] = @IsGroup

	SELECT @Data = CONCAT ('<r><fid>', CAST(@FlowTemplateId AS NVARCHAR(10)), '</fid><did>', CAST(@DepartmentId AS NVARCHAR(10)), '</did><ig>', CAST(@IsGroup AS NVARCHAR), '</ig><fls>', @Flows, '</fls></r>')

	RETURN @Data
END