/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_TargetTemplateSetting_Department    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_TargetTemplateSetting_Department    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_TargetTemplateSetting_Department    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_TargetTemplateSetting_Department    Script Date: 4/6/2016 11:27:50 AM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_TargetTemplateSetting_Department] (
			@DepartmentId INT
			,@IsGroup BIT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)	
	DECLARE @TargetTemplateIds NVARCHAR(100)

	SET @TargetTemplateIds = ''

	SELECT @TargetTemplateIds = @TargetTemplateIds + ',' + CAST([TargetTemplateId] AS NVARCHAR)
					FROM [dbo].[TargetTemplateDepartment] WITH (NOLOCK)
					WHERE [DepartmentId] = @DepartmentId
					AND [IsGroup] = @IsGroup
	
	SELECT @TargetTemplateIds = STUFF(@TargetTemplateIds, 1, 1, '')				

	SELECT @Data = CONCAT('<de><id>', @DepartmentId, '</id><ig>', @IsGroup, '</ig><tts>', @TargetTemplateIds, '</tts></de>')

	RETURN @Data
END