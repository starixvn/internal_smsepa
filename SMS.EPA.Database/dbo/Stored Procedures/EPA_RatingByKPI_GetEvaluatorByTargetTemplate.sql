

CREATE PROCEDURE [dbo].[EPA_RatingByKPI_GetEvaluatorByTargetTemplate] @PeriodId INT
	,@TargetTemplateId INT
AS
/*
Created: 20160322@Razor.Nguyen
Task: Monthly EPA
DB: SMSEPA
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EPAIds IntId
		,@EvaluatorIds IntId

	INSERT INTO @EPAIds
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId IN (3)
		AND pc.IsEndProcess = 1
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId IN (3)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
	WHERE epa.[Status] = 0 -- open
		AND epa.HasMonthlyEPA = 0
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId

	INSERT INTO @EvaluatorIds
	SELECT DISTINCT dt.EvaluatorId
	FROM dbo.EPADetail AS dt WITH (NOLOCK)
		INNER JOIN @EPAIds AS epa ON dt.EPAId = epa.Id
	WHERE dt.ProcessId = 3

	SELECT DISTINCT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM @EvaluatorIds AS tbl
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON tbl.Id = emp.EmployeeId
	ORDER BY emp.NickName
		,emp.Code
END

