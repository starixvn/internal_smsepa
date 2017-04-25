/****** Object:  Stored Procedure dbo.EPA_GetLastScore_xpre1    Script Date: 5/16/2016 2:15:30 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetLastScore_xpre1    Script Date: 5/16/2016 1:59:44 PM ******/

CREATE PROCEDURE [dbo].[EPA_GetLastScore_xpre1] @EPAIds IntId READONLY
AS
/*
Created: 20151006@Tim.Trinh
Task: Get last Final Score, Grade, Discipline.
DB: SPM
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TblEPA TABLE (
		EPAId INT
		,EmployeeId INT
		,TargetTypeId INT
		,StartDate DATE
		)
	DECLARE @TblLastEPA TABLE (
		LastEPAId INT
		,EPAId INT
		,EmployeeId INT
		,Code NVARCHAR(20)
		,[Date] DATE
		,FinalScore NVARCHAR(100)
		,Grade NVARCHAR(100)
		,DisciplineScore FLOAT
		)

	INSERT INTO @TblEPA
	SELECT DISTINCT epa.EPAId
		,emp.EmployeeId
		,tt.TargetTypeId
		,CAST(period.StartDate AS DATE)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN @EPAIds AS epat ON epa.EPAId = epat.Id
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId

	INSERT INTO @TblLastEPA
	SELECT DISTINCT epa.EPAId
		,tblEPA.EPAId
		,emp.EmployeeId
		,emp.Code
		,CAST(period.StartDate AS DATE)
		,epa.FinalScore
		,epa.Grade
		,epa.DisciplineScore
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.TargetType AS tt ON period.TargetTypeId = tt.TargetTypeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN @TblEPA AS tblEPA ON emp.EmployeeId = tblEPA.EmployeeId
		AND tt.TargetTypeId = tblEPA.TargetTypeId
		AND CAST(period.StartDate AS DATE) < tblEPA.StartDate
	WHERE epa.[Status] = 1 -- closed
		AND epa.EPAId NOT IN (
			SELECT Id
			FROM @EPAIds
			)
	
	UNION ALL
	SELECT DISTINCT - lastEPA.Id
		,tblEPA.EPAId
		,emp.EmployeeId
		,emp.Code
		,CAST(lastEPA.PeriodDate AS DATE)
		,lastEPA.FinalScore
		,NULL
		,NULL
	FROM dbo.EPASummaryImport AS lastEPA WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON lastEPA.Code = emp.Code
		INNER JOIN @TblEPA AS tblEPA ON emp.EmployeeId = tblEPA.EmployeeId

	SELECT DISTINCT tbl.EPAId
		,tbl.EmployeeId
		,tbl.Code
		,tbl.FinalScore
		,tbl.Grade
		,tbl.DisciplineScore
	FROM @TblLastEPA AS tbl
		INNER JOIN (
		SELECT EmployeeId
			,Code
			,MAX([Date]) AS MaxDate
		FROM @TblLastEPA
		GROUP BY EmployeeId
			,Code
		) AS tbl1 ON tbl.EmployeeId = tbl1.EmployeeId
		AND tbl.[Date] = tbl1.MaxDate
END