CREATE PROCEDURE [dbo].[EpaAttendanceSummary_Get] @StartDate DATE
	,@EndDate DATE
	,@ApproverId INT
	,@DepartmentId INT = NULL
	,@EmployeeId INT = NULL
	,@ReasonId INT = NULL
	,@IsLocal BIT = NULL
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

	CREATE TABLE #EmployeeDiscipline (
		WorkDate DATE
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

	INSERT INTO #EmployeeDiscipline
	SELECT DISTINCT WorkDate
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
		SELECT WorkDate
			,EmpId
			,IsChangeShift AS ChangeShift
			,IsMissingCheckInOut AS MissingCheckInOut
			,IsLateIn AS LateIn
			,IsEarlyOut AS EarlyOut
			,IsAbsent AS [Absent]
			,IsUnpaidLeave AS UnpaidLeave
			,IsWorkingUnderTime AS WorkingUnderTime
			,IsUnpaidLeaveAbsent AS UnpaidLeaveAbsent
			,0 AS OtherViolationsScore
		FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
		WHERE dis.DisciplineTypeId NOT IN (- 1, - 2) -- verbal/written warning
			AND ISNULL(dis.IsDeleted, 0) = 0
		
		UNION ALL
		SELECT WorkDate
			,EmpId
			,0 AS ChangeShift
			,0 AS MissingCheckInOut
			,0 AS LateIn
			,0 AS EarlyOut
			,0 AS [Absent]
			,0 AS UnpaidLeave
			,0 AS WorkingUnderTime
			,0 AS UnpaidLeaveAbsent
			,dtype.Score AS OtherViolationsScore
		FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
			INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON dis.DisciplineTypeId = dtype.DisciplineTypeId
		WHERE dis.DisciplineTypeId IN (- 1, - 2) -- verbal/written warning
			AND ISNULL(dis.IsDeleted, 0) = 0
		) AS empV
	WHERE WorkDate BETWEEN @StartDate
			AND @EndDate
	GROUP BY EmpId
		,WorkDate

	CREATE TABLE #EmployeeDisciplineFinalScore (
		WorkDate DATE
		,EmpId INT
		,SystemSubtractScore FLOAT
		,ApproverSubtractScore FLOAT
		)

	INSERT INTO #EmployeeDisciplineFinalScore
	SELECT empD.WorkDate
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
		WHERE dis.DisciplineTypeId = 1
			AND ISNULL(dis.IsDeleted, 0) = 0
			AND dis.WorkDate BETWEEN @StartDate
				AND @EndDate
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

	DECLARE @Is_HR_MGT_SA BIT

	SET @Is_HR_MGT_SA = CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EmployeeRole AS empR
					WHERE empR.EmployeeId = @ApproverId
						AND RoleId IN (1, 2, 3)
					) THEN 1
			ELSE 0
			END

	SELECT SUM(empV.ChangeShift) AS ChangeShift
		,SUM(empV.MissingCheckInOut) AS MissingCheckInOut
		,SUM(empV.LateIn) AS LateIn
		,SUM(empV.EarlyOut) AS EarlyOut
		,SUM(empV.[Absent]) AS [Absent]
		,SUM(empV.UnpaidLeave) AS UnpaidLeave
		,SUM(empV.WorkingUnderTime) AS WorkingUnderTime
		,SUM(empV.UnpaidLeaveAbsent) AS UnpaidLeaveAbsent
		,SUM(empV.OtherViolationsScore) AS OtherViolationsScore
		,SUM(empF.SystemSubtractScore + empV.OtherViolationsScore) AS SystemSubtractScore
		,SUM(empF.ApproverSubtractScore + empV.OtherViolationsScore) AS ApproverSubtractScore
		,Permission = CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.HRMEmployeeSupervisor AS empS WITH (NOLOCK)
					WHERE empS.OwnerId = @ApproverId
						AND empS.EmpId = emp.EmployeeId
						AND empS.ActionType = 3
						AND empS.EmpId <> @ApproverId
					)
				OR EXISTS (
					SELECT 1
					FROM dbo.HRMGroupSupervisor AS grS WITH (NOLOCK)
					WHERE gr.GroupId = grS.GroupId
						AND @ApproverId = grS.OwnerId
						AND grS.ActionType = 3
						AND emp.EmployeeId <> @ApproverId
					) THEN 2 -- Host				
			ELSE 1
			END
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.JoinedDate
		,lk.[Name] AS Category
		,emp.Code
		,gr.GroupId
		,gr.[Name] AS GroupName
		,emp.JobTitle AS JobTitleId
		,job.[Name] AS JobTitleName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #EmployeeDiscipline AS empV ON emp.EmployeeId = empV.EmpId
		LEFT JOIN #EmployeeDisciplineFinalScore AS empF ON empV.WorkDate = empF.WorkDate
		AND empV.EmpId = empF.EmpId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.LookupData AS lk WITH (NOLOCK) ON emp.IsLocal = CAST(lk.Value AS BIT)
		AND lk.LookupId = 11
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND (
			@Is_HR_MGT_SA = 1 -- SA/HR/MGT
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empR
					INNER JOIN dbo.Employee AS empT ON empR.EmployeeId = empT.EmployeeId
				WHERE empR.EmployeeId = @ApproverId
					AND RoleId IN (4, 5) -- reviewer/evaluator
					AND empT.GroupId = gr.GroupId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeDisciplinePermission AS empDP WITH (NOLOCK)
				WHERE empDP.OwnerId = @ApproverId
					AND empDP.EmpId = emp.EmployeeId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.TeamDisciplinePermission AS teamDP WITH (NOLOCK)
				WHERE gr.GroupId = teamDP.TeamId
					AND teamDP.OwnerId = @ApproverId
					AND emp.EmployeeId <> @ApproverId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empR
				WHERE empR.EmployeeId = @ApproverId
					AND RoleId = 6 -- Employee
					AND empV.EmpId = @ApproverId
				)
			)
		AND (
			ISNULL(@DepartmentId, '') = ''
			OR (
				@DepartmentId < 0
				AND gr.GroupId = - @DepartmentId
				)
			OR (
				@DepartmentId > 0
				AND EXISTS (
					SELECT 1
					FROM dbo.Department AS dept WITH (NOLOCK)
					WHERE dept.DepartmentId = @DepartmentId
						AND dept.lft <= gr.lft
						AND dept.rgt >= gr.rgt
					)
				)
			)
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR emp.EmployeeId = @EmployeeId
			)
		AND (
			ISNULL(@ReasonId, '') = ''
			OR (
				@ReasonId = 1
				AND ChangeShift > 0
				)
			OR (
				@ReasonId = 2
				AND MissingCheckInOut > 0
				)
			OR (
				@ReasonId = 3
				AND LateIn > 0
				)
			OR (
				@ReasonId = 4
				AND EarlyOut > 0
				)
			OR (
				@ReasonId = 5
				AND [Absent] > 0
				)
			OR (
				@ReasonId = 6
				AND UnpaidLeave > 0
				)
			OR (
				@ReasonId = 7
				AND WorkingUnderTime > 0
				)
			OR (
				@ReasonId = 8
				AND UnpaidLeaveAbsent > 0
				)
			)
		AND (
			@IsLocal IS NULL
			OR emp.IsLocal = @IsLocal
			)
	GROUP BY emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.JoinedDate
		,emp.JobTitle
		,job.[Name]
		,lk.[Name]
		,emp.Code
		,gr.GroupId
		,gr.[Name]
	ORDER BY gr.GroupId
		,emp.Code

	DROP TABLE #EmployeeDiscipline

	DROP TABLE #EmployeeDisciplineFinalScore
END