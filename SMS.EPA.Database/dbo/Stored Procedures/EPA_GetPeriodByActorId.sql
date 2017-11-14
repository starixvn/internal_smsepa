
CREATE PROCEDURE [dbo].[EPA_GetPeriodByActorId] @ActorId INT
	,@ProcessId TINYINT
AS
/*
Created: 20171114@Tim.Trinh
Task: Get Period by Evaluator/Reviewer/Management.
DB: SMSEPA
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@ProbationTargetType TINYINT = 1
		,@MonthlyTargetType TINYINT = 2
		,@SemiTargetType TINYINT = 3
		,@PromotionTargetType TINYINT = 5

	SELECT data.*
	FROM (
		SELECT DISTINCT period.PeriodId
			,period.[Name]
			,period.TargetTypeId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			AND pc.EvaluatorId = @ActorId
			AND pc.ProcessId = @ProcessId
			AND pc.StartDate <= @Today
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.IsActive = 1
		WHERE ISNULL(epa.[Status], 0) = 0 -- open
		) AS data
	ORDER BY CASE 
			WHEN data.TargetTypeId = @ProbationTargetType THEN 1
			WHEN data.TargetTypeId = @SemiTargetType THEN 2
			WHEN data.TargetTypeId = @PromotionTargetType THEN 3
			ELSE 4
			END
END