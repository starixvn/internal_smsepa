
CREATE PROCEDURE [dbo].[Report_EPAComment] @PeriodId INT
AS
/*
Created:
Task:
DB: SMSEPA

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT emp.Code
		,emp.NickName
		,emp.FullName
		,section.[Name] AS Section
		,gr.[Name] AS Team
		,selfp.Comment AS StaffComment
		,eva1.NickName AS EvaluatorNickName
		,eva.Comment AS EvaluatorComment
		,eva.ProcessId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS section WITH (NOLOCK) ON gr.DepartmentId = section.DepartmentId
		AND section.IsActive = 1
		INNER JOIN dbo.EPAProcess AS selfp WITH (NOLOCK) ON epa.EPAId = selfp.EPAId
		AND selfp.ProcessId = 2
		INNER JOIN dbo.EPAProcess AS eva WITH (NOLOCK) ON epa.EPAId = eva.EPAId
		AND eva.ProcessId > 2
		INNER JOIN dbo.Employee AS eva1 WITH (NOLOCK) ON eva.EvaluatorId = eva1.EmployeeId
	WHERE epa.[Status] = 1
		AND period.PeriodId = @PeriodId
		AND EXISTS (
			SELECT Comment
			FROM dbo.EPAProcess AS prc WITH (NOLOCK)
			WHERE prc.EPAId = epa.EPAId
				AND ISNULL(prc.Comment, '') <> ''
			)
	ORDER BY emp.Code ASC
		,ProcessId ASC
END