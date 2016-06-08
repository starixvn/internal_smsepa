/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_IncludeExcludeEmployee    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_IncludeExcludeEmployee    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_IncludeExcludeEmployee    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_IncludeExcludeEmployee    Script Date: 4/6/2016 11:27:50 AM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Period_IncludeExcludeEmployee] (
			@PeriodId INT
			,@EmployeeId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)	
	DECLARE @EPA NVARCHAR(100)

	SET @EPA = ''

	SELECT @EPA = @EPA 
				+ '<epa><eid>' + CAST([EPAId] AS NVARCHAR(10))
				+ '</eid><s>' + CAST([Status] AS NVARCHAR)
				+ '</s></epa>'
	FROM [dbo].[EPA] WITH (NOLOCK)
	WHERE [PeriodId] = @PeriodId
		AND [EmployeeId] = @EmployeeId

	SELECT @Data = CONCAT(
				'<r><eid>', @EmployeeId
				,'</eid><pid>', @PeriodId
				,'</pid>', @EPA
				, '</r>')

	RETURN @Data
END