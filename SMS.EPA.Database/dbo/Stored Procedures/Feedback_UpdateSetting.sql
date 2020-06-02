
CREATE PROCEDURE [dbo].[Feedback_UpdateSetting] @PeriodId INT
	,@EmployeeIds VARCHAR(MAX)
	,@FromDate DATE
	,@ToDate DATE
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200602@Tim.Trinh
Task: update feedback setting
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblEmployeeId (EmployeeId INT)

	INSERT INTO #tblEmployeeId (EmployeeId)
	SELECT val
	FROM dbo.fnc_strSplit(@EmployeeIds, ',')

	UPDATE fb
	SET fb.FromDate = @FromDate
		,fb.ToDate = @ToDate
	FROM dbo.Feedback AS fb WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblEmployeeId AS emp WITH (NOLOCK) ON fb.EmployeeId = emp.EmployeeId
	WHERE fb.PeriodId = @PeriodId

	DROP TABLE #tblEmployeeId
END
