/****** Object:  Stored Procedure dbo.EPA_GetLastPerformanceScore    Script Date: 5/16/2016 2:20:07 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetLastPerformanceScore    Script Date: 5/16/2016 2:15:30 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetLastPerformanceScore    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetLastPerformanceScore    Script Date: 4/6/2016 11:27:41 AM ******/

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
	SET NOCOUNT ON;

	DECLARE @TblLastEPA TABLE (
		LastEPAId INT
		,EPAId INT
		,EmployeeId INT
		,Code NVARCHAR(20)
		,[Date] DATE
		,MaxProcessId INT
		)

	INSERT INTO @TblLastEPA
	SELECT DISTINCT epa.EPAId
		,@EPAId
		,emp.EmployeeId
		,emp.Code
		,CAST(period.StartDate AS DATE)
		,MAX(pc.ProcessId)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS period ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS ttype WITH (NOLOCK) ON period.TargetTypeId = ttype.TargetTypeId
		INNER JOIN (
		SELECT DISTINCT tt1.TargetTypeId
		FROM dbo.EPA AS epa1 WITH (NOLOCK)
			INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa1.PeriodId = period1.PeriodId
			INNER JOIN dbo.TargetType AS tt1 WITH (NOLOCK) ON period1.TargetTypeId = tt1.TargetTypeId
		WHERE epa1.EPAId = @EPAId
		) AS ttype1 ON ttype.TargetTypeId = ttype1.TargetTypeId
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
		,period.StartDate

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
		FROM @TblLastEPA AS lastEPA
			INNER JOIN (
			SELECT EmployeeId
				,Code
				,MAX([Date]) AS MaxDate
				,MaxProcessId
			FROM @TblLastEPA
			GROUP BY EmployeeId
				,Code
				,MaxProcessId
			) AS tbl1 ON lastEPA.EmployeeId = tbl1.EmployeeId
			AND lastEPA.[Date] = tbl1.MaxDate
			AND lastEPA.MaxProcessId = tbl1.MaxProcessId
		) AS tbl ON epa.EPAId = tbl.LastEPAId
		AND pc.ProcessId = tbl.MaxProcessId
END