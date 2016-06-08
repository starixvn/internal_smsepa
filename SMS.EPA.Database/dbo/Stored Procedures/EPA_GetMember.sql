/****** Object:  Stored Procedure dbo.EPA_GetMember    Script Date: 5/16/2016 2:20:11 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetMember    Script Date: 5/16/2016 2:15:33 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetMember    Script Date: 5/16/2016 1:59:44 PM ******/

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

	DECLARE @ToDay DATE

	SET @ToDay = CAST(GETDATE() AS DATE)

	SELECT DISTINCT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.[Enabled]
		,emp.Email
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.EvaluatorId = @ActorUserId
		AND pc.ProcessId = @ProcessId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON emp.GroupId = g.GroupId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
	WHERE (
			@GroupId IS NULL
			OR g.GroupId = @GroupId
			)
		AND (
			@IsLocal IS NULL
			OR emp.IsLocal = @IsLocal
			)
		AND (
			(
				tt.TargetTypeId IN (2,3) -- semi
				AND @ToDay >= CAST(pc.StartDate AS DATE)
				AND (
					@ToDay < CAST(period.EndDate AS DATE)
					OR (
						@ToDay <= CAST(pc.EndDate AS DATE)
						AND pc.ProcessId >= @ProcessId
						)
					)
				)
			OR (
				tt.TargetTypeId NOT IN (2,3)
				AND @ToDay BETWEEN CAST(pc.StartDate AS DATE)
					AND CAST(pc.EndDate AS DATE)
				AND pc.ProcessId >= @ProcessId
				)
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
	ORDER BY emp.NickName
		,emp.Code
END