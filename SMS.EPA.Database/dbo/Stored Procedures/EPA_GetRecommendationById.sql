

CREATE PROCEDURE [dbo].[EPA_GetRecommendationById] @EPAId INT
AS
/*
Created: 20161010@Tim.Trinh
Task: Get recommendation by EPAId.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	SELECT epa.EPAId
		,re.ProcessId
		,re.ProbationStatus
		,re.Promoted
		,re.PayIncreased
		,re.JobLevelIncreased
		,CASE WHEN ISNULL(md.FullName, '') != '' AND ISNULL(md.NickName, '') != '' THEN md.FullName + ' (' + md.NickName + ')'
			ELSE ISNULL(md.FullName, md.NickName) END AS ModifiedBy
		,CASE WHEN ISNULL(promotedby.FullName, '') != '' AND ISNULL(promotedby.NickName, '') != '' THEN promotedby.FullName + ' (' + promotedby.NickName + ')'
			ELSE ISNULL(promotedby.FullName, promotedby.NickName) END AS PromotedBy
		,CASE WHEN ISNULL(piby.FullName, '') != '' AND ISNULL(piby.NickName, '') != '' THEN piby.FullName + ' (' + piby.NickName + ')'
			ELSE ISNULL(piby.FullName, piby.NickName) END AS PayIncreasedBy
		,CASE WHEN ISNULL(jliby.FullName, '') != '' AND ISNULL(jliby.NickName, '') != '' THEN jliby.FullName + ' (' + jliby.NickName + ')'
			ELSE ISNULL(jliby.FullName, jliby.NickName) END AS JobLevelIncreasedBy
		,job.JobLevelId
		,job.[Name] AS JobLevel
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPARecommendation AS re WITH (NOLOCK) ON epa.EPAId = re.EPAId
		LEFT JOIN dbo.JobLevel AS job WITH (NOLOCK) ON re.JobLevelId = job.JobLevelId
		LEFT JOIN dbo.Employee AS md WITH (NOLOCK) ON re.ModifiedBy = md.EmployeeId
		LEFT JOIN dbo.Employee AS promotedby WITH (NOLOCK) ON re.PromotedBy = promotedby.EmployeeId
		LEFT JOIN dbo.Employee AS piby WITH (NOLOCK) ON re.PayIncreasedBy = piby.EmployeeId
		LEFT JOIN dbo.Employee AS jliby WITH (NOLOCK) ON re.JobLevelIncreasedBy = jliby.EmployeeId
	WHERE epa.EPAId = @EPAId
END