CREATE PROCEDURE [dbo].[EPA_GetMember] @ActorUserId INT
	,@ProcessId INT
	,@GroupId INT = NULL
	,@IsLocal BIT = NULL
	,@IsMonthly BIT = NULL
AS
/*
Created: 20150818@Tim.Trinh
Task: Get Member by Evaluator or Reviewer or Management.
DB: SMSEPA
Original:

Revisions:  
20160406@Razor.nguyen: Monthly
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	SELECT DISTINCT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.[Enabled]
		,emp.Email
		,epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.EvaluatorId = @ActorUserId
		AND pc.ProcessId = @ProcessId
		AND pc.StartDate <= @Today
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON emp.GroupId = g.GroupId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
	WHERE epa.[Status] = 0
		AND ISNULL(@GroupId, g.GroupId) = g.GroupId
		AND ISNULL(@IsLocal, emp.IsLocal) = emp.IsLocal
		/*AND (
			(
				tt.TargetTypeId IN (2, 3) -- semi
				AND @Today >= pc.StartDate
				AND (
					@Today < period.EndDate
					OR (
						@Today <= pc.EndDate
						AND pc.ProcessId >= @ProcessId
						)
					)
				)
			OR (
				tt.TargetTypeId NOT IN (2, 3)
				AND @Today BETWEEN pc.StartDate
					AND pc.EndDate
				AND pc.ProcessId >= @ProcessId
				)
			)*/
		AND (
			@IsMonthly IS NULL
			OR (
				@IsMonthly = 1
				AND tt.TargetTypeId = 2
				)
			OR (
				@IsMonthly = 0
				AND tt.TargetTypeId <> 2
				)
			)
	ORDER BY emp.NickName
		,emp.Code
END