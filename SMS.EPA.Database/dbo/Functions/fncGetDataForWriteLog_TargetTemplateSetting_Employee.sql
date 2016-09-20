
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_TargetTemplateSetting_Employee] (@EmployeeId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
		,@TargetTemplateIds NVARCHAR(100)

	SET @TargetTemplateIds = ''

	SELECT @TargetTemplateIds = @TargetTemplateIds + ',' + CAST([TargetTemplateId] AS NVARCHAR)
	FROM [dbo].[TargetTemplateEmployee] WITH (NOLOCK)
	WHERE [EmployeeId] = @EmployeeId

	SELECT @TargetTemplateIds = STUFF(@TargetTemplateIds, 1, 1, '')

	SELECT @Data = CONCAT ('<emp><id>', @EmployeeId, '</id><tts>', @TargetTemplateIds, '</tts></emp>')

	RETURN @Data
END