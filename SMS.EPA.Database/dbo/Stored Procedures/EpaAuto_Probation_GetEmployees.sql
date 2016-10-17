
CREATE PROCEDURE [dbo].[EpaAuto_Probation_GetEmployees]
AS
/*
Created: 2015-09-01@Josh.Phan
Task: Get employee that has probation.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ProbDay VARCHAR(50)
		,@PeriodId INT

	SELECT @PeriodId = MAX(PeriodId)
	FROM dbo.Period WITH (NOLOCK)
	WHERE TargetTypeId = 1 -- Probation

	SELECT @ProbDay = Value
	FROM dbo.LookupData
	WHERE [Id] = 7 -- Probation Setting Day

	SELECT emp.EmployeeId
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE CAST(DATEADD(DAY, CAST(@ProbDay AS INT), GETDATE()) AS DATE) = CAST(emp.ConfirmedDate AS DATE)
		AND ISNULL(emp.IsDeleted, 0) = 0
		AND emp.[Enabled] = 1
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodFlowSetting AS peFS WITH (NOLOCK)
			WHERE peFS.EmployeeId = emp.EmployeeId
				AND peFS.PeriodId = @PeriodId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodTargetSetting AS peTS WITH (NOLOCK)
			WHERE peTS.EmployeeId = emp.EmployeeId
				AND peTS.PeriodId = @PeriodId
			)

	SELECT @PeriodId
END