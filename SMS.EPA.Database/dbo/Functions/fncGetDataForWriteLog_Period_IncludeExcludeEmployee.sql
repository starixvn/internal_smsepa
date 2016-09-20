
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Period_IncludeExcludeEmployee] (
	@PeriodId INT
	,@EmployeeId INT
	)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
		,@EPA NVARCHAR(100)

	SET @EPA = ''

	SELECT @EPA = @EPA + '<epa><eid>' + CAST([EPAId] AS NVARCHAR(10)) + '</eid><s>' + CAST([Status] AS NVARCHAR) + '</s></epa>'
	FROM [dbo].[EPA] WITH (NOLOCK)
	WHERE [PeriodId] = @PeriodId
		AND [EmployeeId] = @EmployeeId

	SELECT @Data = CONCAT ('<r><eid>', @EmployeeId, '</eid><pid>', @PeriodId, '</pid>', @EPA, '</r>')

	RETURN @Data
END