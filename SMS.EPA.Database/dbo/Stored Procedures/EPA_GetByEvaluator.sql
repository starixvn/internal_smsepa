

CREATE PROCEDURE [dbo].[EPA_GetByEvaluator] @EvaluatorId INT
	,@PeriodId INT = NULL
	,@EmployeeIds VARCHAR(MAX) = NULL
	,@GroupId INT = NULL
	,@IsLocal BIT = NULL
	,@IsMonthly BIT = 0
	,@StatusProcess BIT = NULL
AS
/*
Created: 20150817@Tim.Trinh
Task: Get EPA by Evaluator.
DB: SMSEPA
Original:

Revisions:  
    20160226@Razor.Nguyen: Monthly
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EPAIds IntId
		,@Today DATE = GETDATE()
		,@MonthlyTarget TINYINT = 2
		,@SemiTarget TINYINT = 3
		,@EvaluatorProcessId TINYINT = 3
		,@ReviewerProcessId TINYINT = 4
		,@ManagementProcessId TINYINT = 5

	CREATE TABLE #tblEmployee (EmployeeId INT)

	IF @EmployeeIds IS NOT NULL
	BEGIN
		INSERT INTO #tblEmployee (EmployeeId)
		SELECT DISTINCT fnc_strSplit.val
		FROM dbo.fnc_strSplit(@EmployeeIds, ',')
	END

	INSERT INTO @EPAIds
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EMployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND emp.ResignedDate IS NULL
		INNER JOIN dbo.EPAProcess AS pcEva WITH (NOLOCK) ON epa.EPAId = pcEva.EPAId
		AND pcEva.ProcessId = @EvaluatorProcessId
		AND pcEva.EvaluatorId = @EvaluatorId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN #tblEmployee AS tbl WITH (NOLOCK) ON emp.EmployeeId = tbl.EmployeeId
	WHERE ISNULL(epa.[Status], 0) = 0 -- open
		AND (
			@EmployeeIds IS NULL
			OR tbl.EmployeeId IS NOT NULL
			)
		AND ISNULL(@PeriodId, period.PeriodId) = period.PeriodId
		AND ISNULL(@GroupId, gr.GroupId) = gr.GroupId
		AND ISNULL(@IsLocal, emp.IsLocal) = emp.IsLocal
		AND (
			(
				@IsMonthly = 1
				AND tt.TargetTypeId = @MonthlyTarget
				)
			OR (
				@IsMonthly = 0
				AND tt.TargetTypeId <> @MonthlyTarget
				)
			)
		AND (
			(
				tt.TargetTypeId IN (@MonthlyTarget, @SemiTarget)
				AND @Today >= pcEva.StartDate
				AND (
					@Today < period.EndDate
					OR (
						@Today <= pc.EndDate
						AND pc.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId, @ManagementProcessId)
						)
					)
				)
			OR (
				tt.TargetTypeId NOT IN (@MonthlyTarget, @SemiTarget)
				AND @Today BETWEEN pc.StartDate
					AND pc.EndDate
				AND pc.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId, @ManagementProcessId)
				)
			)
		AND (
			@StatusProcess IS NULL
			OR (
				@StatusProcess = 0
				AND pcEva.EvaluateDate is null
				AND epa.HasMonthlyEPA = 0
				)
			OR (
				@StatusProcess = 1
				AND (
					pcEva.EvaluateDate is not null
					OR epa.HasMonthlyEPA = 1
					)
				)
			)

	-- General Info
	EXECUTE dbo.EPA_GetGeneralInfo @EPAIds

	-- Process
	EXECUTE dbo.EPA_GetProcess @EPAIds

	DROP TABLE #tblEmployee
END