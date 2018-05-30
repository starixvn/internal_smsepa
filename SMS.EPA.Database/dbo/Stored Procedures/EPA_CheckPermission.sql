
CREATE PROCEDURE [dbo].[EPA_CheckPermission] @EPAId INT
	,@ActorUserId INT
	,@ProcessId INT = NULL
AS
/*
Created: 20151013@Tim.Trinh
Task: Check permission.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Permission INT
		,@Today DATE = GETDATE()
		,@EmployeeId INT
		,@TargetTypeId TINYINT
		,@LastedStartDate DATE
	-- target
	DECLARE @SemiTargetId INT = 3

	IF EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
			WHERE epa.EPAId = @EPAId
				AND ISNULL(epa.[Status], 0) = 0 -- open
			)
		AND @ProcessId IS NOT NULL
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM dbo.EPA AS epa WITH (NOLOCK)
					INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
					AND pc.EvaluatorId = @ActorUserId
					AND pc.ProcessId = @ProcessId
					AND pc.StartDate <= @Today
					INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
					INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
					AND emp.[Enabled] = 1
					AND emp.IsDeleted = 0
					AND emp.ResignedDate IS NULL
				WHERE epa.EPAId = @EPAId
					AND (
						epa.HasMonthlyEPA = 0
						OR (
							epa.HasMonthlyEPA = 1
							AND period.TargetTypeId = @SemiTargetId
							AND pc.ProcessId > 3
							)
						)
				)
		BEGIN
			SET @Permission = 1
		END
		ELSE
		BEGIN
			SET @Permission = 0 -- don't permission
		END
	END
	ELSE
	BEGIN
		SELECT @EmployeeId = EmployeeId
			,@TargetTypeId = period.TargetTypeId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		WHERE epa.EPAId = @EPAId

		SELECT @LastedStartDate = MAX(process.StartDate)
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS process WITH (NOLOCK) ON epa.EPAId = process.EPAId
			AND process.ProcessId = 1 -- HR
			AND epa.EmployeeId = @EmployeeId
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.TargetTypeId = @TargetTypeId
		GROUP BY epa.EmployeeId

		IF @ProcessId IS NULL
			AND (
				EXISTS (
					SELECT 1
					FROM dbo.EmployeeRole AS erole WITH (NOLOCK)
						INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON erole.EmployeeId = emp.EmployeeId
						AND emp.EmployeeId = @ActorUserId
					WHERE erole.RoleId IN (1, 2) -- SA, HR
					)
				OR (
					EXISTS (
						SELECT 1
						FROM dbo.EmployeeRole AS erole WITH (NOLOCK)
							INNER JOIN dbo.Employee AS emp ON erole.EmployeeId = emp.EmployeeId
						WHERE emp.EmployeeId = @ActorUserId
							AND erole.RoleId IN (3, 4, 5) -- MGT, Reviewer, Evaluator
						)
					AND EXISTS (
						SELECT 1
						FROM dbo.EPA AS epa WITH (NOLOCK)
							INNER JOIN dbo.EPAProcess AS process WITH (NOLOCK) ON epa.EPAId = process.EPAId
							AND process.ProcessId > 2 -- self
							AND process.EvaluatorId = @ActorUserId
							INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
							AND period.TargetTypeId = @TargetTypeId
						WHERE epa.EmployeeId = @EmployeeId
							AND epa.EPAId IN (
								SELECT DISTINCT epa1.EPAId
								FROM dbo.EPA AS epa1 WITH (NOLOCK)
									INNER JOIN dbo.EPAProcess AS process1 WITH (NOLOCK) ON epa1.EPAId = process1.EPAId
									AND process1.ProcessId = 1 -- HR
									AND process1.StartDate = @LastedStartDate
									INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa1.PeriodId = period1.PeriodId
									AND period1.TargetTypeId = @TargetTypeId
								)
						)
					)
				OR EXISTS (
					SELECT 1
					FROM dbo.EPA AS epa WITH (NOLOCK)
						INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
						AND emp.EmployeeId = @ActorUserId
						INNER JOIN dbo.EmployeeRole AS erole WITH (NOLOCK) ON emp.EmployeeId = erole.EmployeeId
						AND erole.RoleId = 6 -- employee
					WHERE epa.EPAId = @EPAId
					)
				)
		BEGIN
			SET @Permission = 1
		END
		ELSE
		BEGIN
			SET @Permission = 0 -- don't permission
		END
	END

	SELECT @Permission
END