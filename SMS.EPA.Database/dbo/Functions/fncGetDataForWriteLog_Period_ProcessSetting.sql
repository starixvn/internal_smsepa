/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_ProcessSetting    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_ProcessSetting    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_ProcessSetting    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_ProcessSetting    Script Date: 4/6/2016 11:27:50 AM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Period_ProcessSetting] (@PeriodId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)	
	DECLARE @Processes NVARCHAR(MAX)

	SET @Processes = ''

	SELECT @Processes = @Processes 
						+ '<pr><id>' + CAST([ProcessId] AS NVARCHAR(10)) 
						+ '</id><du>' + CAST([Duration] AS NVARCHAR(5)) 
						+ '</du><nab>' + CAST([NoticeAtBegin] AS NVARCHAR)
						+ '</nab><rd>' + CAST([RemindDay] AS NVARCHAR(5))
						+ '</rd><pr>'
	FROM [dbo].[PeriodProcessSetting] WITH (NOLOCK)
	WHERE [PeriodId] = @PeriodId

	SELECT @Data = CONCAT('<r><pid>', @PeriodId, '</pid><prs>', @Processes, '</prs></r>')

	RETURN @Data
END