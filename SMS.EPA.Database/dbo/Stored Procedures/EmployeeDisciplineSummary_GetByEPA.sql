
CREATE PROCEDURE [dbo].[EmployeeDisciplineSummary_GetByEPA] @EPAIds NVARCHAR(MAX)
AS
/*
Created: 20160915@Tim.Trinh
Task: Get discipline summary.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #EPAId (EPAId INT)

	CREATE TABLE #EPA (
		EPAId INT
		,EmployeeId INT
		,StartDate DATE
		,EndDate DATE
		)

	CREATE TABLE #EmployeeDisciplineScore (
		EPAId INT
		,SystemSubtractScore FLOAT
		,ApproverSubtractScore FLOAT
		,OtherDisciplines FLOAT
		)

	INSERT INTO #EPAId
	SELECT DISTINCT val
	FROM dbo.fnc_strSplit(@EPAIds, ',')

	INSERT INTO #EPA
	SELECT DISTINCT epa.EPAId
		,epa.EmployeeId
		,period.StartDate
		,period.EndDate
	FROM #EPAId AS epat
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.EPAId = epa.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1

	INSERT INTO #EmployeeDisciplineScore
	SELECT epa.EPAId
		,d.SystemSubtractScore
		,(d.ChangeShift + d.MissingCheckInOut + d.LateIn + d.EarlyOut + d.[Absent] + d.UnpaidLeave + d.WorkingUndertime + d.UnpaidLeaveAbsent) AS ApproverSubtractScore
		,d.OtherDisciplines
	FROM dbo.EmployeeDisciplineSummary AS d WITH (NOLOCK)
		INNER JOIN #EPA AS epa WITH (NOLOCK) ON d.EmployeeId = epa.EmployeeId
		AND d.Period BETWEEN epa.StartDate
			AND epa.EndDate
	WHERE d.ChangeShift > 0
		OR d.MissingCheckInOut > 0
		OR d.LateIn > 0
		OR d.EarlyOut > 0
		OR d.[Absent] > 0
		OR d.UnpaidLeave > 0
		OR d.WorkingUndertime > 0
		OR d.UnpaidLeaveAbsent > 0

	SELECT DISTINCT empF.EPAId
		,(CASE srt.ScoreRatingTypeId
			WHEN 1 THEN srt.Attendance - (d.SystemSubtractScore + d.OtherDisciplines)
			WHEN 2 THEN srt.Attendance - d.OtherDisciplines
			ELSE 0 END
			) AS SystemDisciplineScore
		,(CASE srt.ScoreRatingTypeId
			WHEN 1 THEN (CASE 
					WHEN (srt.Attendance - (d.ApproverSubtractScore + d.OtherDisciplines)) > 0 THEN (srt.Attendance - (d.ApproverSubtractScore + d.OtherDisciplines))
					ELSE 0 END
					)
			WHEN 2 THEN srt.Attendance - d.OtherDisciplines
			ELSE 0 END
			) AS DisciplineScore
	FROM #EmployeeDisciplineScore AS d
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON d.EPAId = epa.EPAId
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
	
	UNION
	SELECT epat.EPAId
		,srt.Attendance AS SystemDisciplineScore
		,srt.Attendance AS DisciplineScore
	FROM #EPA AS epat
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.EPAId = epa.EPAId
		INNER JOIN dbo.ScoreRatingType AS srt ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
	WHERE NOT EXISTS (
			SELECT 1
			FROM #EmployeeDisciplineScore AS d
			WHERE epat.EPAId = d.EPAId
			)

	DROP TABLE #EPAId

	DROP TABLE #EPA

	DROP TABLE #EmployeeDisciplineScore
END