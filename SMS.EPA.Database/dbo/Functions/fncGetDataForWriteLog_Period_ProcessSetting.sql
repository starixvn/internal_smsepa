
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Period_ProcessSetting] (@PeriodId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
		,@Processes NVARCHAR(MAX)

	SET @Processes = ''

	SELECT @Processes = @Processes + '<pr><id>' + CAST([ProcessId] AS NVARCHAR(10)) + '</id><du>' + CAST([Duration] AS NVARCHAR(5)) + '</du><nab>' + CAST([NoticeAtBegin] AS NVARCHAR) + '</nab><rd>' + CAST([RemindDay] AS NVARCHAR(5)) + '</rd><pr>'
	FROM [dbo].[PeriodProcessSetting] WITH (NOLOCK)
	WHERE [PeriodId] = @PeriodId

	SELECT @Data = CONCAT ('<r><pid>', @PeriodId, '</pid><prs>', @Processes, '</prs></r>')

	RETURN @Data
END