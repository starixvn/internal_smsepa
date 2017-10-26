
CREATE PROCEDURE [dbo].[DisciplineSummary_GetForOtherSystem] @StartDate DATE
	,@EndDate DATE
	,@DepartmentId INT = NULL
	,@EmployeeId INT = NULL
	,@ReasonId INT = NULL
	,@IsLocal BIT = NULL
AS
/*
Created: 20171025@Tim.Trinh
Task: Get discipline summary for CTI, GROUP, LEXFIN.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	CREATE TABLE #tblEmployeeDiscipline (
		EmployeeId INT
		,MissingCheckInOut DECIMAL(5, 2)
		,LateIn DECIMAL(5, 2)
		,UnpaidLeave DECIMAL(5, 2)
		,WorkingUnderTime DECIMAL(5, 2)
		,UnplannedLeave DECIMAL(5, 2)
		,WeekendLeave DECIMAL(5, 2)
		,OtherViolationsScore DECIMAL(5, 2)
		,SystemSubtractScore DECIMAL(5, 2)
		,ApproverSubtractScore DECIMAL(5, 2)
		)

	INSERT INTO #tblEmployeeDiscipline (
		EmployeeId
		,MissingCheckInOut
		,LateIn
		,UnpaidLeave
		,WorkingUnderTime
		,UnplannedLeave
		,WeekendLeave
		,OtherViolationsScore
		,SystemSubtractScore
		,ApproverSubtractScore
		)
	SELECT emp.EmployeeId
		,SUM(d.MissingCheckInOut)
		,SUM(d.LateIn)
		,SUM(d.UnpaidLeave)
		,SUM(d.WorkingUndertime)
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

	SELECT SUM(d.MissingCheckInOut) AS MissingCheckInOut
		,SUM(d.LateIn) AS LateIn
		,SUM(d.UnpaidLeave) AS UnpaidLeave
		,SUM(d.WorkingUnderTime) AS WorkingUnderTime
		,SUM(d.UnplannedLeave) AS UnplannedLeave
		,SUM(d.WeekendLeave) AS WeekendLeave
		,SUM(d.OtherViolationsScore) AS OtherDisciplines
		,SUM(d.SystemSubtractScore + d.OtherViolationsScore) AS SystemSubtractScore
		,SUM(d.ApproverSubtractScore + d.OtherViolationsScore) AS ApproverSubtractScore
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
		INNER JOIN #tblEmployeeDiscipline AS d WITH (NOLOCK) ON emp.EmployeeId = d.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
	WHERE emp.[Enabled] = 1
		AND ISNULL(emp.IsDeleted, 0) = 0
		AND ISNULL(@EmployeeId, emp.EmployeeId) = emp.EmployeeId
		AND ISNULL(@IsLocal, emp.IsLocal) = emp.IsLocal
		AND (
			@DepartmentId IS NULL
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
			d.MissingCheckInOut > 0
			OR d.LateIn > 0
			OR d.UnpaidLeave > 0
			OR d.WorkingUndertime > 0
			OR d.UnplannedLeave > 0
			OR d.WeekendLeave > 0
			OR d.OtherViolationsScore > 0
			)
		AND (
			@ReasonId IS NULL
			OR (
				@ReasonId = 2
				AND d.MissingCheckInOut > 0
				)
			OR (
				@ReasonId = 3
				AND d.LateIn > 0
				)
			OR (
				@ReasonId = 6
				AND d.UnpaidLeave > 0
				)
			OR (
				@ReasonId = 7
				AND d.WorkingUndertime > 0
				)
			OR (
				@ReasonId = 9
				AND d.UnplannedLeave > 0
				)
			OR (
				@ReasonId = 10
				AND d.WeekendLeave > 0
				)
			OR (
				@ReasonId = - 1
				AND d.OtherViolationsScore > 0
				)
			)
	GROUP BY emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.IsLocal
		,emp.Code
		,emp.JoinedDate
		,gr.GroupId
		,gr.[Name]
		,emp.JobTitle
		,job.[Name]
	ORDER BY gr.[Name]
		,emp.Code

	DROP TABLE #tblEmployeeDiscipline
END