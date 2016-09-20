
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_TargetTemplate_SetKPIGroup] (@ObjectId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
		,@KpiGroups NVARCHAR(MAX)

	SET @KpiGroups = ''

	SELECT @KpiGroups = @KpiGroups + '<gr><id>' + CAST([KPiGroupId] AS NVARCHAR(10)) + '</id><w>' + CAST([Weight] AS NVARCHAR(10)) + '</w><s>' + CAST([IsSpreadKPI] AS NVARCHAR) + '</s></gr>'
	FROM [dbo].[TargetTemplateKPIGroup] WITH (NOLOCK)
	WHERE [TargetTemplateId] = @ObjectId

	SELECT @Data = CONCAT ('<r><id>', CAST(@ObjectId AS NVARCHAR(10)), '</id><grs>', @KpiGroups, '</grs></r>')

	RETURN @Data
END