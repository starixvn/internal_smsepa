
/****** Object:  Stored Procedure dbo.EPA_GetByManagement    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetByManagement    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetByManagement    Script Date: 5/16/2016 1:59:44 PM ******/
CREATE PROCEDURE [dbo].[EPA_GetByManagement] @ManagementId INT
	,@EmployeeIds VARCHAR(MAX) = NULL
	,@GroupId INT = NULL
	,@IsLocal BIT = NULL
AS
/*
Created: 20150828@Tim.Trinh
Task: Get EPA by Management.
DB: SMSEPA
Original:

Revisions:
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
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 5 -- management
		AND pc.EvaluatorId = @ManagementId
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
				tt.TargetTypeId IN (2, 3) -- monthly, semi
				AND @ToDay >= CAST(pc.StartDate AS DATE)
				AND (
					@ToDay < CAST(period.EndDate AS DATE)
					OR (
						@ToDay <= CAST(pc.EndDate AS DATE)
						AND pc.ProcessId IN (5)
						)
					)
				)
			OR (
				tt.TargetTypeId NOT IN (2, 3)
				AND @ToDay BETWEEN CAST(pc.StartDate AS DATE)
					AND CAST(pc.EndDate AS DATE)
				AND pc.ProcessId IN (5)
				)
			)

	-- General Info
	EXECUTE dbo.EPA_GetGeneralInfo @EPAIds

	-- Process
	EXECUTE dbo.EPA_GetProcess @EPAIds
END
