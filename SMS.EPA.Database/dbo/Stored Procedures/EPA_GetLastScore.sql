
CREATE PROCEDURE [dbo].[EPA_GetLastScore] @EPAIds IntId READONLY
AS
/*
Created: 20151006@Tim.Trinh
Task: Get last Final Score, Grade, Discipline.
DB: SPM
Original: 

Revisions:  
Param's Explanation:
	20170424@Hunter.Tran: Get last EPAId

*/
BEGIN
	SET NOCOUNT ON

	CREATE TABLE #tblEPA (
		EPAId INT
		,EmployeeId INT
		,TargetTypeId INT
		,StartDate DATE
		)

	CREATE TABLE #tblLastEPA (
		LastEPAId INT
		,EPAId INT
		,EmployeeId INT
		,Code NVARCHAR(20)
		,[Date] DATE
		,FinalScore NVARCHAR(100)
		,Grade NVARCHAR(100)
		,DisciplineScore FLOAT
		)

	INSERT INTO #tblEPA (
		EPAId
		,EmployeeId
		,TargetTypeId
		,StartDate
		)
	SELECT DISTINCT epa.EPAId
		,emp.EmployeeId
		,tt.TargetTypeId
		,pc.StartDate
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN @EPAIds AS epat ON epa.EPAId = epat.Id
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 2 -- self

	INSERT INTO #tblLastEPA (
		LastEPAId
		,EPAId
		,EmployeeId
		,Code
		,[Date]
		,FinalScore
		,Grade
		,DisciplineScore
		)
	SELECT DISTINCT epa.EPAId
		,tblEPA.EPAId
		,emp.EmployeeId
		,emp.Code
		,pc.EndDate
		,epa.FinalScore
		,epa.Grade
		,epa.DisciplineScore
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS tt ON period.TargetTypeId = tt.TargetTypeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN #tblEPA AS tblEPA WITH (NOLOCK) ON emp.EmployeeId = tblEPA.EmployeeId
		AND tt.TargetTypeId = tblEPA.TargetTypeId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
	WHERE epa.[Status] = 1 -- closed
		AND epa.EPAId NOT IN (
			SELECT Id
			FROM @EPAIds
			)
		AND EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa1 WITH (NOLOCK)
				INNER JOIN dbo.EPAProcess AS pc1 WITH (NOLOCK) ON epa1.EPAId = pc1.EPAId
			WHERE epa1.EPAId = epa.EPAId
				AND pc1.ProcessId = pc.ProcessId
				AND pc1.EvaluatorId = pc.EvaluatorId
				AND pc.EndDate IN (
					SELECT MAX(pc2.EndDate)
					FROM dbo.EPA AS epa2 WITH (NOLOCK)
						INNER JOIN dbo.EPAProcess AS pc2 WITH (NOLOCK) ON epa2.EPAId = pc2.EPAId
					WHERE epa1.EPAId = epa2.EPAId
					GROUP BY epa2.EPAId
					)
			)
	
	UNION ALL
	SELECT DISTINCT - lastEPA.Id
		,tblEPA.EPAId
		,emp.EmployeeId
		,emp.Code
		,lastEPA.PeriodDate
		,lastEPA.FinalScore
		,NULL
		,NULL
	FROM dbo.EPASummaryImport AS lastEPA WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON lastEPA.Code = emp.Code
		INNER JOIN #tblEPA AS tblEPA WITH (NOLOCK) ON emp.EmployeeId = tblEPA.EmployeeId
		AND tblEPA.TargetTypeId = 3 -- semi
		INNER JOIN (
		SELECT lastEPA1.Code
			,CAST(MAX(lastEPA1.PeriodDate) AS DATE) AS PeriodDate
		FROM dbo.EPASummaryImport AS lastEPA1 WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON lastEPA1.Code = emp.Code
			INNER JOIN #tblEPA AS tbl WITH (NOLOCK) ON emp.EmployeeId = tbl.EmployeeId
			AND tbl.TargetTypeId = 3 -- semi
		GROUP BY lastEPA1.Code
		) AS tblData ON lastEPA.Code = tblData.Code
		AND CAST(lastEPA.PeriodDate AS DATE) = tblData.PeriodDate

	SELECT DISTINCT tbl.EPAId
		,tbl1.LastEPAId
		,tbl.EmployeeId
		,tbl1.Code
		,tbl1.FinalScore
		,tbl1.Grade
		,tbl1.DisciplineScore
	FROM #tblEPA AS tbl WITH (NOLOCK)
		INNER JOIN (
		SELECT LastEPAId
			,EPAId
			,EmployeeId
			,Code
			,[Date]
			,FinalScore
			,Grade
			,DisciplineScore
		FROM #tblLastEPA AS lastepa WITH (NOLOCK)
		WHERE [Date] = (
				SELECT TOP 1 MAX([Date])
				FROM #tblLastEPA WITH (NOLOCK)
				WHERE EmployeeId = lastepa.EmployeeId
					AND EPAId = lastepa.EPAId
				GROUP BY EPAId
					,EmployeeId
				)
		) AS tbl1 ON tbl.EmployeeId = tbl1.EmployeeId
		AND tbl.EPAId = tbl1.EPAId

	DROP TABLE #tblEPA

	DROP TABLE #tblLastEPA
END