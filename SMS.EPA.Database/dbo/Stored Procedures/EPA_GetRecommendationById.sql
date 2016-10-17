
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
		,re.ProbationStatus
		,re.Promoted
		,re.PayIncreased
		,re.JobGradeIncreased
		,job.JobGradeId
		,job.[Name]
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPARecommendation AS re WITH (NOLOCK) ON epa.EPAId = re.EPAId
		LEFT JOIN dbo.JobGrade AS job WITH (NOLOCK) ON re.JobGradeId = job.JobGradeId
	WHERE epa.EPAId = @EPAId
END