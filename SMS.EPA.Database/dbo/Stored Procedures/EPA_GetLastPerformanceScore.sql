
CREATE PROCEDURE [dbo].[EPA_GetLastPerformanceScore] @EPAId INT
AS
/*
Created: 20151110@Tim.Trinh
Task: Get last perfomance score.
DB: SMSEPA
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @TargetTypeId INT

	SELECT @TargetTypeId = tt.TargetTypeId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
	WHERE epa.EPAId = @EPAId

	CREATE TABLE #TblLastEPA (
		LastEPAId INT
		,EPAId INT
		,EmployeeId INT
		,Code NVARCHAR(20)
		,[Date] DATE
		,MaxProcessId INT
		)

	INSERT INTO #TblLastEPA (
		LastEPAId
		,EPAId
		,EmployeeId
		,Code
		,[Date]
		,MaxProcessId
		)
	SELECT DISTINCT epa.EPAId
		,@EPAId
		,emp.EmployeeId
		,emp.Code
		,MAX(pc.StartDate)
		,MAX(pc.ProcessId)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS period ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS ttype WITH (NOLOCK) ON period.TargetTypeId = ttype.TargetTypeId
		AND ttype.TargetTypeId = @TargetTypeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
	WHERE epa.[Status] = 1 -- closed
		AND epa.EPAId <> @EPAId
		AND epa.EmployeeId IN (
			SELECT DISTINCT epa1.EmployeeId
			FROM dbo.EPA AS epa1 WITH (NOLOCK)
			WHERE epa1.EPAId = @EPAId
			)
	GROUP BY epa.EPAId
		,emp.EmployeeId
		,emp.Code

	SELECT DISTINCT tbl.EPAId
		,pc.ProcessId
		,pc.TotalScore
		,emp.EmployeeId AS EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pc.EvaluatorId = emp.EmployeeId
		INNER JOIN (
		SELECT DISTINCT lastEPA.LastEPAId
			,lastEPA.EPAId
			,lastEPA.MaxProcessId
		FROM #TblLastEPA AS lastEPA
			INNER JOIN (
			SELECT EmployeeId
				,Code
				,MAX([Date]) AS MaxDate
				,MaxProcessId
			FROM #TblLastEPA
			GROUP BY EmployeeId
				,Code
				,MaxProcessId
			) AS tbl1 ON lastEPA.EmployeeId = tbl1.EmployeeId
			AND lastEPA.[Date] = tbl1.MaxDate
			AND lastEPA.MaxProcessId = tbl1.MaxProcessId
		) AS tbl ON epa.EPAId = tbl.LastEPAId
		AND pc.ProcessId = tbl.MaxProcessId
	
	DROP TABLE #TblLastEPA
END