CREATE PROCEDURE [dbo].[EmployeeDisciplineSummary_GetByPeriod] @PeriodId INT = NULL
	,@DepartmentId INT = NULL
	,@EmployeeId INT = NULL
	,@DisciplineTypeId INT = NULL
	,@IsLocal BIT = NULL
AS
/*
Created: 20160915@Tim.Trinh
Task: Get discipline summary by period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	SELECT DISTINCT 0 AS ChangeShift
		,SUM(d.MissingCheckInOut) AS MissingCheckInOut
		,SUM(d.LateIn) AS LateIn
		,0 AS EarlyOut
		,0 AS [Absent]
		,SUM(d.UnpaidLeave) AS UnpaidLeave
		,SUM(d.WorkingUndertime) AS WorkingUndertime
		,0 AS UnpaidLeaveAbsent
		,SUM(ISNULL(d.UnplannedLeave, 0)) AS UnplannedLeave
		,SUM(ISNULL(d.WeekendLeave, 0)) AS WeekendLeave
		,SUM(d.OtherDisciplines) AS OtherDisciplines
		,SUM(d.SystemSubtractScore - d.ChangeShift - d.EarlyOut - d.[Absent] - d.UnpaidLeaveAbsent) AS SystemSubtractScore
		,period.PeriodId
		,period.[Name]
		,period.StartDate
		,period.EndDate
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,lk.[Name] AS Category
		,emp.Code
		,emp.JoinedDate
		,gr.GroupId
		,gr.[Name] AS GroupName
		,emp.JobTitle AS JobTitleId
		,job.[Name] AS JobTitleName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeDisciplineSummary AS d WITH (NOLOCK) ON emp.EmployeeId = d.EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON d.Period BETWEEN period.AttendanceStartDate
			AND period.AttendanceEndDate
		AND period.TargetTypeId = 3 -- semi
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.LookupData AS lk WITH (NOLOCK) ON emp.IsLocal = CAST(lk.Value AS BIT)
		AND lk.LookupId = 11
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND (
			ISNULL(@PeriodId, '') = ''
			OR period.PeriodId = @PeriodId
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
			d.ChangeShift > 0
			OR d.MissingCheckInOut > 0
			OR d.LateIn > 0
			OR d.EarlyOut > 0
			OR d.[Absent] > 0
			OR d.UnpaidLeave > 0
			OR d.WorkingUndertime > 0
			OR d.UnplannedLeave > 0
			OR d.WeekendLeave > 0
			)
		AND (
			ISNULL(@DisciplineTypeId, '') = ''
			OR (
				@DisciplineTypeId = 1
				AND d.ChangeShift > 0
				)
			OR (
				@DisciplineTypeId = 2
				AND d.MissingCheckInOut > 0
				)
			OR (
				@DisciplineTypeId = 3
				AND d.LateIn > 0
				)
			OR (
				@DisciplineTypeId = 4
				AND d.EarlyOut > 0
				)
			OR (
				@DisciplineTypeId = 5
				AND d.[Absent] > 0
				)
			OR (
				@DisciplineTypeId = 6
				AND d.UnpaidLeave > 0
				)
			OR (
				@DisciplineTypeId = 7
				AND d.WorkingUndertime > 0
				)
			OR (
				@DisciplineTypeId = 8
				AND d.UnpaidLeaveAbsent > 0
				)
			OR (
				@DisciplineTypeId = 9
				AND d.UnplannedLeave > 0
				)
			OR (
				@DisciplineTypeId = 10
				AND d.WeekendLeave > 0
				)
			OR (
				@DisciplineTypeId = - 1
				AND d.OtherDisciplines > 0
				)
			)
		AND (
			@IsLocal IS NULL
			OR emp.IsLocal = @IsLocal
			)
	GROUP BY period.PeriodId
		,period.[Name]
		,period.StartDate
		,period.EndDate
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,lk.[Name]
		,emp.Code
		,emp.JoinedDate
		,gr.GroupId
		,gr.[Name]
		,emp.JobTitle
		,job.[Name]
	ORDER BY period.EndDate
		,gr.[Name]
		,emp.Code
END