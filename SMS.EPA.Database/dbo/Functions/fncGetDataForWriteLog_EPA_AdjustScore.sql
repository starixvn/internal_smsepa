/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_EPA_AdjustScore    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_EPA_AdjustScore    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_EPA_AdjustScore    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_EPA_AdjustScore    Script Date: 4/6/2016 11:27:50 AM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_EPA_AdjustScore] (@EPADetails NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)	

	SELECT @Data = CONCAT('<r><eds>', @EPADetails,'</eds></r>')

	RETURN @Data
END