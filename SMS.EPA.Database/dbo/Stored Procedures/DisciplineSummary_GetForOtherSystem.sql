
CREATE PROCEDURE [dbo].[DisciplineSummary_GetForOtherSystem] @StartDate DATE
	,@EndDate DATE
	,@TeamIds VARCHAR(MAX) = NULL
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
		,WorkingUndertime DECIMAL(5, 2)
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
	END

	INSERT INTO #tblEmployeeDiscipline (
		EmployeeId
		,ChangeShift
		,MissingCheckInOut
		,LateIn
		,EarlyOut
		,[Absent]
		,UnpaidLeave
		,WorkingUndertime
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
		,SUM(d.ChangeShift) + SUM(d.MissingCheckInOut) + SUM(d.LateIn) + SUM(d.EarlyOut) + SUM(d.[Absent]) + SUM(d.UnpaidLeave) + SUM(d.WorkingUndertime) + SUM(d.UnpaidLeaveAbsent) + SUM(ISNULL(d.UnplannedLeave, 0)) + SUM(ISNULL(d.WeekendLeave, 0)) + SUM(ISNULL(d.SevenDaysLeave, 0))
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

	SELECT SUM(ISNULL(d.ChangeShift, 0)) AS ChangeShift
		,SUM(ISNULL(d.MissingCheckInOut, 0)) AS MissingCheckInOut
		,SUM(ISNULL(d.LateIn, 0)) AS LateIn
		,SUM(ISNULL(d.EarlyOut, 0)) AS EarlyOut
		,SUM(ISNULL(d.[Absent], 0)) AS [Absent]
		,SUM(ISNULL(d.UnpaidLeave, 0)) AS UnpaidLeave
		,SUM(ISNULL(d.WorkingUndertime, 0)) AS WorkingUndertime
		,SUM(ISNULL(d.UnpaidLeaveAbsent, 0)) AS UnpaidLeaveAbsent
		,SUM(ISNULL(d.UnplannedLeave, 0)) AS UnplannedLeave
		,SUM(ISNULL(d.WeekendLeave, 0)) AS WeekendLeave
		,SUM(ISNULL(d.SevenDaysLeave, 0)) AS SevenDaysLeave
		,SUM(ISNULL(d.OtherViolationsScore, 0)) AS OtherDisciplines
		,SUM(ISNULL(d.SystemSubtractScore, 0) + ISNULL(d.OtherViolationsScore, 0)) AS SystemSubtractScore
		,SUM(ISNULL(d.ApproverSubtractScore, 0) + ISNULL(d.OtherViolationsScore, 0)) AS ApproverSubtractScore
		,emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.JoinedDate
		,(CASE 
			WHEN emp.IsLocal = 1 THEN 'Local'
			ELSE 'Expat' END
			) AS Category
		,dept.[Name] AS Section
		,gr.[Name] AS Team
		,job.[Name] AS JobTitle
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblEmployeeDiscipline AS d WITH (NOLOCK) ON emp.EmployeeId = d.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
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
			@TeamIds IS NULL
			OR EXISTS (
				SELECT 1
				FROM #tmpTeam AS tmp WITH (NOLOCK)
				WHERE emp.GroupId = tmp.TeamId
				)
			)
		AND (
			d.ChangeShift > 0
			OR d.MissingCheckInOut > 0
			OR d.LateIn > 0
			OR d.EarlyOut > 0
			OR d.[Absent] > 0
			OR d.UnpaidLeave > 0
			OR d.WorkingUndertime > 0
			OR d.UnpaidLeaveAbsent > 0
			OR d.UnplannedLeave > 0
			OR d.WeekendLeave > 0
			OR d.SevenDaysLeave > 0
			OR d.OtherViolationsScore > 0
			)
		AND (
			@ReasonId IS NULL
			OR (
				@ReasonId = 1
				AND d.ChangeShift > 0
				)
			OR (
				@ReasonId = 2
				AND d.MissingCheckInOut > 0
				)
			OR (
				@ReasonId = 3
				AND d.LateIn > 0
				)
			OR (
				@ReasonId = 4
				AND d.EarlyOut > 0
				)
			OR (
				@ReasonId = 5
				AND d.[Absent] > 0
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
				@ReasonId = 8
				AND d.UnpaidLeaveAbsent > 0
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
				@ReasonId = 11
				AND d.SevenDaysLeave > 0
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
		,dept.[Name]
		,gr.[Name]
		,job.[Name]
	ORDER BY gr.[Name] ASC
		,emp.Code ASC

	DROP TABLE #tmpTeam
	
	DROP TABLE #tblEmployeeDiscipline
END