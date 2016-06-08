/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_TargetTemplate_SetKPIGroup    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_TargetTemplate_SetKPIGroup    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_TargetTemplate_SetKPIGroup    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_TargetTemplate_SetKPIGroup    Script Date: 4/6/2016 11:27:50 AM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_TargetTemplate_SetKPIGroup] (@ObjectId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
	DECLARE @KpiGroups NVARCHAR(MAX)

	SET @KpiGroups = ''

	SELECT @KpiGroups = @KpiGroups + '<gr><id>' + CAST([KPiGroupId] AS NVARCHAR(10)) + '</id><w>' + CAST([Weight] AS NVARCHAR(10)) + '</w><s>' + CAST([IsSpreadKPI] AS NVARCHAR) + '</s></gr>'
	FROM [dbo].[TargetTemplateKPIGroup] WITH (NOLOCK)
	WHERE [TargetTemplateId] = @ObjectId

	SELECT @Data = CONCAT ('<r><id>', CAST(@ObjectId AS NVARCHAR(10)), '</id><grs>', @KpiGroups, '</grs></r>')

	RETURN @Data
END