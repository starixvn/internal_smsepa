/****** Object:  Stored Procedure dbo.EpaAuto_UpdateFinalScoreEndEvaluation    Script Date: 7/21/2016 3:01:37 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_UpdateFinalScoreEndEvaluation    Script Date: 7/21/2016 2:45:49 PM ******/

CREATE PROCEDURE [dbo].[EpaAuto_UpdateFinalScoreEndEvaluation] @EPAScores VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Check end process.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
	@EPAScores = '<ROOT>
					<EPA EPAId='' FinalScore='' DisciplineScore='' Grade=''></EPA>
				</ROOT>'
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #EPAScore (
		[EPAId] INT
		,[FinalScore] NVARCHAR(100)
		,[DisciplineScore] FLOAT
		,[Grade] NVARCHAR(100)
		)

	DECLARE @DocHandler INT

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@EPAScores

	INSERT INTO #EPAScore (
		[EPAId]
		,[FinalScore]
		,[DisciplineScore]
		,[Grade]
		)
	SELECT [EPAId]
		,[FinalScore]
		,[DisciplineScore]
		,[Grade]
	FROM OPENXML(@DocHandler, '/ROOT/EPA', 1) WITH (EPAId INT, FinalScore NVARCHAR(100), DisciplineScore FLOAT, [Grade] NVARCHAR(100)) AS Data

	EXEC sp_xml_removedocument @DocHandler

	UPDATE [dbo].[EPA]
	SET [FinalScore] = epaS.[FinalScore]
		,[DisciplineScore] = epaS.[DisciplineScore]
		,[Grade] = epaS.[Grade]
		,[Status] = 1 -- closed
	FROM [dbo].[EPA] AS epa WITH (NOLOCK)
		INNER JOIN #EPAScore AS epaS ON epa.[EPAId] = epaS.[EPAId]
END