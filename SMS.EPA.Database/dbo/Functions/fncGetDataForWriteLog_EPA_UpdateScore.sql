/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_EPA_UpdateScore    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_EPA_UpdateScore    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_EPA_UpdateScore    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_EPA_UpdateScore    Script Date: 4/6/2016 11:27:50 AM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_EPA_UpdateScore] (
				@EPAId INT
				,@ProcessId TINYINT
				,@EvaluatorId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)	

	SELECT @Data = CONCAT(
				'<r><eid>', CAST(@EPAId AS NVARCHAR(20))
				,'</eid><pid>', CAST(@ProcessId AS NVARCHAR(20))
				,'</pid><eva>', CAST(@EvaluatorId AS NVARCHAR(20))
				,'</eva><sc>', [TotalScore]
				,'</sc><c>', [Comment]
				,'</c></r>')
	FROM [dbo].[EPAProcess] WITH (NOLOCK)
	WHERE [EPAId] = @EPAId
		AND [ProcessId] = @ProcessId
		AND [EvaluatorId] = @EvaluatorId
	RETURN @Data
END