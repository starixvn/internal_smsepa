/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_GetDetail    Script Date: 7/21/2016 2:45:47 PM ******/
CREATE PROCEDURE [dbo].[EpaAttendanceSummary_GetDetail] @StartDate DATE
	,@EndDate DATE
	,@ApproverId INT
	,@DepartmentId INT = NULL
	,@EmployeeId INT = NULL
	,@ReasonId INT = NULL
	,@IsLocal BIT = NULL
	,@HasIgnored BIT = NULL
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Get attendance detail of employees.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #DisciplineDetail (
		WorkDate DATE
		,ShiftName NVARCHAR(100)
		,CheckIn DATETIME NULL
		,CheckOut DATETIME NULL
		,TotalHours FLOAT
		,LateIn INT
		,EarlyOut INT
		,EmployeeId INT
		,Code NVARCHAR(20)
		,NickName NVARCHAR(20)
		,FullName NVARCHAR(100)
		,JoinedDate DATETIME
		,Category NVARCHAR(100)
		,GroupId INT
		,GroupName NVARCHAR(150)
		,GroupCode NVARCHAR(20)
		,JobTitleId INT
		,JobTitleName NVARCHAR(100)
		,ChangeShiftId INT
		,ChangeShiftValue INT
		,ChangeShiftScore DECIMAL(5, 2)
		,ChangeShiftHasIgnored BIT
		,MissingCheckInOutId INT
		,MissingCheckInOutValue INT
		,MissingCheckInOutScore DECIMAL(5, 2)
		,MissingCheckInOutHasIgnored BIT
		,LateInId INT
		,LateInValue INT
		,LateInScore DECIMAL(5, 2)
		,LateInHasIgnored BIT
		,EarlyOutId INT
		,EarlyOutValue INT
		,EarlyOutScore DECIMAL(5, 2)
		,EarlyOutHasIgnored BIT
		,AbsentId INT
		,AbsentValue INT
		,AbsentScore DECIMAL(5, 2)
		,AbsentHasIgnored BIT
		,UnpaidLeaveId INT
		,UnpaidLeaveValue INT
		,UnpaidLeaveScore DECIMAL(5, 2)
		,UnpaidLeaveHasIgnored BIT
		,TotalHoursId INT
		,TotalHoursValue INT
		,TotalHoursScore DECIMAL(5, 2)
		,TotalHoursHasIgnored BIT
		,UnpaidLeaveAbsentId INT
		,UnpaidLeaveAbsentValue INT
		,UnpaidLeaveAbsentScore DECIMAL(5, 2)
		,UnpaidLeaveAbsentHasIgnored BIT
		,EmployeeComment NVARCHAR(500)
		,ApproverComment NVARCHAR(500)
		)

	INSERT INTO #DisciplineDetail
	SELECT DISTINCT empV.WorkDate
		,empV.ShiftName
		,empV.CheckIn
		,empV.CheckOut
		,empV.TotalHours
		,empV.LateIn
		,empV.EarlyOut
		,emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.JoinedDate
		,lk.[Name] AS Category
		,gr.GroupId
		,gr.[Name] AS GroupName
		,gr.Code AS GroupCode
		,emp.JobTitle AS JobTitleId
		,job.[Name] AS JobTitleName
		,1 AS ChangeShiftId
		,empV.IsChangeShift AS ChangeShiftValue
		,dbo.fncGetDisciplineScoreById(1) AS ChangeShiftScore
		,empv.IsIgnoredChangeShift AS ChangeShiftHasIgnored
		,2 AS MissingCheckInOutId
		,empV.IsMissingCheckInOut AS MissingCheckInOutValue
		,dbo.fncGetDisciplineScoreById(2) AS MissingCheckInOutScore
		,empv.IsIgnoredMissingCheckInOut AS MissingCheckInOutHasIgnored
		,3 AS LateInId
		,empV.IsLateIn AS LateInValue
		,dbo.fncGetDisciplineScoreById(3) AS LateInScore
		,empV.IsIgnoredLateIn AS LateInHasIgnored
		,4 AS EarlyOutId
		,empV.IsEarlyOut AS EarlyOutValue
		,dbo.fncGetDisciplineScoreById(4) AS EarlyOutScore
		,empV.IsIgnoredEarlyOut AS EarlyOutHasIgnored
		,5 AS AbsentId
		,empV.IsAbsent AS AbsentValue
		,dbo.fncGetDisciplineScoreById(5) AS AbsentScore
		,empV.IsIgnoredAbsent AS AbsentHasIgnored
		,6 AS UnpaidLeaveId
		,empV.IsUnpaidLeave AS UnpaidLeaveValue
		,dbo.fncGetDisciplineScoreById(6) AS UnpaidLeaveScore
		,empV.IsIgnoredUnpaidLeave AS UnpaidLeaveHasIgnored
		,7 AS TotalHoursId
		,empV.IsWorkingUnderTime AS TotalHoursValue
		,dbo.fncGetDisciplineScoreById(7) AS TotalHoursScore
		,empV.IsIgnoredWorkingUndertime AS TotalHoursHasIgnored
		,8 AS UnpaidLeaveAbsentId
		,empV.IsUnpaidLeaveAbsent AS UnpaidLeaveAbsentValue
		,dbo.fncGetDisciplineScoreById(8) AS UnpaidLeaveAbsentScore
		,empV.IsIgnoredUnpaidLeaveAbsent AS UnpaidLeaveAbsentHasIgnored
		,empV.EmployeeComment
		,empV.ApproverComment
	FROM dbo.EmployeeDiscipline AS empV WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON empV.EmpId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND ISNULL(emp.IsDeleted, 0) = 0
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN [dbo].[LookupData] AS lk WITH (NOLOCK) ON emp.IsLocal = CAST(lk.[Value] AS BIT)
		AND lk.LookupId = 11
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
	WHERE empV.DisciplineTypeId > 0
		AND ISNULL(empV.IsDeleted, 0) = 0
		AND empV.WorkDate BETWEEN @StartDate
			AND @EndDate
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR emp.EmployeeId = @EmployeeId
			)
		AND (
			ISNULL(@ReasonId, '') = ''
			OR (
				@ReasonId = 1
				AND empV.IsChangeShift = 1
				)
			OR (
				@ReasonId = 2
				AND empV.IsMissingCheckInOut = 1
				)
			OR (
				@ReasonId = 3
				AND empV.IsLateIn = 1
				)
			OR (
				@ReasonId = 4
				AND empV.IsEarlyOut = 1
				)
			OR (
				@ReasonId = 5
				AND empV.IsAbsent = 1
				)
			OR (
				@ReasonId = 6
				AND empV.IsUnpaidLeave = 1
				)
			OR (
				@ReasonId = 7
				AND empV.IsWorkingUndertime = 1
				)
			OR (
				@ReasonId = 8
				AND empV.IsUnpaidLeaveAbsent = 1
				)
			)
		AND (
			empV.IsChangeShift > 0
			OR empV.IsMissingCheckInOut > 0
			OR empV.IsLateIn > 0
			OR empV.IsEarlyOut > 0
			OR empV.IsAbsent > 0
			OR empV.IsUnpaidLeave > 0
			OR empV.IsWorkingUndertime > 0
			OR empV.IsUnpaidLeaveAbsent > 0
			)
		AND (
			ISNULL(@DepartmentId, '') = ''
			OR (
				@DepartmentId < 0
				AND emp.GroupId = - @DepartmentId
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
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empR WITH (NOLOCK)
				WHERE empR.EmployeeId = @ApproverId
					AND empR.RoleId IN (1, 2, 3) -- SA/HR/MGT
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeDisciplinePermission AS empDP WITH (NOLOCK)
				WHERE empDP.OwnerId = @ApproverId
					AND empDP.EmpId = emp.EmployeeId
					AND ISNULL(empDP.IsDeleted, 0) = 0
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.TeamDisciplinePermission AS teamDP WITH (NOLOCK)
				WHERE gr.GroupId = teamDP.TeamId
					AND teamDP.OwnerId = @ApproverId
					AND emp.EmployeeId <> @ApproverId
					AND ISNULL(teamDP.IsDeleted, 0) = 0
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empR WITH (NOLOCK)
				WHERE empR.EmployeeId = @ApproverId
					AND RoleId = 6 -- Employee
					AND empV.EmpId = @ApproverId
				)
			)
		AND (
			@IsLocal IS NULL
			OR emp.[IsLocal] = @IsLocal
			)
		AND (
			@HasIgnored IS NULL
			OR (
				@HasIgnored = 1
				AND (
					(
						empV.IsChangeShift = 1
						AND empV.IsIgnoredChangeShift = 1
						)
					OR (
						empV.IsMissingCheckInOut = 1
						AND empV.IsIgnoredMissingCheckInOut = 1
						)
					OR (
						empV.IsLateIn = 1
						AND empV.IsIgnoredLateIn = 1
						)
					OR (
						empV.IsEarlyOut = 1
						AND empV.IsIgnoredEarlyOut = 1
						)
					OR (
						empV.IsAbsent = 1
						AND empV.IsIgnoredAbsent = 1
						)
					OR (
						empV.IsUnpaidLeave = 1
						AND empV.IsIgnoredUnpaidLeave = 1
						)
					OR (
						empV.IsWorkingUnderTime = 1
						AND empV.IsIgnoredWorkingUndertime = 1
						)
					OR (
						empV.IsUnpaidLeaveAbsent = 1
						AND empV.IsIgnoredUnpaidLeaveAbsent = 1
						)
					)
				)
			OR (
				@HasIgnored = 0
				AND (
					(
						empV.IsChangeShift = 1
						AND empV.IsIgnoredChangeShift = 0
						)
					OR (
						empV.IsMissingCheckInOut = 1
						AND empV.IsIgnoredMissingCheckInOut = 0
						)
					OR (
						empV.IsLateIn = 1
						AND empV.IsIgnoredLateIn = 0
						)
					OR (
						empV.IsEarlyOut = 1
						AND empV.IsIgnoredEarlyOut = 0
						)
					OR (
						empV.IsAbsent = 1
						AND empV.IsIgnoredAbsent = 0
						)
					OR (
						empV.IsUnpaidLeave = 1
						AND empV.IsIgnoredUnpaidLeave = 0
						)
					OR (
						empV.IsWorkingUnderTime = 1
						AND empV.IsIgnoredWorkingUndertime = 0
						)
					OR (
						empV.IsUnpaidLeaveAbsent = 1
						AND empV.IsIgnoredUnpaidLeaveAbsent = 0
						)
					)
				)
			)
	ORDER BY empV.WorkDate DESC

	SELECT d.*
	FROM #DisciplineDetail AS d
	ORDER BY d.WorkDate DESC

	DROP TABLE #DisciplineDetail
END