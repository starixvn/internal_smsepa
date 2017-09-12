CREATE PROCEDURE [dbo].[EPA_GetByReviewer] @ReviewerId INT
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
		,@Employees IntId
		,@Today DATE = GETDATE()

	IF ISNULL(@EmployeeIds, '') <> ''
	BEGIN
		INSERT INTO @Employees (Id)
		SELECT DISTINCT val
		FROM fnc_strSplit(@EmployeeIds, ',')
	END

	INSERT INTO @EPAIds
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EMployeeId = emp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.EPAProcess AS pcRev WITH (NOLOCK) ON epa.EPAId = pcRev.EPAId
		AND pcRev.ProcessId = 4 -- reviewer
		AND pcRev.EvaluatorId = @ReviewerId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
	WHERE ISNULL(epa.[Status], 0) = 0 -- inprogress
		AND (
			@EmployeeIds IS NULL
			OR epa.EmployeeId IN (
				SELECT Id
				FROM @Employees
				)
			)
		AND ISNULL(@GroupId, gr.GroupId) = gr.GroupId
		AND ISNULL(@IsLocal, emp.IsLocal) = emp.IsLocal
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
				AND @Today >= pcRev.StartDate
				AND (
					@Today < period.EndDate
					OR (
						@Today <= pc.EndDate
						AND pc.ProcessId IN (4, 5)
						)
					)
				)
			OR (
				tt.TargetTypeId NOT IN (2, 3)
				AND @Today BETWEEN pc.StartDate
					AND pc.EndDate
				AND pc.ProcessId IN (4, 5)
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
END