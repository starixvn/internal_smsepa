CREATE PROCEDURE [dbo].[EPA_GetLastPerformanceScoreProcess] @EPAs IntId READONLY
AS
/*
Created: 20191220@Drake.Doi
Task: Get last perfomance score process.
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
		,[Date] DATE
		)

	DECLARE @TblLastEPAProcess TABLE (
	EPAId INT
	,ProcessId INT
	,EvaluatorId INT
	,TotalScore NVARCHAR(200)
	)

	INSERT INTO @TblLastEPA (
		LastEPAId
		,EPAId
		,EmployeeId
		,[Date]
		)
	SELECT DISTINCT 
		lastEPA.EPAId
		,epa.EPAId
		,epa.EmployeeId
		,CAST(lastPeriod.StartDate AS DATE)
	FROM EPA AS epa
		INNER JOIN @EPAs AS epat ON epa.EPAId = epat.Id
		INNER JOIN Period AS period WITH (NOLOCK)
			ON epa.PeriodId = period.PeriodId
		INNER JOIN EPA AS lastEPA WITH (NOLOCK)
			ON epa.EPAId <> lastEPA.EPAId 
			AND lastEPA.EmployeeId = epa.EmployeeId
		INNER JOIN Period AS lastPeriod WITH (NOLOCK)
			ON lastEPA.PeriodId = lastPeriod.PeriodId 
			AND lastPeriod.TargetTypeId = period.TargetTypeId
	WHERE lastEPA.[Status] = 1

	INSERT INTO @TblLastEPAProcess (EPAId, ProcessId, EvaluatorId, TotalScore)
		(
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
					,MAX([Date]) AS MaxDate
				FROM @TblLastEPA
				GROUP BY EmployeeId
				) AS tbl1 ON lastEPA.EmployeeId = tbl1.EmployeeId
				AND lastEPA.[Date] = tbl1.MaxDate
			) AS tbl ON epa.EPAId = tbl.LastEPAId
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		)
	
	SELECT pc.EPAId, pc.ProcessId, pc.EvaluatorId, pc.TotalScore
	FROM @TblLastEPAProcess pc
		INNER JOIN (
			SELECT EPAId, MAX(ProcessId) AS ProcessId
			FROM @TblLastEPAProcess
			GROUP BY EPAId
		) AS tbl ON pc.EPAId = tbl.EPAId AND pc.ProcessId = tbl.ProcessId
END