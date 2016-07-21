CREATE PROCEDURE [dbo].[EpaAttendanceSummary_GetByEPA] @EPAIds NVARCHAR(MAX)
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Get attendance summary of employees.
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
		,EmpId INT
		,AttendanceStartDate DATE
		,AttendanceEndDate DATE
		)

	CREATE TABLE #EmployeeDiscipline (
		EPAId INT
		,WorkDate DATE
		,EmpId INT
		,ChangeShift INT
		,MissingCheckInOut INT
		,LateIn INT
		,EarlyOut INT
		,[Absent] INT
		,UnpaidLeave INT
		,WorkingUnderTime INT
		,UnpaidLeaveAbsent INT
		,OtherViolationsScore FLOAT
		)

	CREATE TABLE #EmployeeDisciplineScore (
		EPAId INT
		,WorkDate DATE
		,EmpId INT
		,SystemSubtractScore FLOAT
		,ApproverSubtractScore FLOAT
		)

	CREATE TABLE #EmployeeDisciplineFinalScore (
		EPAId INT
		,SystemSubtractScore FLOAT
		,ApproverSubtractScore FLOAT
		,OtherViolationsScore FLOAT
		)

	INSERT INTO #EPAId
	SELECT DISTINCT val
	FROM dbo.fnc_strSplit(@EPAIds, ',')

	INSERT INTO #EPA
	SELECT DISTINCT epa.EPAId
		,epa.EmployeeId
		,period.AttendanceStartDate
		,period.AttendanceEndDate
	FROM #EPAId AS epat
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.EPAId = epa.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1

	INSERT INTO #EmployeeDiscipline
	SELECT EPAId
		,WorkDate
		,EmpId
		,SUM(ChangeShift)
		,SUM(MissingCheckInOut)
		,SUM(LateIn)
		,SUM(EarlyOut)
		,SUM([Absent])
		,SUM(UnpaidLeave)
		,SUM(WorkingUnderTime)
		,SUM(UnpaidLeaveAbsent)
		,SUM(OtherViolationsScore)
	FROM (
		SELECT epa.EPAId
			,dis.WorkDate
			,dis.EmpId
			,dis.IsChangeShift AS ChangeShift
			,dis.IsMissingCheckInOut AS MissingCheckInOut
			,dis.IsLateIn AS LateIn
			,dis.IsEarlyOut AS EarlyOut
			,dis.IsAbsent AS [Absent]
			,dis.IsUnpaidLeave AS UnpaidLeave
			,dis.IsWorkingUnderTime AS WorkingUnderTime
			,dis.IsUnpaidLeaveAbsent AS UnpaidLeaveAbsent
			,0 AS OtherViolationsScore
		FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
			INNER JOIN #EPA AS epa ON dis.EmpId = epa.EmpId
			AND dis.WorkDate BETWEEN epa.AttendanceStartDate
				AND epa.AttendanceEndDate
		WHERE dis.DisciplineTypeId > 0
		     AND ISNULL(dis.IsDeleted, 0) = 0
		
		UNION
		SELECT epa.EPAId
			,dis.WorkDate
			,epa.EmpId
			,0 AS ChangeShift
			,0 AS MissingCheckInOut
			,0 AS LateIn
			,0 AS EarlyOut
			,0 AS [Absent]
			,0 AS UnpaidLeave
			,0 AS WorkingUnderTimeTime
			,0 AS UnpaidLeaveAbsent
			,dtype.Score AS OtherViolationsScore
		FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
			INNER JOIN #EPA AS epa ON dis.EmpId = epa.EmpId
			AND dis.WorkDate BETWEEN epa.AttendanceStartDate
				AND epa.AttendanceEndDate
			INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON dis.DisciplineTypeId = dtype.DisciplineTypeId
		WHERE dis.DisciplineTypeId < 0
		     AND ISNULL(dis.IsDeleted, 0) = 0
		) AS empV
	GROUP BY EPAId
		,EmpId
		,WorkDate

	INSERT INTO #EmployeeDisciplineScore
	SELECT empD.EPAId
		,empD.WorkDate
		,empD.EmpId
		,(empD.ChangeShift * dbo.fncGetDisciplineScoreById(1)
		  + empD.MissingCheckInOut * dbo.fncGetDisciplineScoreById(2)
		  + empD.LateIn * dbo.fncGetDisciplineScoreById(3)
		  + empD.EarlyOut * dbo.fncGetDisciplineScoreById(4)
		  + empD.[Absent] * dbo.fncGetDisciplineScoreById(5)
		  + empD.UnpaidLeave * dbo.fncGetDisciplineScoreById(6)
		  + empD.WorkingUnderTime * dbo.fncGetDisciplineScoreById(7)
		  + empD.UnpaidLeaveAbsent * dbo.fncGetDisciplineScoreById(8)) AS SystemSubtractScore
		,(CASE 
			WHEN ignored.ApproverIgnoredScore IS NOT NULL THEN ignored.ApproverIgnoredScore
			ELSE 0 END
			) AS ApproverSubtractScore
	FROM #EmployeeDiscipline AS empD
		LEFT JOIN (
		SELECT dis.EmpId
			,dis.WorkDate
			,(
				(CASE 
					WHEN dis.IsChangeShift = 1
						AND dis.IsIgnoredChangeShift = 0 THEN dbo.fncGetDisciplineScoreById(1)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsMissingCheckInOut = 1
						AND dis.IsIgnoredMissingCheckInOut = 0 THEN dbo.fncGetDisciplineScoreById(2)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsLateIn = 1
						AND dis.IsIgnoredLateIn = 0 THEN dbo.fncGetDisciplineScoreById(3)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsEarlyOut = 1
						AND dis.IsIgnoredEarlyOut = 0 THEN dbo.fncGetDisciplineScoreById(4)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsAbsent = 1
						AND dis.IsIgnoredAbsent = 0 THEN dbo.fncGetDisciplineScoreById(5)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsUnpaidLeave = 1
						AND dis.IsIgnoredUnpaidLeave = 0 THEN dbo.fncGetDisciplineScoreById(6)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsWorkingUndertime = 1
						AND dis.IsIgnoredWorkingUndertime = 0 THEN dbo.fncGetDisciplineScoreById(7)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsUnpaidLeaveAbsent = 1
						AND dis.IsIgnoredUnpaidLeaveAbsent = 0 THEN dbo.fncGetDisciplineScoreById(8)
					ELSE 0 END
					)
				) AS ApproverIgnoredScore
		FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
			INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON dis.DisciplineTypeId = dtype.DisciplineTypeId
			INNER JOIN #EPA AS epat ON dis.EmpId = epat.EmpId
			AND dis.WorkDate BETWEEN epat.AttendanceStartDate
				AND epat.AttendanceEndDate
		WHERE dis.DisciplineTypeId = 1
		     AND ISNULL(dis.IsDeleted, 0) = 0
		) AS ignored ON empD.EmpId = ignored.EmpId
		AND empD.WorkDate = ignored.WorkDate
	WHERE (
			[Absent] > 0
			OR ChangeShift > 0
			OR EarlyOut > 0
			OR LateIn > 0
			OR UnpaidLeave > 0
			OR empD.WorkingUnderTime > 0
			OR UnpaidLeaveAbsent > 0
			OR OtherViolationsScore > 0
			OR MissingCheckInOut > 0
			)

	INSERT INTO #EmployeeDisciplineFinalScore
	SELECT DISTINCT empD.EPAId
		,SUM(empS.SystemSubtractScore) AS SystemSubtractScore
		,SUM(empS.ApproverSubtractScore) AS ApproverSubtractScore
		,SUM(empD.OtherViolationsScore) AS OtherViolationsScore
	FROM #EmployeeDiscipline AS empD
		LEFT JOIN #EmployeeDisciplineScore AS empS ON empD.EPAId = empS.EPAId
	GROUP BY empD.EPAId

	SELECT DISTINCT empF.EPAId
		,(CASE srt.ScoreRatingTypeId
			WHEN 1 THEN srt.Attendance - (empF.SystemSubtractScore + empF.OtherViolationsScore)
			WHEN 2 THEN srt.Attendance - (empF.OtherViolationsScore)
			ELSE 0 END
			) AS SystemDisciplineScore
		,(CASE srt.ScoreRatingTypeId
			WHEN 1 THEN (CASE 
					WHEN srt.Attendance - (empF.SystemSubtractScore + empF.OtherViolationsScore) > 0
					   THEN srt.Attendance - (empF.SystemSubtractScore + empF.OtherViolationsScore)
					ELSE 0 END
					)
			WHEN 2 THEN srt.Attendance - (empF.OtherViolationsScore)
			ELSE 0 END
			) AS DisciplineScore
	FROM dbo.#EmployeeDisciplineFinalScore AS empF
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON empF.EPAId = epa.EPAId
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
	
	UNION
	SELECT epaT.EPAId
		,srt.Attendance AS SystemDisciplineScore
		,srt.Attendance AS DisciplineScore
	FROM #EPA AS epat
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.EPAId = epa.EPAId
		INNER JOIN dbo.ScoreRatingType AS srt ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
	WHERE NOT EXISTS (
			SELECT 1
			FROM #EmployeeDisciplineFinalScore AS empF
			WHERE epat.EPAId = empF.EPAId
			)

	DROP TABLE #EPAId

	DROP TABLE #EPA

	DROP TABLE #EmployeeDiscipline

	DROP TABLE #EmployeeDisciplineScore

	DROP TABLE #EmployeeDisciplineFinalScore
END