
CREATE PROCEDURE [dbo].[EpaAuto_UpdateIsEndProcess] @EPAProcess NVARCHAR(MAX)
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
	@EPAProcess = '<Root><Row EPAId='' ProcessId='' /></Root>'
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@DocHandler INT

	CREATE TABLE #tblEPAProcess (
		EPAId INT
		,ProcessId TINYINT
		)

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@EPAProcess

	INSERT INTO #tblEPAProcess (
		EPAId
		,ProcessId
		)
	SELECT EPAId
		,ProcessId
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (EPAId INT, ProcessId INT) AS Data

	EXEC sp_xml_removedocument @DocHandler

	UPDATE pc 
	SET pc.IsEndProcess = 1
	FROM dbo.EPAProcess AS pc WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblEPAProcess AS tblpc ON pc.EPAId = tblpc.EPAId
		AND pc.ProcessId = tblpc.ProcessId
	WHERE pc.IsEndProcess = 0
		AND pc.EndDate < @Today

	UPDATE pc
	SET pc.IsEndProcess = 1
	FROM dbo.EPAProcess AS pc WITH (ROWLOCK, UPDLOCK)
	WHERE pc.IsEndProcess = 0
		AND pc.EndDate < @Today

	DROP TABLE #tblEPAProcess
END