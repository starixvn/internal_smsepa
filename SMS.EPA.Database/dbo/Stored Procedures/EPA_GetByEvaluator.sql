
CREATE PROCEDURE [dbo].[EPA_GetByEvaluator] @EvaluatorId INT
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
		,@Employees IntId
		,@ToDay DATE

	SET @ToDay = CAST(GETDATE() AS DATE)

	IF ISNULL(@EmployeeIds, '') <> ''
	BEGIN
		INSERT INTO @Employees (Id)
		SELECT DISTINCT fnc_strSplit.val
		FROM dbo.fnc_strSplit(@EmployeeIds, ',')
	END

	INSERT INTO @EPAIds
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EMployeeId = emp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.EPAProcess AS pcEva WITH (NOLOCK) ON epa.EPAId = pcEva.EPAId
		AND pcEva.ProcessId = 3 -- evaluator
		AND pcEva.EvaluatorId = @EvaluatorId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
	WHERE epa.[Status] = 0 -- inprogress
		AND (
			@EmployeeIds IS NULL
			OR epa.EmployeeId IN (
				SELECT Id
				FROM @Employees
				)
			)
		AND (
			@GroupId IS NULL
			OR gr.GroupId = @GroupId
			)
		AND (
			@IsLocal IS NULL
			OR emp.IsLocal = @IsLocal
			)
		AND (
			(
				@IsMonthly = 1
				AND tt.TargetTypeId = 2
				)
			OR (
				@IsMonthly = 0
				AND tt.TargetTypeId <> 2
				)
			)
		AND (
			(
				tt.TargetTypeId IN (2, 3) -- monthly, semi
				AND @ToDay >= CAST(pcEva.StartDate AS DATE)
				AND (
					@ToDay < CAST(period.EndDate AS DATE)
					OR (
						@ToDay <= CAST(pc.EndDate AS DATE)
						AND pc.ProcessId IN (3, 4, 5)
						)
					)
				)
			OR (
				tt.TargetTypeId NOT IN (2, 3)
				AND @ToDay BETWEEN CAST(pc.StartDate AS DATE)
					AND CAST(pc.EndDate AS DATE)
				AND pc.ProcessId IN (3, 4, 5)
				)
			)
		AND (
			@StatusProcess IS NULL
			OR (
				@StatusProcess = 0
				AND @ToDay BETWEEN CAST(pcEva.StartDate AS DATE)
					AND CAST(pcEva.EndDate AS DATE)
				AND epa.HasMonthlyEPA = 0
				)
			OR (
				@StatusProcess = 1
				AND (
					@ToDay > CAST(pcEva.EndDate AS DATE)
					OR epa.HasMonthlyEPA = 1
					)
				)
			)

	-- General Info
	EXECUTE dbo.EPA_GetGeneralInfo @EPAIds

	-- Process
	EXECUTE dbo.EPA_GetProcess @EPAIds
END