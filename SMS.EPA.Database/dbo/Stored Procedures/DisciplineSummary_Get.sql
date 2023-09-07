
CREATE PROCEDURE [dbo].[DisciplineSummary_Get] @StartDate DATE
	,@EndDate DATE
	,@ApproverId INT
	,@TeamIds VARCHAR(MAX) = NULL
	,@EmployeeId INT = NULL
	,@ReasonId SMALLINT = NULL
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

	CREATE TABLE #tmpTeam (TeamId INT)

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
		,SevenDaysLeave DECIMAL(5, 2)
		,OtherViolationsScore DECIMAL(5, 2)
		,SystemSubtractScore DECIMAL(5, 2)
		,ApproverSubtractScore DECIMAL(5, 2)
		)

	IF @TeamIds IS NOT NULL
	BEGIN
		INSERT INTO #tmpTeam (TeamId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
		WHERE ISNUMERIC(val) = 1
	END

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
		,SevenDaysLeave
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
		,SUM(ISNULL(d.SevenDaysLeave, 0))
		,SUM(d.OtherDisciplines)
		,SUM(d.SystemSubtractScore)
		,SUM(d.ChangeShift) + SUM(d.MissingCheckInOut) + SUM(d.LateIn) + SUM(d.EarlyOut) + SUM(d.[Absent]) + SUM(d.UnpaidLeave)
			+ SUM(d.WorkingUndertime) + SUM(d.UnpaidLeaveAbsent) + SUM(ISNULL(d.UnplannedLeave, 0)) + SUM(ISNULL(d.WeekendLeave, 0)) + SUM(ISNULL(d.SevenDaysLeave, 0))
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.DisciplineSummary AS d WITH (NOLOCK) ON emp.EmployeeId = d.EmployeeId
		AND d.IsDeleted = 0
		AND d.WorkDate BETWEEN @StartDate
			AND @EndDate
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND (
			@EmployeeId IS NULL
			OR emp.EmployeeId = @EmployeeId
			)
		AND (
			@IsLocal IS NULL
			OR emp.IsLocal = @IsLocal
			)
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

	SELECT d.ChangeShift
		,d.MissingCheckInOut
		,d.LateIn
		,d.EarlyOut
		,d.[Absent]
		,d.UnpaidLeave
		,d.WorkingUnderTime
		,d.UnpaidLeaveAbsent
		,d.UnplannedLeave
		,d.WeekendLeave
		,d.SevenDaysLeave
		,d.OtherViolationsScore AS OtherDisciplines
		,ISNULL(d.SystemSubtractScore, 0) + ISNULL(d.OtherViolationsScore, 0) AS SystemSubtractScore
		,ISNULL(d.ApproverSubtractScore, 0) + ISNULL(d.OtherViolationsScore, 0) AS ApproverSubtractScore
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.JoinedDate
		,(CASE 
			WHEN emp.IsLocal = 1 THEN 'Local'
			ELSE 'Expat' END
			) AS Category
		,emp.Code
		,dept.[Name] AS Department
		,sec.[Name] AS Section
		,gr.[Name] AS Team
		,job.[Name] AS JobTitle
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Section AS sec WITH (NOLOCK) ON gr.DepartmentId = sec.SectionId
		AND sec.IsActive = 1
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON sec.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
		LEFT JOIN #tblEmployeeDiscipline AS d WITH (NOLOCK) ON emp.EmployeeId = d.EmployeeId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND (
			@EmployeeId IS NULL
			OR emp.EmployeeId = @EmployeeId
			)
		AND (
			@IsLocal IS NULL
			OR emp.IsLocal = @IsLocal
			)
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
					AND emp.EmployeeId != @ApproverId
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
			@TeamIds IS NULL
			OR EXISTS (
				SELECT 1
				FROM #tmpTeam AS tmp WITH (NOLOCK)
				WHERE emp.GroupId = tmp.TeamId
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
				@ReasonId = 11
				AND d.EmployeeId IS NOT NULL
				AND d.SevenDaysLeave > 0
				)
			OR (
				@ReasonId < 0
				AND d.EmployeeId IS NOT NULL
				AND d.OtherViolationsScore > 0
				)
			)
	ORDER BY dept.[Name]
		,sec.[Name]
		,gr.[Name]
		,emp.Code

	DROP TABLE #tmpTeam

	DROP TABLE #tblEmployeeDiscipline
END