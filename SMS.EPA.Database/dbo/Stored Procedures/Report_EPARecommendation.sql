
CREATE PROCEDURE [dbo].[Report_EPARecommendation] @PeriodId INT
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

	SELECT DISTINCT emp.Code
		,emp.NickName
		,emp.FullName
		,section.[Name] AS Section
		,gr.[Name] AS Team
		,CASE 
			WHEN rc.Promoted = 1 THEN 'Yes'
			ELSE ''
			END AS Promotion
		,CASE 
			WHEN rc.PayIncreased = 1 THEN 'Yes'
			ELSE ''
			END AS 'AnnualIncrement'
		,CASE 
			WHEN rc.JobLevelIncreased = 1 THEN joblevel.[Name]
			ELSE ''
			END AS 'JobLevel'
		,rc.ProbationStatus
		,CASE 
			WHEN ISNULL(md.FullName, '') != ''
				AND ISNULL(md.NickName, '') != '' THEN md.FullName + ' (' + md.NickName + ')'
			ELSE ISNULL(md.FullName, md.NickName)
			END AS ModifiedBy
		,CASE 
			WHEN ISNULL(promotedby.FullName, '') != ''
				AND ISNULL(promotedby.NickName, '') != '' THEN promotedby.FullName + ' (' + promotedby.NickName + ')'
			ELSE ISNULL(promotedby.FullName, promotedby.NickName)
			END AS PromotedBy
		,CASE 
			WHEN ISNULL(piby.FullName, '') != ''
				AND ISNULL(piby.NickName, '') != '' THEN piby.FullName + ' (' + piby.NickName + ')'
			ELSE ISNULL(piby.FullName, piby.NickName)
			END AS PayIncreasedBy
		,CASE 
			WHEN ISNULL(jliby.FullName, '') != ''
				AND ISNULL(jliby.NickName, '') != '' THEN jliby.FullName + ' (' + jliby.NickName + ')'
			ELSE ISNULL(jliby.FullName, jliby.NickName)
			END AS JobLevelIncreasedBy
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS section WITH (NOLOCK) ON gr.DepartmentId = section.DepartmentId
		AND section.IsActive = 1
		INNER JOIN dbo.EPARecommendation AS rc WITH (NOLOCK) ON epa.EPAId = rc.EPAId
		AND rc.ProcessId = 4
		LEFT JOIN dbo.JobLevel AS joblevel WITH (NOLOCK) ON rc.JobLevelId = joblevel.JobLevelId
		LEFT JOIN dbo.Employee AS md WITH (NOLOCK) ON rc.ModifiedBy = md.EmployeeId
		LEFT JOIN dbo.Employee AS promotedby WITH (NOLOCK) ON rc.PromotedBy = promotedby.EmployeeId
		LEFT JOIN dbo.Employee AS piby WITH (NOLOCK) ON rc.PayIncreasedBy = piby.EmployeeId
		LEFT JOIN dbo.Employee AS jliby WITH (NOLOCK) ON rc.JobLevelIncreasedBy = jliby.EmployeeId
	WHERE epa.[Status] = 1
		AND period.PeriodId = @PeriodId
		AND (
			rc.Promoted = 1
			OR rc.PayIncreased = 1
			OR rc.JobLevelIncreased = 1
			OR (
				period.TargetTypeId = 1 -- Probation
				AND rc.ProbationStatus IS NOT NULL
				)
			)
	ORDER BY emp.Code
END