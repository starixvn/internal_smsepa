
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Period_GenerateEPA] (
	@PeriodId INT
	,@EmployeeIds VARCHAR(MAX)
	)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
		,@EPAIds NVARCHAR(MAX)

	SET @EPAIds = ''

	SELECT @EPAIds = @EPAIds + ',' + CAST([EPAId] AS NVARCHAR(20))
	FROM [dbo].[EPA] AS epa WITH (NOLOCK)
	WHERE epa.[PeriodId] = @PeriodId
		AND EXISTS (
			SELECT 1
			FROM fnc_strSplit(@EmployeeIds, ',')
			WHERE val = epa.[EmployeeId]
			)

	SELECT @EPAIds = STUFF(@EPAIds, 1, 1, '')

	SELECT @Data = CONCAT ('<r><pid>', @PeriodId, '</pid><emids>', @EmployeeIds, '</emids><eids>', @EPAIds, '</eids></r>')

	RETURN @Data
END