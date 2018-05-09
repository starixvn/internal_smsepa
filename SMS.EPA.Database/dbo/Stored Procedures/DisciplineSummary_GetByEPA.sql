
CREATE PROCEDURE [dbo].[DisciplineSummary_GetByEPA] @EPAIds NVARCHAR(MAX)
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

	DECLARE @ProbationTarget TINYINT = 1
		,@SemiTarget TINYINT = 3
		,@PromotionTarget TINYINT = 5
		,@Discipline TINYINT = 1
		,@NonDicipline TINYINT = 2

	CREATE TABLE #tblEPAId (EPAId INT)

	CREATE TABLE #tblEPA (
		EPAId INT
		,EmployeeId INT
		,StartDate DATE
		,EndDate DATE
		)

	CREATE TABLE #tblEmployeeDisciplineScore (
		EPAId INT
		,SystemSubtractScore FLOAT
		,ApproverSubtractScore FLOAT
		,OtherDisciplines FLOAT
		)

	INSERT INTO #tblEPAId (EPAId)
	SELECT DISTINCT val
	FROM dbo.fnc_strSplit(@EPAIds, ',')

	INSERT INTO #tblEPA (
		EPAId
		,EmployeeId
		,StartDate
		,EndDate
		)
	SELECT DISTINCT epa.EPAId
		,epa.EmployeeId
		,period.AttendanceStartDate
		,period.AttendanceEndDate
	FROM #tblEPAId AS epat WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.EPAId = epa.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1
	WHERE period.TargetTypeId = @SemiTarget
	
	UNION
	SELECT DISTINCT epa.EPAId
		,epa.EmployeeId
		,ds.StartDate
		,ds.EndDate
	FROM #tblEPAId AS epat WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.EPAId = epa.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1
		INNER JOIN dbo.PeriodDisciplineSetting AS ds WITH (NOLOCK) ON epa.PeriodId = ds.PeriodId
		AND epa.EmployeeId = ds.EmployeeId
	WHERE period.TargetTypeId IN (@ProbationTarget, @PromotionTarget)

	INSERT INTO #tblEmployeeDisciplineScore (
		EPAId
		,SystemSubtractScore
		,ApproverSubtractScore
		,OtherDisciplines
		)
	SELECT epa.EPAId
		,SUM(d.SystemSubtractScore)
		,SUM(d.ChangeShift) + SUM(d.MissingCheckInOut) + SUM(d.LateIn) + SUM(d.EarlyOut) + SUM(d.[Absent]) + SUM(d.UnpaidLeave) + SUM(d.WorkingUndertime) + SUM(d.UnpaidLeaveAbsent) + SUM(ISNULL(d.UnplannedLeave, 0)) + SUM(ISNULL(d.WeekendLeave, 0)) + SUM(ISNULL(SevenDaysLeave, 0))
		,SUM(d.OtherDisciplines)
	FROM dbo.DisciplineSummary AS d WITH (NOLOCK)
		INNER JOIN #tblEPA AS epa WITH (NOLOCK) ON d.EmployeeId = epa.EmployeeId
		AND d.WorkDate BETWEEN epa.StartDate
			AND epa.EndDate
	WHERE d.IsDeleted = 0
	GROUP BY epa.EPAId

	SELECT DISTINCT d.EPAId
		,(CASE srt.ScoreRatingTypeId
			WHEN @Discipline THEN srt.Attendance - (d.SystemSubtractScore + d.OtherDisciplines)
			WHEN @NonDicipline THEN srt.Attendance - d.OtherDisciplines
			ELSE 0 END
			) AS SystemDisciplineScore
		,(CASE srt.ScoreRatingTypeId
			WHEN @Discipline THEN (CASE 
					WHEN (srt.Attendance - (d.ApproverSubtractScore + d.OtherDisciplines)) > 0 THEN (srt.Attendance - (d.ApproverSubtractScore + d.OtherDisciplines))
					ELSE 0 END
					)
			WHEN @NonDicipline THEN srt.Attendance - d.OtherDisciplines
			ELSE 0 END
			) AS DisciplineScore
		,d.OtherDisciplines
	FROM #tblEmployeeDisciplineScore AS d WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON d.EPAId = epa.EPAId
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
	
	UNION
	SELECT epat.EPAId
		,srt.Attendance AS SystemDisciplineScore
		,srt.Attendance AS DisciplineScore
		,0 AS OtherDisciplines
	FROM #tblEPA AS epat WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.EPAId = epa.EPAId
		INNER JOIN dbo.ScoreRatingType AS srt ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
	WHERE NOT EXISTS (
			SELECT 1
			FROM #tblEmployeeDisciplineScore AS d WITH (NOLOCK)
			WHERE epat.EPAId = d.EPAId
			)

	DROP TABLE #tblEPAId

	DROP TABLE #tblEPA

	DROP TABLE #tblEmployeeDisciplineScore
END