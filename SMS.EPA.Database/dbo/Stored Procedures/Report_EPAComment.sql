CREATE PROCEDURE Report_EPAComment @PeriodId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT emp.Code,
		emp.NickName,
		gr.[Name] AS Section,
		self.Comment AS StaffComment,
		eva1.NickName AS EvaluatorNickName,
		eva.Comment AS EvaluatorComment,
		eva.ProcessId
	FROM dbo.EPA as epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId AND emp.Enabled = 1 AND emp.IsDeleted = 0
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.EPAProcess AS self WITH (NOLOCK) ON epa.EPAId = self.EPAId AND self.ProcessId = 2
		INNER JOIN dbo.EPAProcess AS eva WITH (NOLOCK) ON epa.EPAId = eva.EPAId AND eva.ProcessId > 2
		INNER JOIN dbo.Employee AS eva1 WITH (NOLOCK) ON eva.EvaluatorId = eva1.EmployeeId
	WHERE epa.[Status] = 1 AND period.PeriodId = @PeriodId
	AND EXISTS
		(
		SELECT Comment 
		FROM EPAProcess prc 
		WHERE 
			prc.EPAId = epa.EPAId
			AND ISNULL(prc.Comment, '') <> ''
		)
	ORDER BY emp.Code ASC, ProcessId ASC
END