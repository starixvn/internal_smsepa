
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_UpdateKPIRequiredEvaluate] (@ObjectId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)

	SELECT @Data = CONCAT ('<r><id>', @ObjectId, '</id><re>', [IsRequiredEvaluate], '</re><ln>', [LogNo], '</ln></r>')
	FROM [dbo].[KPI] WITH (NOLOCK)
	WHERE [KPIId] = @ObjectId

	RETURN @Data
END