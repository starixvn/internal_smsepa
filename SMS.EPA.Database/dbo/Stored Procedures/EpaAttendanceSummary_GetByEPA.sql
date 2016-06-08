/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_GetByEPA    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_GetByEPA    Script Date: 5/16/2016 2:15:32 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_GetByEPA    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_GetByEPA    Script Date: 4/6/2016 11:27:42 AM ******/

CREATE PROCEDURE [dbo].[EpaAttendanceSummary_GetByEPA] @EPAIds NVARCHAR(MAX)
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Get attendance summary of employees.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #EPATemp (
		[EPAId] INT
		,[EmpId] INT
		,[AttendanceStartDate] DATETIME
		,[AttendanceEndDate] DATETIME
		,[AttendanceSettingScore] FLOAT
		)

	INSERT INTO #EPATemp (
		[EPAId]
		,[EmpId]
		,[AttendanceStartDate]
		,[AttendanceEndDate]
		,[AttendanceSettingScore]
		)
	SELECT val
		,(
			SELECT [EmployeeId]
			FROM [dbo].[EPA] AS epa WITH (NOLOCK)
			WHERE epa.[EPAId] = val
			)
		,(
			SELECT pe.[AttendanceStartDate]
			FROM [dbo].[EPA] AS epa WITH (NOLOCK)
				INNER JOIN [dbo].[Period] AS pe WITH (NOLOCK) ON epa.[PeriodId] = pe.[PeriodId]
			WHERE epa.[EPAId] = val
			)
		,(
			SELECT pe.[AttendanceEndDate]
			FROM [dbo].[EPA] AS epa WITH (NOLOCK)
				INNER JOIN [dbo].[Period] AS pe WITH (NOLOCK) ON epa.[PeriodId] = pe.[PeriodId]
			WHERE epa.[EPAId] = val
			)
		,(
			SELECT [Attendance]
			FROM [dbo].[ScoreRatingType] AS srt WITH (NOLOCK)
			WHERE EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[EPA] WITH (NOLOCK)
					WHERE [EPAId] = val
						AND [ScoreRatingTypeId] = srt.[ScoreRatingTypeId]
					)
			)
	FROM fnc_strSplit(@EPAIds, ',')

	CREATE TABLE #EmployeeViolation (
		[EPAId] INT
		,[WorkDate] DATETIME
		,[EmpId] INT
		,[MissingCheckInOut] INT
		,[LateIn] INT
		,[EarlyOut] INT
		,[Absent] INT
		,[WorkingBelowTime] INT
		,[UnpaidLeave] INT
		,[ChangeShift] INT
		,[UnpaidLeaveAbsent] INT
		,[OtherViolationsScore] FLOAT
		,[AttendanceStartDate] DATETIME
		,[AttendanceEndDate] DATETIME
		)

	INSERT INTO #EmployeeViolation (
		[EPAId]
		,[WorkDate]
		,[EmpId]
		,[MissingCheckInOut]
		,[LateIn]
		,[EarlyOut]
		,[Absent]
		,[WorkingBelowTime]
		,[UnpaidLeave]
		,[ChangeShift]
		,[UnpaidLeaveAbsent]
		,[OtherViolationsScore]
		,[AttendanceStartDate]
		,[AttendanceEndDate]
		)
	SELECT [EPAId]
		,[WorkDate]
		,[EmpId]
		,SUM([MissingCheckInOut])
		,SUM([LateIn])
		,SUM([EarlyOut])
		,SUM([Absent])
		,SUM([WorkingBelowTime])
		,SUM([UnpaidLeave])
		,SUM([ChangeShift])
		,SUM([UnpaidLeaveAbsent])
		,SUM([OtherViolationsScore])
		,[AttendanceStartDate]
		,[AttendanceEndDate]
	FROM (
		SELECT epaT.[EPAId]
			,[WorkDate]
			,empA.[EmpId]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 2 -- Missing Check In/Out
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empA.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [MissingCheckInOutAutoIgnore] <> 1 THEN [MissingCheckInOut]
						ELSE 0
						END
				ELSE 0 END
				) AS [MissingCheckInOut]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 3 -- Late In
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND empA.[EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [LateInAutoIgnore] <> 1 THEN [LateIn]
						ELSE 0
						END
				ELSE 0 END
				) AS [LateIn]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 4 -- Early Out
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND empA.[EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [EarlyOutAutoIgnore] <> 1 THEN [EarlyOut]
						ELSE 0
						END
				ELSE 0 END
				) AS [EarlyOut]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 5 -- Absent
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND empA.[EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [AbsentAutoIgnore] <> 1 THEN [Absent]
						ELSE 0
						END
				ELSE 0 END
				) AS [Absent]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 7 -- Working under time
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND empA.[EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [WorkingBelowTimeAutoIgnore] <> 1 THEN [WorkingBelowTime]
						ELSE 0
						END
				ELSE 0 END
				) AS [WorkingBelowTime]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 6 -- Unpaid Leave
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND empA.[EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [UnpaidLeaveAutoIgnore] <> 1 
						THEN CASE WHEN [UnpaidLeave] = 0.5 THEN 1 ELSE [UnpaidLeave] END
						ELSE 0
						END
				ELSE 0 END
				) AS [UnpaidLeave]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 1 -- Change Shift
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND empA.[EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [ChangeShiftAutoIgnore] <> 1 THEN [ChangeShift]
						ELSE 0
						END
				ELSE 0 END
				) AS [ChangeShift]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 8 -- Unpaid Leave Absent
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND empA.[EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [UnpaidLeaveAbsentAutoIgnore] <> 1 
						THEN CASE WHEN [UnpaidLeaveAbsent] = 0.5 THEN 1 ELSE [UnpaidLeaveAbsent] END
						ELSE 0
						END
				ELSE 0 END
				) AS [UnpaidLeaveAbsent]
			,0 AS OtherViolationsScore
			,'' AS OtherViolationsName
			,epaT.[AttendanceStartDate]
			,epaT.[AttendanceEndDate]
		FROM [dbo].[HRMEmployeeAttendanceViolation] AS empA WITH (NOLOCK)
			INNER JOIN #EPATemp AS epaT ON empA.[EmpId] = epaT.[EmpId]
			AND CAST(empA.[WorkDate] AS DATE) >= CAST(epaT.[AttendanceStartDate] AS DATE)
			AND CAST(empA.[WorkDate] AS DATE) <= CAST(epaT.[AttendanceEndDate] AS DATE)
		
		UNION
		SELECT epaT.[EPAId]
			,[WorkDate]
			,epaT.[EmpId]
			,0 AS MissingCheckInOut
			,0 AS LateIn
			,0 AS EarlyOut
			,0 AS [Absent]
			,0 AS WorkingBelowTime
			,0 AS UnpaidLeave
			,0 AS ChangeShift
			,0 AS UnpaidLeaveAbsent
			,[Score] AS OtherViolationsScore
			,[Name] AS OtherViolationsName
			,epaT.[AttendanceStartDate]
			,epaT.[AttendanceEndDate]
		FROM [dbo].[EmployeeOtherViolation] AS empO WITH (NOLOCK)
			INNER JOIN #EPATemp AS epaT ON empO.[EmpId] = epaT.[EmpId]
			AND CAST(empO.[WorkDate] AS DATE) >= CAST(epaT.[AttendanceStartDate] AS DATE)
			AND CAST(empO.[WorkDate] AS DATE) <= CAST(epaT.[AttendanceEndDate] AS DATE)
		) AS empV
	GROUP BY [EPAId]
		,[EmpId]
		,[WorkDate]
		,[AttendanceStartDate]
		,[AttendanceEndDate]

	CREATE TABLE #EmployeeViolationSystemScore (
		[EPAId] INT
		,[WorkDate] DATETIME
		,[EmpId] INT
		,[SystemSubtractScore] FLOAT
		,[ApproverIgnoredScore] FLOAT
		,[OtherViolationsScore] FLOAT
		)

	INSERT INTO #EmployeeViolationSystemScore (
		[EPAId]
		,[WorkDate]
		,[EmpId]
		,[SystemSubtractScore]
		,[ApproverIgnoredScore]
		,[OtherViolationsScore]
		)
	SELECT [EPAId]
		,[WorkDate]
		,[EmpId]
		,(
			[MissingCheckInOut] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 2 -- Missing Check In/Out
				) + empV.[LateIn] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 3 -- Late In
				) + empV.[EarlyOut] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 4 -- Early Out
				) + empV.[Absent] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 5 -- Absent
				) + empV.[WorkingBelowTime] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 7 -- Working below 7H
				) + empV.[UnpaidLeave] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 6 -- Unpaid Leave
				) + empV.[ChangeShift] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 1 -- Change Shift
				) + empV.[UnpaidLeaveAbsent] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 8 -- Unpaid Leave Absent
				)
			) AS SystemSubtractScore
		,(
			SELECT SUM(attV.[Score])
			FROM [dbo].[AttendanceDetailIgnored] AS attI WITH (NOLOCK)
				INNER JOIN [dbo].[AttendanceViolation] AS attV WITH (NOLOCK) ON attI.[ViolationId] = attV.[Id]
			WHERE attI.[EmployeeId] = empV.[EmpId]
				AND CAST(attI.[WorkDate] AS DATE) >= CAST(empV.[AttendanceStartDate] AS DATE)
				AND CAST(attI.[WorkDate] AS DATE) <= CAST(empV.[AttendanceEndDate] AS DATE)
				AND CAST(attI.[WorkDate] AS DATE) = CAST(empV.[WorkDate] AS DATE)
				AND (
					(empV.ChangeShift > 0 AND attI.ViolationId = 1)
					OR (empV.MissingCheckInOut > 0 AND attI.ViolationId = 2)
					OR (empV.LateIn > 0 AND attI.ViolationId = 3)
					OR (empV.EarlyOut > 0 AND attI.ViolationId = 4)
					OR (empV.[Absent] > 0 AND attI.ViolationId = 5)
					OR (empV.UnpaidLeave > 0 AND attI.ViolationId = 6)
					OR (empV.WorkingBelowTime > 0 AND attI.ViolationId = 7)
					OR (empV.UnpaidLeaveAbsent > 0 AND attI.ViolationId = 8)
				)
			GROUP BY attI.[EmployeeId]
			) AS ApproverIgnoredScore
		,empV.[OtherViolationsScore]
	FROM #EmployeeViolation AS empV

	CREATE TABLE #EmployeeViolationFinalScore (
		[EPAId] INT
		,[WorkDate] DATETIME
		,[EmpId] INT
		,[SystemSubtractScore] FLOAT
		,[ApproverSubtractScore] FLOAT
		,[OtherViolationsScore] FLOAT
		)

	INSERT INTO #EmployeeViolationFinalScore (
		[EPAId]
		,[WorkDate]
		,[EmpId]
		,[SystemSubtractScore]
		,[ApproverSubtractScore]
		,[OtherViolationsScore]
		)
	SELECT [EPAId]
		,[WorkDate]
		,[EmpId]
		,(CASE 
			WHEN [SystemSubtractScore] < 3.5 THEN [SystemSubtractScore]
			ELSE 3.5 END
			) AS SystemSubtractScore
		,(CASE 
			WHEN [SystemSubtractScore] - (CASE 
					WHEN ISNULL([ApproverIgnoredScore], '') <> '' THEN [ApproverIgnoredScore]
					ELSE 0 END
					) < 3.5 THEN [SystemSubtractScore] - (CASE 
					WHEN ISNULL([ApproverIgnoredScore], '') <> '' THEN [ApproverIgnoredScore]
					ELSE 0 END
					)
			ELSE 3.5 END
			) AS ApproverSubtractScore
		,[OtherViolationsScore]
	FROM #EmployeeViolationSystemScore

	SELECT empV.[EPAId]
		,CASE (
				SELECT [ScoreRatingTypeId]
				FROM [dbo].[EPA] WITH (NOLOCK)
				WHERE [EPAId] = empV.[EPAId]
				)
			WHEN 1 THEN (
					SELECT [AttendanceSettingScore]
					FROM #EPATemp
					WHERE [EPAId] = empV.[EPAId]
					) - SUM(empV.[SystemSubtractScore] + empV.[OtherViolationsScore])
			WHEN 2 THEN (
					SELECT [AttendanceSettingScore]
					FROM #EPATemp
					WHERE [EPAId] = empV.[EPAId]
					) - SUM(empV.OtherViolationsScore)
			ELSE 0
			END AS SystemDisciplineScore
		,CASE (
				SELECT [ScoreRatingTypeId]
				FROM [dbo].[EPA] WITH (NOLOCK)
				WHERE [EPAId] = empV.[EPAId]
				)
			WHEN 1 THEN CASE 
					WHEN (
							SELECT [AttendanceSettingScore]
							FROM #EPATemp
							WHERE [EPAId] = empV.[EPAId]
							) - SUM(empV.[ApproverSubtractScore] + empV.OtherViolationsScore) > 0 THEN (
							SELECT [AttendanceSettingScore]
							FROM #EPATemp
							WHERE [EPAId] = empV.[EPAId]
							) - SUM(empV.[ApproverSubtractScore] + empV.OtherViolationsScore)
					ELSE 0
					END
			WHEN 2 THEN (
					SELECT [AttendanceSettingScore]
					FROM #EPATemp
					WHERE [EPAId] = empV.[EPAId]
					) - SUM(empV.OtherViolationsScore)
			ELSE 0
			END AS DisciplineScore
	FROM #EmployeeViolationFinalScore AS empV WITH (NOLOCK)
	GROUP BY empV.[EPAId]
	
	UNION
	SELECT epaT.[EPAId]
		,(
			SELECT [Attendance]
			FROM [dbo].[ScoreRatingType] AS srt WITH (NOLOCK)
			WHERE EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[EPA] WITH (NOLOCK)
					WHERE [EPAId] = epaT.[EPAId]
						AND [ScoreRatingTypeId] = srt.[ScoreRatingTypeId]
					)
			) AS SystemDisciplineScore
		,(
			SELECT [Attendance]
			FROM [dbo].[ScoreRatingType] AS srt WITH (NOLOCK)
			WHERE EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[EPA] WITH (NOLOCK)
					WHERE [EPAId] = epaT.[EPAId]
						AND [ScoreRatingTypeId] = srt.[ScoreRatingTypeId]
					)
			) AS DisciplineScore
	FROM #EPATemp AS epaT
	WHERE NOT EXISTS (
			SELECT TOP 1 1
			FROM #EmployeeViolation
			WHERE [EPAId] = epaT.[EPAId]
			)

	DROP TABLE #EmployeeViolationSystemScore

	DROP TABLE #EmployeeViolationFinalScore

	DROP TABLE #EPATemp

	DROP TABLE #EmployeeViolation
END