
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_TargetTemplateSetting_Department] (
	@DepartmentId INT
	,@IsGroup BIT
	)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
		,@TargetTemplateIds NVARCHAR(100)

	SET @TargetTemplateIds = ''

	SELECT @TargetTemplateIds = @TargetTemplateIds + ',' + CAST([TargetTemplateId] AS NVARCHAR)
	FROM [dbo].[TargetTemplateDepartment] WITH (NOLOCK)
	WHERE [DepartmentId] = @DepartmentId
		AND [IsGroup] = @IsGroup

	SELECT @TargetTemplateIds = STUFF(@TargetTemplateIds, 1, 1, '')

	SELECT @Data = CONCAT ('<de><id>', @DepartmentId, '</id><ig>', @IsGroup, '</ig><tts>', @TargetTemplateIds, '</tts></de>')

	RETURN @Data
END