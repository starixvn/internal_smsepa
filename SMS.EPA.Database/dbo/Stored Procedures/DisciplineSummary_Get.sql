
CREATE PROCEDURE [dbo].[DisciplineSummary_Get] @StartDate DATE
	,@EndDate DATE
	,@ApproverId INT
	,@DepartmentId INT = NULL
	,@EmployeeId INT = NULL
	,@ReasonId INT = NULL
	,@IsLocal BIT = NULL
	,@IsShowAll BIT = 0
AS
/*
Created: 20171025@Tim.Trinh
Task: Get discipline summary.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblEmployeeDiscipline (
		EmployeeId INT
		,ChangeShift DECIMAL(5, 2)
		,MissingCheckInOut DECIMAL(5, 2)
		,LateIn DECIMAL(5, 2)
		,EarlyOut DECIMAL(5, 2)
		,[Absent] DECIMAL(5, 2)
		,UnpaidLeave DECIMAL(5, 2)
		,WorkingUnderTime DECIMAL(5, 2)
		,UnpaidLeaveAbsent DECIMAL(5, 2)
		,UnplannedLeave DECIMAL(5, 2)
		,WeekendLeave DECIMAL(5, 2)
		,OtherViolationsScore DECIMAL(5, 2)
		,SystemSubtractScore DECIMAL(5, 2)
		,ApproverSubtractScore DECIMAL(5, 2)
		)

	INSERT INTO #tblEmployeeDiscipline (
		EmployeeId
		,ChangeShift
		,MissingCheckInOut
		,LateIn
		,EarlyOut
		,[Absent]
		,UnpaidLeave
		,WorkingUnderTime
		,UnpaidLeaveAbsent
		,UnplannedLeave
		,WeekendLeave
		,OtherViolationsScore
		,SystemSubtractScore
		,ApproverSubtractScore
		)
	SELECT emp.EmployeeId
		,SUM(d.ChangeShift)
		,SUM(d.MissingCheckInOut)
		,SUM(d.LateIn)
		,SUM(d.EarlyOut)
		,SUM(d.[Absent])
		,SUM(d.UnpaidLeave)
		,SUM(d.WorkingUndertime)
		,SUM(d.UnpaidLeaveAbsent)
		,SUM(ISNULL(d.UnplannedLeave, 0))
		,SUM(ISNULL(d.WeekendLeave, 0))
		,SUM(d.OtherDisciplines)
		,SUM(d.SystemSubtractScore)
		,SUM(d.ChangeShift) + SUM(d.MissingCheckInOut) + SUM(d.LateIn) + SUM(d.EarlyOut) + SUM(d.[Absent]) + SUM(d.UnpaidLeave) + SUM(d.WorkingUndertime) + SUM(d.UnpaidLeaveAbsent) + SUM(ISNULL(d.UnplannedLeave, 0)) + SUM(ISNULL(d.WeekendLeave, 0))
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.DisciplineSummary AS d WITH (NOLOCK) ON emp.EmployeeId = d.EmployeeId
		AND d.IsDeleted = 0
		AND d.WorkDate BETWEEN @StartDate
			AND @EndDate
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND ISNULL(@EmployeeId, emp.EmployeeId) = emp.EmployeeId
		AND ISNULL(@IsLocal, emp.IsLocal) = emp.IsLocal
	GROUP BY emp.EmployeeId

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

	SELECT SUM(ISNULL(d.ChangeShift, 0)) AS ChangeShift
		,SUM(ISNULL(d.MissingCheckInOut, 0)) AS MissingCheckInOut
		,SUM(ISNULL(d.LateIn, 0)) AS LateIn
		,SUM(ISNULL(d.EarlyOut, 0)) AS EarlyOut
		,SUM(ISNULL(d.[Absent], 0)) AS [Absent]
		,SUM(ISNULL(d.UnpaidLeave, 0)) AS UnpaidLeave
		,SUM(ISNULL(d.WorkingUnderTime, 0)) AS WorkingUnderTime
		,SUM(ISNULL(d.UnpaidLeaveAbsent, 0)) AS UnpaidLeaveAbsent
		,SUM(ISNULL(d.UnplannedLeave, 0)) AS UnplannedLeave
		,SUM(ISNULL(d.WeekendLeave, 0)) AS WeekendLeave
		,SUM(ISNULL(d.OtherViolationsScore, 0)) AS OtherDisciplines
		,SUM(ISNULL(d.SystemSubtractScore, 0) + ISNULL(d.OtherViolationsScore, 0)) AS SystemSubtractScore
		,SUM(ISNULL(d.ApproverSubtractScore, 0) + ISNULL(d.OtherViolationsScore, 0)) AS ApproverSubtractScore
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.JoinedDate
		,(CASE 
			WHEN emp.IsLocal = 1 THEN 'Local'
			ELSE 'Expat' END
			) AS Category
		,emp.Code
		,gr.GroupId
		,gr.[Name] AS GroupName
		,emp.JobTitle AS JobTitleId
		,job.[Name] AS JobTitleName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
		LEFT JOIN #tblEmployeeDiscipline AS d WITH (NOLOCK) ON emp.EmployeeId = d.EmployeeId
	WHERE emp.[Enabled] = 1
		AND ISNULL(emp.IsDeleted, 0) = 0
		AND ISNULL(@EmployeeId, emp.EmployeeId) = emp.EmployeeId
		AND ISNULL(@IsLocal, emp.IsLocal) = emp.IsLocal
		AND (
			@IsShowAll = 1
			OR (
				@IsShowAll = 0
				AND d.EmployeeId IS NOT NULL
				)
			)
		AND (
			@Is_HR_MGT_SA = 1 -- SA/HR/MGT
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empR WITH (NOLOCK)
					INNER JOIN dbo.Employee AS empT WITH (NOLOCK) ON empR.EmployeeId = empT.EmployeeId
				WHERE empR.EmployeeId = @ApproverId
					AND RoleId IN (4, 5) -- reviewer/evaluator
					AND empT.GroupId = gr.GroupId
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
					AND d.EmployeeId = @ApproverId
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
			@ReasonId IS NULL
			OR (
				@ReasonId = 1
				AND d.EmployeeId IS NOT NULL
				AND d.ChangeShift > 0
				)
			OR (
				@ReasonId = 2
				AND d.EmployeeId IS NOT NULL
				AND d.MissingCheckInOut > 0
				)
			OR (
				@ReasonId = 3
				AND d.EmployeeId IS NOT NULL
				AND d.LateIn > 0
				)
			OR (
				@ReasonId = 4
				AND d.EmployeeId IS NOT NULL
				AND d.EarlyOut > 0
				)
			OR (
				@ReasonId = 5
				AND d.EmployeeId IS NOT NULL
				AND d.[Absent] > 0
				)
			OR (
				@ReasonId = 6
				AND d.EmployeeId IS NOT NULL
				AND d.UnpaidLeave > 0
				)
			OR (
				@ReasonId = 7
				AND d.EmployeeId IS NOT NULL
				AND d.WorkingUnderTime > 0
				)
			OR (
				@ReasonId = 8
				AND d.EmployeeId IS NOT NULL
				AND d.UnpaidLeaveAbsent > 0
				)
			OR (
				@ReasonId = 9
				AND d.EmployeeId IS NOT NULL
				AND d.UnplannedLeave > 0
				)
			OR (
				@ReasonId = 10
				AND d.EmployeeId IS NOT NULL
				AND d.WeekendLeave > 0
				)
			OR (
				@ReasonId < 0
				AND d.EmployeeId IS NOT NULL
				AND d.OtherViolationsScore > 0
				)
			)
	GROUP BY emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.JoinedDate
		,emp.JobTitle
		,job.[Name]
		,emp.IsLocal
		,emp.Code
		,gr.GroupId
		,gr.[Name]
	ORDER BY gr.[Name] ASC
		,emp.Code ASC

	DROP TABLE #tblEmployeeDiscipline
END