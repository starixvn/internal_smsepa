/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_TargetTemplateSetting_Employee    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_TargetTemplateSetting_Employee    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_TargetTemplateSetting_Employee    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_TargetTemplateSetting_Employee    Script Date: 4/6/2016 11:27:50 AM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_TargetTemplateSetting_Employee] (@EmployeeId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)	
	DECLARE @TargetTemplateIds NVARCHAR(100)

	SET @TargetTemplateIds = ''

	SELECT @TargetTemplateIds = @TargetTemplateIds + ',' + CAST([TargetTemplateId] AS NVARCHAR)
					FROM [dbo].[TargetTemplateEmployee] WITH (NOLOCK)
					WHERE [EmployeeId] = @EmployeeId
	
	SELECT @TargetTemplateIds = STUFF(@TargetTemplateIds, 1, 1, '')				

	SELECT @Data = CONCAT('<emp><id>', @EmployeeId, '</id><tts>', @TargetTemplateIds, '</tts></emp>')

	RETURN @Data
END