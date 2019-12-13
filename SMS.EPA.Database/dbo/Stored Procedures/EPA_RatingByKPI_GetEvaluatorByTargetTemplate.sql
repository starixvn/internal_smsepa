
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
	SET NOCOUNT ON

	DECLARE @tblEPAIds IntId
		,@tblEvaluatorIds IntId
		,@Today DATE

	SET @Today = CAST(GETDATE() AS DATE)

	INSERT INTO @tblEPAIds (Id)
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 3
		AND CAST(pc.EndDate AS DATE) < @Today
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		AND dt.ProcessId = 3
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
	WHERE epa.[Status] = 0 -- open
		AND epa.HasMonthlyEPA = 0
		AND period.PeriodId = @PeriodId
		AND dt.TargetTemplateId = @TargetTemplateId

	INSERT INTO @tblEvaluatorIds (Id)
	SELECT DISTINCT dt.EvaluatorId
	FROM dbo.EPADetail AS dt WITH (NOLOCK)
		INNER JOIN @tblEPAIds AS epa ON dt.EPAId = epa.Id
	WHERE dt.ProcessId = 3

	SELECT DISTINCT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM @tblEvaluatorIds AS tbl
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON tbl.Id = emp.EmployeeId
	ORDER BY emp.NickName ASC
		,emp.Code ASC
END
