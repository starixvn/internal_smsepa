
CREATE PROCEDURE [dbo].[EPA_GetByReviewer] @ReviewerId INT
	,@PeriodId INT = NULL
	,@EmployeeIds VARCHAR(MAX) = NULL
	,@GroupId INT = NULL
	,@IsLocal BIT = NULL
	,@IsMonthly BIT = 0
	,@StatusProcess BIT = NULL
AS
/*
Created: 20150824@Tim.Trinh
Task: Get EPA by Reviewer.
DB: SMSEPA
Original:

Revisions:  
    20160229@Razor.Nguyen: add Monthly
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EPAIds IntId
		,@Today DATE = GETDATE()
		,@MonthlyTarget TINYINT = 2
		,@SemiTarget TINYINT = 3
		,@ReviewerProcessId TINYINT = 4
		,@ManagementProcessId TINYINT = 5

	CREATE TABLE #tblEmployee (EmployeeId INT)

	IF @EmployeeIds IS NOT NULL
	BEGIN
		INSERT INTO #tblEmployee (EmployeeId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@EmployeeIds, ',')
	END

	INSERT INTO @EPAIds
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EMployeeId = emp.EmployeeId
		INNER JOIN dbo.EPAProcess AS pcRev WITH (NOLOCK) ON epa.EPAId = pcRev.EPAId
		AND pcRev.ProcessId = @ReviewerProcessId
		AND pcRev.EvaluatorId = @ReviewerId
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
				AND @Today >= pcRev.StartDate
				AND (
					@Today < period.EndDate
					OR (
						@Today <= pc.EndDate
						AND pc.ProcessId IN (@ReviewerProcessId, @ManagementProcessId)
						)
					)
				)
			OR (
				tt.TargetTypeId NOT IN (@MonthlyTarget, @SemiTarget)
				AND @Today BETWEEN pc.StartDate
					AND pc.EndDate
				AND pc.ProcessId IN (@ReviewerProcessId, @ManagementProcessId)
				)
			)
		AND (
			@StatusProcess IS NULL
			OR (
				@StatusProcess = 0
				AND @Today BETWEEN pcRev.StartDate
					AND pcRev.EndDate
				)
			OR (
				@StatusProcess = 1
				AND @Today > pcRev.EndDate
				)
			)

	-- General Info
	EXECUTE dbo.EPA_GetGeneralInfo @EPAIds

	-- Process
	EXECUTE dbo.EPA_GetProcess @EPAIds

	DROP TABLE #tblEmployee
END