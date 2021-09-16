
CREATE PROCEDURE [dbo].[EPA_GetDataToUpdateTotalScoreOfProcess] @PeriodId INT
	,@EmployeeIds VARCHAR(MAX) = NULL
AS
/*
Created: 2021-09-10@Tim.Trinh
Task: get data to update total score of process.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	CREATE TABLE #tblEmployee (EmployeeId INT)

	IF ISNULL(@EmployeeIds, '') <> ''
	BEGIN
		INSERT INTO #tblEmployee (EmployeeId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@EmployeeIds, ',')
	END

	SELECT DISTINCT epa.EPAId
		,period.PeriodId
		,period.TargetTypeId
		,pc.ProcessId
		,pc.TotalScore
		,pc.AdjustScore
		,(CASE 
			WHEN period.TargetTypeId = 3 --semi
				AND epa.HasMonthlyEPA = 1 THEN 1
			ELSE 0 END
			) AS HasMonthlyEPA
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId > 2
		AND pc.EndDate < @Today
		LEFT JOIN #tblEmployee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
	WHERE period.PeriodId = @PeriodId
		AND ISNULL(epa.[Status], 0) <> 2 -- rejected
		AND (
			ISNULL(@EmployeeIds, '') = ''
			OR emp.EmployeeId IS NOT NULL
			)

	DROP TABLE #tblEmployee
END