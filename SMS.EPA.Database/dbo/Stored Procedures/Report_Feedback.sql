
CREATE PROCEDURE [dbo].[Report_Feedback] @PeriodId INT
	,@DepartmentId INT = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 20200528@Tim.Trinh
Task: feedback report.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	CREATE TABLE #tblEmployeeId (EmployeeId INT)

	INSERT INTO #tblEmployeeId (EmployeeId)
	SELECT DISTINCT fb.EmployeeId
	FROM dbo.Feedback AS fb WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON fb.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
	WHERE fb.PeriodId = @PeriodId
		AND fb.FromDate <= @Today
		AND (
			@DepartmentId IS NULL
			OR (
				@DepartmentId < 0
				AND gr.GroupId = ABS(@DepartmentId)
				)
			OR (
				@DepartmentId > 0
				AND (
					dept.DepartmentId = @DepartmentId
					OR dept.DepartmentId IN (
						SELECT DepartmentId
						FROM dbo.RelatedDepartment(@DepartmentId)
						
						UNION
						SELECT @DepartmentId
						)
					)
				)
			)
		AND (
			@EmployeeId IS NULL
			OR fb.EmployeeId = @EmployeeId
			)

	SELECT DISTINCT fb.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,gr.[Name] AS Section
	FROM dbo.Feedback AS fb WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON fb.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE fb.PeriodId = @PeriodId
		AND EXISTS (
			SELECT 1
			FROM #tblEmployeeId AS tbl WITH (NOLOCK)
			WHERE fb.EmployeeId = tbl.EmployeeId
			)
	ORDER BY emp.Code

	SELECT fb.FeedbackId
		,fb.FinalScore
		,fb.Comment
		,fb.EvaluatedDate
		,fb.EmployeeId
		,fb.EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,gr.[Name] AS Section
	FROM dbo.Feedback AS fb WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON fb.EvaluatorId = emp.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE fb.PeriodId = @PeriodId
		AND EXISTS (
			SELECT 1
			FROM #tblEmployeeId AS tbl WITH (NOLOCK)
			WHERE fb.EmployeeId = tbl.EmployeeId
			)

	DROP TABLE #tblEmployeeId
END