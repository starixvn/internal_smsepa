
CREATE PROCEDURE [dbo].[Epa_GetMonthlyEPAHistory] @PeriodId INT = 0
	,@EvaluatorId INT
	,@EmployeeId INT = NULL
AS
/*
Created: 20160606@Razor.Nguyen
Task: Get Monthly EPA history.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Is_HR_SA BIT
		,@MonthlyTargetId TINYINT = 2

	CREATE TABLE #tblLastedEPA (
		EmployeeId INT
		,StartDate DATE
		)

	SET @Is_HR_SA = CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EmployeeRole AS empR
					WHERE empR.EmployeeId = @EvaluatorId
						AND RoleId IN (1)
					) THEN 1
			ELSE 0
			END

	INSERT INTO #tblLastedEPA (
		EmployeeId
		,StartDate
		)
	SELECT DISTINCT epa.EmployeeId
		,MAX(process.StartDate)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS process WITH (NOLOCK) ON epa.EPAId = process.EPAId
		AND process.ProcessId = 1 -- HR
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.TargetTypeId = @MonthlyTargetId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND emp.ResignedDate IS NULL
	GROUP BY epa.EmployeeId

	SELECT epa.FinalScore AS EncodeFinalScore
		,epa.Grade AS EncodeGrade
		,1 AS HasEPAForm
		,epa.EPAId
		,pe.PeriodId
		,pe.[Name]
		,pe.StartDate
		,pe.EndDate
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,sec.[Name] AS Section
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,gr.GroupId
		,gr.[Name] AS GroupName
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON epa.PeriodId = pe.PeriodId
		AND pe.TargetTypeId = @MonthlyTargetId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
		LEFT JOIN dbo.Section AS sec WITH (NOLOCK) ON gr.DepartmentId = sec.SectionId
		AND sec.IsActive = 1
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON sec.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
	WHERE epa.PeriodId = @PeriodId
		AND epa.[Status] = 1 -- Closed
		AND (
			@Is_HR_SA = 1
			OR (
				EXISTS (
					SELECT 1
					FROM dbo.EmployeeRole AS erole WITH (NOLOCK)
						INNER JOIN dbo.Employee AS emp1 ON erole.EmployeeId = emp1.EmployeeId
					WHERE erole.EmployeeId = @EvaluatorId
						AND erole.RoleId IN (4, 5) -- Reviewer, Evaluator
					)
				AND EXISTS (
					SELECT 1
					FROM dbo.EPA AS epa1 WITH (NOLOCK)
						INNER JOIN dbo.EPAProcess AS process1 WITH (NOLOCK) ON epa1.EPAId = process1.EPAId
						AND process1.ProcessId > 2 -- self
						AND process1.EvaluatorId = @EvaluatorId
						AND epa1.EmployeeId = epa.EmployeeId
						INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa1.PeriodId = period1.PeriodId
						AND period1.TargetTypeId = pe.TargetTypeId
					WHERE epa1.EPAId IN (
							SELECT 1
							FROM dbo.EPA AS epa2 WITH (NOLOCK)
								INNER JOIN dbo.EPAProcess AS process2 WITH (NOLOCK) ON epa2.EPAId = process2.EPAId
								AND process2.ProcessId = 1 -- HR
								INNER JOIN dbo.Period AS period2 WITH (NOLOCK) ON epa2.PeriodId = period2.PeriodId
								AND period2.TargetTypeId = pe.TargetTypeId
								INNER JOIN #tblLastedEPA AS lastedepa WITH (NOLOCK) ON epa2.EmployeeId = lastedepa.EmployeeId
								AND process2.StartDate = lastedepa.StartDate
							)
					)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS erole WITH (NOLOCK)
				WHERE erole.EmployeeId = @EvaluatorId
					AND erole.RoleId = 6 -- employee
					AND emp.EmployeeId = @EvaluatorId
				)
			)
		AND ISNULL(@EmployeeId, emp.EmployeeId) = emp.EmployeeId
	ORDER BY pe.StartDate DESC
		,emp.Code ASC

	DROP TABLE #tblLastedEPA
END