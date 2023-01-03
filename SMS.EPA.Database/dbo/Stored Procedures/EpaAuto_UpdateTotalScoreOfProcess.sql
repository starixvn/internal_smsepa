
CREATE PROCEDURE [dbo].[EpaAuto_UpdateTotalScoreOfProcess] @DataXml NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2021-09-10@Tim.Trinh
Task: Update total score.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
	@DataXml = '<Root><Row EPAId='' ProcessId='' EvaluatorId='' TotalScore='' /></Root>'
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @DocHandler INT

	CREATE TABLE #tblDetail (
		EPAId INT
		,ProcessId TINYINT
		,EvaluatorId INT
		,TotalScore VARCHAR(100)
		)

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@DataXml

	INSERT INTO #tblDetail (
		EPAId
		,ProcessId
		,EvaluatorId
		,TotalScore
		)
	SELECT EPAId
		,ProcessId
		,EvaluatorId
		,TotalScore
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (EPAId INT, ProcessId INT, EvaluatorId INT, TotalScore VARCHAR(100)) AS Data

	EXEC sp_xml_removedocument @DocHandler

	UPDATE pc
	SET pc.TotalScore = tbl.TotalScore
	FROM dbo.EPAProcess AS pc WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblDetail AS tbl WITH (NOLOCK) ON pc.EPAId = tbl.EPAId
		AND pc.ProcessId = tbl.ProcessId
		AND pc.EvaluatorId = tbl.EvaluatorId

	DROP TABLE #tblDetail
END