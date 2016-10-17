
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_EPA_UpdateFinalScore] (@EPAIds NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
		,@EPAId IntId

	INSERT INTO @EPAId
	SELECT DISTINCT val
	FROM dbo.fnc_strSplit(@EPAIds, ',')

	SELECT @Data = CONCAT (
	     '<r><eid>', CAST(EPAId AS NVARCHAR(20)),
	     '</eid><fs>', epa.FinalScore,
	     '</fs><ds>', CAST(epa.DisciplineScore AS NVARCHAR(20)),
	     '</ds><g>', epa.Grade, '</g><ofs>', epa.OldFinalScore,
	     '</ofs><ods>', CAST(epa.OldDisciplineScore AS NVARCHAR(20)),
	     '</ods><og>', epa.OldGrade, '</og></r>')
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN @EPAId AS tbl ON epa.EPAId = tbl.Id

	RETURN @Data
END