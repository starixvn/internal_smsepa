CREATE PROCEDURE [dbo].[Report_EPARecommendation] @PeriodId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT emp.Code
	,emp.NickName
	,emp.FullName
	,gr.[Name] AS Section
	,CASE
		WHEN rc.Promoted = 1 THEN 'Yes'
		ELSE ''
		END AS Promotion
	,CASE
		WHEN rc.PayIncreased = 1 THEN 'Yes'
		ELSE ''
		END AS 'AnnualIncrement'
	,CASE
		WHEN rc.JobGradeIncreased = 1 THEN jobgrade.[Name]
		ELSE ''
		END AS 'JobGrade'
	,rc.ProbationStatus
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.Enabled = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.EPARecommendation AS rc WITH (NOLOCK) ON epa.EPAId = rc.EPAId
		LEFT JOIN dbo.JobGrade AS jobgrade WITH (NOLOCK) ON rc.JobGradeId = jobgrade.JobGradeId
	WHERE epa.[Status] = 1
		AND period.PeriodId = @PeriodId
		AND (
			rc.Promoted = 1
			OR rc.PayIncreased = 1
			OR rc.JobGradeIncreased = 1
			OR (
				period.TargetTypeId = 1 -- Probation
				AND rc.ProbationStatus IS NOT NULL
			)
		)
	ORDER BY emp.Code
END