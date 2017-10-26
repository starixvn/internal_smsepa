
CREATE PROCEDURE [dbo].[EpaAuto_Probation_GetEmployees]
AS
/*
Created: 20161031@Tim.Trinh
Task: Get employee that has probation.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @ProbDay INT
		,@PeriodId INT
		,@CurrentEvaluationTimes INT
		,@EmployeeIds IntId

	CREATE TABLE #EvaluationTimes (
		PeriodId INT
		,EmployeeId INT
		,Times INT
		)

	SELECT TOP 1 @PeriodId = PeriodId
	FROM dbo.Period WITH (NOLOCK)
	WHERE TargetTypeId = 1 -- Probation
		AND EvaluationTimes = 1 -- first
	ORDER BY CreatedDate ASC

	SELECT @CurrentEvaluationTimes = MAX(EvaluationTimes)
	FROM dbo.Period WITH (NOLOCK)
	WHERE TargetTypeId = 1 -- Probation

	SELECT @ProbDay = CAST(Value AS INT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'ProbationSettingDay'

	INSERT INTO @EmployeeIds (Id)
	SELECT DISTINCT emp.EmployeeId
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE emp.[Enabled] = 1
		AND ISNULL(emp.IsDeleted, 0) = 0
		AND CAST(DATEADD(DAY, @ProbDay, GETDATE()) AS DATE) = CAST(emp.ConfirmedDate AS DATE)

	INSERT INTO #EvaluationTimes (
		EmployeeId
		,Times
		)
	SELECT DISTINCT pfs.EmployeeId
		,MAX(pe.EvaluationTimes)
	FROM dbo.Period AS pe WITH (NOLOCK)
		INNER JOIN dbo.PeriodFlowSetting AS pfs WITH (NOLOCK) ON pe.PeriodId = pfs.PeriodId
		AND pfs.ProcessId = 2 -- self
		INNER JOIN @EmployeeIds AS emp ON pfs.EmployeeId = emp.Id
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
		AND epa.EmployeeId = emp.Id
		AND epa.[Status] <> 0 -- inprogress
	WHERE pe.TargetTypeId = 1 -- Probation
	GROUP BY pfs.EmployeeId

	-- insert period if any
	IF EXISTS (
			SELECT 1
			FROM #EvaluationTimes WITH (NOLOCK)
			WHERE Times = @CurrentEvaluationTimes
			)
	BEGIN
		DECLARE @EvaluationTimes INT

		SET @EvaluationTimes = @CurrentEvaluationTimes + 1

		EXEC dbo.EpaAuto_Probation_InsertPeriod @EvaluationTimes
	END

	UPDATE #EvaluationTimes WITH (ROWLOCK, UPDLOCK)
	SET PeriodId = (
			SELECT TOP 1 pe.PeriodId
			FROM dbo.Period AS pe WITH (NOLOCK)
			WHERE pe.EvaluationTimes = (Times + 1)
				AND pe.TargetTypeId = 1 -- Probation
			)
		,Times = Times + 1

	INSERT INTO #EvaluationTimes (
		PeriodId
		,EmployeeId
		,Times
		)
	SELECT @PeriodId
		,emp.Id
		,1
	FROM @EmployeeIds AS emp
	WHERE NOT EXISTS (
			SELECT 1
			FROM #EvaluationTimes WITH (NOLOCK)
			WHERE emp.Id = EmployeeId
			)

	SELECT DISTINCT t.PeriodId
		,t.EmployeeId
	FROM #EvaluationTimes AS t WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.Period AS pe WITH (NOLOCK)
				INNER JOIN dbo.PeriodFlowSetting AS pfs WITH (NOLOCK) ON pe.PeriodId = pfs.PeriodId
			WHERE pe.PeriodId = t.PeriodId
				AND pfs.EmployeeId = t.EmployeeId
			)

	DROP TABLE #EvaluationTimes
END