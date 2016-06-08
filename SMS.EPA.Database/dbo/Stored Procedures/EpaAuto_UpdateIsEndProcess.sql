/****** Object:  Stored Procedure dbo.EpaAuto_UpdateIsEndProcess    Script Date: 5/16/2016 2:20:07 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_UpdateIsEndProcess    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_UpdateIsEndProcess    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_UpdateIsEndProcess    Script Date: 4/6/2016 11:27:44 AM ******/

CREATE PROCEDURE dbo.EpaAuto_UpdateIsEndProcess @EPAProcess NVARCHAR(MAX)
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
	@EPAProcess = '<ROOT><EPAProcess EPAId='' ProcessId=''></EPAProcess></ROOT>'
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @DocHandler INT

	CREATE TABLE #EPAProcessTemp (
		EPAId INT
		,ProcessId INT
		)

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@EPAProcess

	INSERT INTO #EPAProcessTemp (
		EPAId
		,ProcessId
		)
	SELECT EPAId
		,ProcessId
	FROM OPENXML(@DocHandler, '/ROOT/EPAProcess', 1) WITH (EPAId INT, ProcessId INT) AS Data

	EXEC sp_xml_removedocument @DocHandler

	UPDATE pc WITH (ROWLOCK, UPDLOCK)
	SET pc.IsEndProcess = 1
	FROM dbo.EPAProcess AS pc
		INNER JOIN #EPAProcessTemp AS pcT ON pc.EPAId = pcT.EPAId
		AND pc.ProcessId = pcT.ProcessId

	DROP TABLE #EPAProcessTemp
END