
CREATE PROCEDURE [dbo].[EPA_GetLastScoreProcess] @EPAs IntId READONLY
	,@Processes ProcessTable READONLY
AS
/*
Created: 20151006@Tim.Trinh
Task: Get last Total.
DB: SPM
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TblLastEPA TABLE (
		LastEPAId INT
		,EPAId INT
		,EmployeeId INT
		,Code NVARCHAR(20)
		,[Date] DATE
		)

	INSERT INTO @TblLastEPA
	SELECT DISTINCT epa.EPAId
		,epat.Id
		,emp.EmployeeId
		,emp.Code
		,CAST(period.StartDate AS DATE)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN @EPAs AS epat ON epa.EPAId <> epat.Id
		INNER JOIN @Processes AS pct ON pc.ProcessId = pct.ProcessId
		AND pc.EvaluatorId = pct.EvaluatorId
	WHERE epa.[Status] = 1 -- closed
		AND epa.EmployeeId IN (
			SELECT DISTINCT epa1.EmployeeId
			FROM dbo.EPA AS epa1 WITH (NOLOCK)
				INNER JOIN @EPAs AS epa2 ON epa1.EPAId = epa2.Id
			)

	SELECT DISTINCT tbl.EPAId
		,pc.ProcessId
		,pc.EvaluatorId
		,pc.TotalScore
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN (
		SELECT DISTINCT lastEPA.LastEPAId
			,lastEPA.EPAId
		FROM @TblLastEPA AS lastEPA
			INNER JOIN (
			SELECT EmployeeId
				,Code
				,MAX([Date]) AS MaxDate
			FROM @TblLastEPA
			GROUP BY EmployeeId
				,Code
			) AS tbl1 ON lastEPA.EmployeeId = tbl1.EmployeeId
			AND lastEPA.[Date] = tbl1.MaxDate
		) AS tbl ON epa.EPAId = tbl.LastEPAId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN @Processes AS pct ON pc.ProcessId = pct.ProcessId
		AND pc.EvaluatorId = pct.EvaluatorId
END