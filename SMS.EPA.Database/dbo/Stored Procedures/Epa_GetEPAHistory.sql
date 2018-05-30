
CREATE PROCEDURE [dbo].[Epa_GetEPAHistory] @PeriodId INT = 0
	,@EvaluatorId INT
	,@DepartmentId INT = NULL
	,@IsLocal BIT = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 2015-10-19@Josh.Phan
Task: Get EPA history.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Is_HR_SA BIT
		,@TargetTypeId TINYINT
		,@MonthlyTargetId SMALLINT = 2

	CREATE TABLE #tblLastedEPA (
		EmployeeId INT
		,StartDate DATE
		)

	SET @Is_HR_SA = CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EmployeeRole AS empR
					WHERE empR.EmployeeId = @EvaluatorId
						AND RoleId IN (1, 2)
					) THEN 1
			ELSE 0
			END

	SELECT @TargetTypeId = TargetTypeId
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	INSERT INTO #tblLastedEPA (
		EmployeeId
		,StartDate
		)
	SELECT epa.EmployeeId
		,MAX(process.StartDate)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS process WITH (NOLOCK) ON epa.EPAId = process.EPAId
		AND process.ProcessId = 1 -- HR
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.TargetTypeId = @TargetTypeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND emp.ResignedDate IS NULL
	GROUP BY epa.EmployeeId

	SELECT data.EncodeFinalScore
		,data.EncodeGrade
		,data.HasEPAForm
		,data.HasMonthlyEPA
		,data.EPAId
		,data.PeriodId
		,data.PeriodName AS [Name]
		,data.StartDate
		,data.EndDate
		,data.TargetTypeId
		,data.EmployeeId
		,data.NickName
		,data.FullName
		,data.Code
		,data.IsLocal
		,data.GroupId
		,data.GroupName
	FROM (
		SELECT epa.FinalScore AS EncodeFinalScore
			,epa.Grade AS EncodeGrade
			,1 AS HasEPAForm
			,epa.HasMonthlyEPA
			,epa.EPAId
			,epa.[Status]
			,pe.PeriodId
			,pe.[Name] AS PeriodName
			,pe.StartDate
			,pe.EndDate
			,pe.TargetTypeId
			,emp.EmployeeId
			,emp.NickName
			,emp.FullName
			,emp.Code
			,emp.IsLocal
			,dept.DepartmentId
			,dept.[Name] AS DepartmentName
			,gr.GroupId
			,gr.[Name] AS GroupName
			,gr.Code AS GroupCode
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON epa.PeriodId = pe.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			AND emp.ResignedDate IS NULL
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		
		UNION ALL
		SELECT FinalScore AS EncodeFinalScore
			,NULL AS EncodeGrade
			,0 AS HasEPAForm
			,0 AS HasMonthlyEPA
			,NULL AS EPAId
			,1 AS [Status]
			,PeriodId
			,PeriodName
			,PeriodDate AS StartDate
			,DATEADD(DAY, 29, [PeriodDate]) AS EndDate
			,3 AS TargetTypeId
			,emp.EmployeeId
			,emp.NickName
			,emp.FullName
			,emp.Code
			,emp.IsLocal
			,dept.DepartmentId
			,dept.[Name] AS DepartmentName
			,gr.GroupId
			,gr.[Name] AS GroupName
			,gr.Code AS GroupCode
		FROM dbo.EPASummaryImport AS epa WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.Code = emp.Code
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			AND emp.ResignedDate IS NULL
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		) AS data
	WHERE data.PeriodId = @PeriodId
		AND (
			ISNULL(@DepartmentId, '') = ''
			OR (
				@DepartmentId < 0
				AND data.GroupId = ABS(@DepartmentId)
				)
			OR (
				@DepartmentId > 0
				AND (
					data.DepartmentId = @DepartmentId
					OR data.DepartmentId IN (
						SELECT DepartmentId
						FROM dbo.RelatedDepartment(@DepartmentId)
						
						UNION
						SELECT @DepartmentId
						)
					)
				)
			)
		AND ISNULL(@EmployeeId, data.EmployeeId) = data.EmployeeId
		AND ISNULL(@IsLocal, data.IsLocal) = data.IsLocal
		AND data.[Status] = 1 -- Closed
		AND data.TargetTypeId <> @MonthlyTargetId
		AND (
			@Is_HR_SA = 1
			OR (
				EXISTS (
					SELECT 1
					FROM dbo.Employee AS emp WITH (NOLOCK)
						INNER JOIN dbo.EmployeeRole AS erole WITH (NOLOCK) ON emp.EmployeeId = erole.EmployeeId
						AND erole.EmployeeId = @EvaluatorId
						AND erole.RoleId IN (3, 4, 5) -- MGT, Reviewer, Evaluator
					)
				AND EXISTS (
					SELECT 1
					FROM dbo.EPA AS epa WITH (NOLOCK)
						INNER JOIN dbo.EPAProcess AS process WITH (NOLOCK) ON epa.EPAId = process.EPAId
						AND process.ProcessId > 2 -- self
						AND process.EvaluatorId = @EvaluatorId
						AND epa.EmployeeId = data.EmployeeId
						INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
						AND period.TargetTypeId = data.TargetTypeId
					WHERE epa.EPAId IN (
							SELECT DISTINCT epa1.EPAId
							FROM dbo.EPA AS epa1 WITH (NOLOCK)
								INNER JOIN dbo.EPAProcess AS process1 WITH (NOLOCK) ON epa1.EPAId = process1.EPAId
								INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa1.PeriodId = period1.PeriodId
								AND period1.TargetTypeId = data.TargetTypeId
								AND process1.ProcessId = 1 -- HR
								INNER JOIN #tblLastedEPA AS lastedepa WITH (NOLOCK) ON epa1.EmployeeId = lastedepa.EmployeeId
								AND process1.StartDate = lastedepa.StartDate
							)
					)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.Employee AS emp WITH (NOLOCK)
					INNER JOIN dbo.EmployeeRole AS erole WITH (NOLOCK) ON emp.EmployeeId = erole.EmployeeId
					AND erole.EmployeeId = @EvaluatorId
					AND erole.RoleId = 6 -- employee
					AND data.EmployeeId = @EvaluatorId
				)
			)
	ORDER BY data.StartDate DESC
		,data.GroupCode
		,data.Code

	DROP TABLE #tblLastedEPA
END