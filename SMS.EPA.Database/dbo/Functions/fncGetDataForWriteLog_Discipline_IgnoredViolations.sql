
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Discipline_IgnoredViolations] (@IgnoredViolations NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)

	SELECT @Data = CONCAT ('<r><vis>', @IgnoredViolations, '</vis></r>')

	RETURN @Data
END