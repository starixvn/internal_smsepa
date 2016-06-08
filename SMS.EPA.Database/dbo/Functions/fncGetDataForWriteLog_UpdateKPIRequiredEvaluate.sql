/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_UpdateKPIRequiredEvaluate    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_UpdateKPIRequiredEvaluate    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_UpdateKPIRequiredEvaluate    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_UpdateKPIRequiredEvaluate    Script Date: 4/6/2016 11:27:50 AM ******/
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_UpdateKPIRequiredEvaluate]
(
	@ObjectId INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @Data NVARCHAR(MAX)
	
	SELECT @Data = CONCAT(
					'<r><id>', @ObjectId
					,'</id><re>', [IsRequiredEvaluate]
					,'</re><ln>', [LogNo]
					,'</ln></r>')
		FROM [dbo].[KPI] WITH (NOLOCK)
		WHERE [KPIId] = @ObjectId

	RETURN @Data
END