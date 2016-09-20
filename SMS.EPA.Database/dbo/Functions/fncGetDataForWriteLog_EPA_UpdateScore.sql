
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_EPA_UpdateScore] (
	@EPAId INT
	,@ProcessId TINYINT
	,@EvaluatorId INT
	)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)

	SELECT @Data = CONCAT ('<r><eid>', CAST(@EPAId AS NVARCHAR(20)), '</eid><pid>', CAST(@ProcessId AS NVARCHAR(20)), '</pid><eva>', CAST(@EvaluatorId AS NVARCHAR(20)), '</eva><sc>', [TotalScore], '</sc><c>', [Comment], '</c></r>')
	FROM [dbo].[EPAProcess] WITH (NOLOCK)
	WHERE [EPAId] = @EPAId
		AND [ProcessId] = @ProcessId
		AND [EvaluatorId] = @EvaluatorId

	RETURN @Data
END