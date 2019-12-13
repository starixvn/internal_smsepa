
CREATE PROCEDURE [dbo].[Dashboard_GetToDoList] @EmployeeId INT
AS
/*
Created: 20160928@Tim.Trinh
Task: Get to do list.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @PeriodIds IntId
		,@Today DATE = GETDATE()
		,@ProcessBySelf TINYINT = 2
		,@ProcessByEvaluator TINYINT = 3
		,@ProcessByReviewer TINYINT = 4
		,@ProcessByManageMent TINYINT = 5
		,@Inprogress TINYINT = 0
		,@ProbationTargetType TINYINT = 1
		,@MonthlyTargetType TINYINT = 2
		,@SemiTargetType TINYINT = 3
		,@PromotionTargetType TINYINT = 5
	DECLARE @tblTodo TABLE (
		PeriodId INT
		,[Name] NVARCHAR(200)
		,TargetTypeId TINYINT
		,FinishedByEvaluator SMALLINT
		,UnfinishedByEvaluator SMALLINT
		,FinishedByReviewer SMALLINT
		,UnfinishedByReviewer SMALLINT
		,FinishedByManagement SMALLINT
		,UnfinishedByManagement SMALLINT
		)

	INSERT INTO @tblTodo (
		PeriodId
		,[Name]
		,TargetTypeId
		,FinishedByEvaluator
		,UnfinishedByEvaluator
		,FinishedByReviewer
		,UnfinishedByReviewer
		,FinishedByManagement
		,UnfinishedByManagement
		)
	SELECT period.PeriodId
		,period.[Name]
		,period.TargetTypeId
		,SUM(CASE 
				WHEN pc.ProcessId = @ProcessByEvaluator
					AND pc.EvaluateDate IS NOT NULL THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN pc.ProcessId = @ProcessByEvaluator
					AND pc.EvaluateDate IS NULL THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN pc.ProcessId = @ProcessByReviewer
					AND pc.EvaluateDate IS NOT NULL THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN pc.ProcessId = @ProcessByReviewer
					AND pc.EvaluateDate IS NULL THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN pc.ProcessId = @ProcessByManageMent
					AND pc.EvaluateDate IS NOT NULL THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN pc.ProcessId = @ProcessByManageMent
					AND pc.EvaluateDate IS NULL THEN 1
				ELSE 0
				END)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.EvaluatorId = @EmployeeId
		AND pc.ProcessId >= @ProcessByEvaluator
		AND @Today >= pc.StartDate
	WHERE ISNULL(epa.[Status], 0) = @Inprogress
	GROUP BY period.PeriodId
		,period.[Name]
		,period.TargetTypeId

	SELECT PeriodId
		,[Name]
		,TargetTypeId
		,FinishedByEvaluator
		,UnfinishedByEvaluator
		,FinishedByReviewer
		,UnfinishedByReviewer
		,FinishedByManagement
		,UnfinishedByManagement
	FROM @tblTodo
	ORDER BY CASE 
			WHEN TargetTypeId = @ProbationTargetType THEN 1
			WHEN TargetTypeId = @SemiTargetType THEN 2
			WHEN TargetTypeId = @PromotionTargetType THEN 3
			ELSE 4
			END ASC
END
