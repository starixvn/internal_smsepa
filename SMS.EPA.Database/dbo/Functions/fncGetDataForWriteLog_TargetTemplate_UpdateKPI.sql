
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_TargetTemplate_UpdateKPI] (@ObjectId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
		,@KpiGroups NVARCHAR(MAX)
		,@Kpis NVARCHAR(MAX)

	SET @KpiGroups = ''
	SET @Kpis = ''

	SELECT @KpiGroups = @KpiGroups + '<gr><id>' + CAST([KPiGroupId] AS NVARCHAR(10)) + '</id><w>' + CAST([Weight] AS NVARCHAR(10)) + '</w><s>' + CAST([IsSpreadKPI] AS NVARCHAR) + '</s></gr>'
	FROM [dbo].[TargetTemplateKPIGroup] WITH (NOLOCK)
	WHERE [TargetTemplateId] = @ObjectId

	SELECT @Kpis = @Kpis + '<kpi><gid>' + CAST([KPIGroupId] AS NVARCHAR(10)) + '</gid><id>' + CAST([KPIId] AS NVARCHAR(10)) + '</id><w>' + CAST([Weight] AS NVARCHAR(10)) + '</w><re>' + CAST(IsRequiredEvaluate AS NVARCHAR(1)) + '</re><rc>' + CAST(IsRequiredComment AS NVARCHAR(1)) + '</rc><c>' + CAST(HasComment AS NVARCHAR(1)) + '</kpi>'
	FROM [dbo].[TargetTemplateKPI] WITH (NOLOCK)
	WHERE [TargetTemplateId] = @ObjectId

	SELECT @Data = CONCAT ('<r><id>', CAST(@ObjectId AS NVARCHAR(10)), '</id><grs>', @KpiGroups, '</grs><kpis>', @Kpis, '</kpis></r>')

	RETURN @Data
END