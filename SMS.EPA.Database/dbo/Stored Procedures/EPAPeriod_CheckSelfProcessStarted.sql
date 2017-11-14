
CREATE PROCEDURE [dbo].[EPAPeriod_CheckSelfProcessStarted] @PeriodId INT
	,@EmployeeIds VARCHAR(MAX)
AS
/*
Created: 20150924@Josh.Phan
Task: Check that self process started or not.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblEmployee (EmployeeId INT)

	IF ISNULL(@EmployeeIds, '') <> ''
	BEGIN
		INSERT INTO #tblEmployee (EmployeeId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@EmployeeIds, ',')

		IF EXISTS (
				SELECT 1
				FROM dbo.PeriodFlowSetting AS pe WITH (NOLOCK)
					INNER JOIN #tblEmployee AS emp WITH (NOLOCK) ON pe.EmployeeId = emp.EmployeeId
					AND pe.PeriodId = @PeriodId
					INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
					AND pe.EmployeeId = epa.EmployeeId
				WHERE pe.ProcessId = 2
					AND ISNULL(pe.StartDate, '') <> ''
					AND pe.StartDate <= CAST(GETDATE() AS DATE)
				)
		BEGIN
			SELECT CAST(1 AS BIT)
		END
		ELSE
		BEGIN
			SELECT CAST(0 AS BIT)
		END
	END

	DROP TABLE #tblEmployee
END