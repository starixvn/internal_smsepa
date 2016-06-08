/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Discipline_IgnoredViolations    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Discipline_IgnoredViolations    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Discipline_IgnoredViolations    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Discipline_IgnoredViolations    Script Date: 4/6/2016 11:27:49 AM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Discipline_IgnoredViolations] (@IgnoredViolations NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)		

	SELECT @Data = CONCAT('<r><vis>', @IgnoredViolations, '</vis></r>')
	RETURN @Data
END