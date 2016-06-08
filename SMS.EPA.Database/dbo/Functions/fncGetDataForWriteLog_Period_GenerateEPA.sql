/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_GenerateEPA    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_GenerateEPA    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_GenerateEPA    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_GenerateEPA    Script Date: 4/6/2016 11:27:50 AM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Period_GenerateEPA] (
			@PeriodId INT
			,@EmployeeIds VARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)	
	DECLARE @EPAIds NVARCHAR(MAX)

	SET @EPAIds = ''

	SELECT @EPAIds = @EPAIds + ',' + CAST([EPAId] AS NVARCHAR(20))
	FROM [dbo].[EPA] AS epa WITH (NOLOCK)
	WHERE  epa.[PeriodId] = @PeriodId
	AND EXISTS (
		SELECT 1
		FROM fnc_strSplit(@EmployeeIds, ',')
		WHERE val = epa.[EmployeeId]
	)	

	SELECT @EPAIds = STUFF(@EPAIds, 1, 1, '')

	SELECT @Data = CONCAT('<r><pid>', @PeriodId,'</pid><emids>', @EmployeeIds, '</emids><eids>', @EPAIds, '</eids></r>')

	RETURN @Data
END