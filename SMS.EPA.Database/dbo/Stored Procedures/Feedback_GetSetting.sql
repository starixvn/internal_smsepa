
CREATE PROCEDURE [dbo].[Feedback_GetSetting] @PeriodId INT
	,@DepartmentId INT = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 20200602@Tim.Trinh
Task: Get feedback setting
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblFeedback (
		PeriodId INT
		,EmployeeId INT
		,FromDate DATE
		,ToDate DATE
		)

	INSERT INTO #tblFeedback (
		PeriodId
		,EmployeeId
		,FromDate
		,ToDate
		)
	SELECT fb.PeriodId
		,fb.EmployeeId
		,MIN(fb.FromDate)
		,MIN(fb.ToDate)
	FROM dbo.Feedback AS fb WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON fb.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
	WHERE fb.PeriodId = @PeriodId
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
	GROUP BY fb.PeriodId
		,fb.EmployeeId

	SELECT fb.PeriodId
		,fb.FromDate
		,fb.ToDate
		,emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,gr.[Name] AS Section
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblFeedback AS fb WITH (NOLOCK) ON emp.EmployeeId = fb.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	ORDER BY emp.Code

	SELECT fb.EmployeeId
		,fb.EvaluatorId
		,eva.Code
		,eva.NickName AS Nickname
		,eva.FullName
	FROM dbo.Feedback AS fb WITH (NOLOCK)
		INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON fb.EvaluatorId = eva.EmployeeId
	WHERE fb.PeriodId = @PeriodId

	DROP TABLE #tblFeedback
END
