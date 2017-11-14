
CREATE PROCEDURE [dbo].[EpaPeriod_AddEmployee_CheckEmployeeExisted] @PeriodId INT
	,@EmployeeIds IntId READONLY
AS
/*
Created: 20160304@Tim.Trinh
Task: Check employee existed.
DB: SMSEPA
Original:

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ExistedEmployeeIds NVARCHAR(MAX)

	SELECT @ExistedEmployeeIds = COALESCE(@ExistedEmployeeIds + ',', '') + CAST(emp.EmployeeId AS NVARCHAR)
	FROM @EmployeeIds AS empS
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON empS.Id = emp.EmployeeId
	WHERE EXISTS (
			SELECT 1
			FROM dbo.PeriodFlowSetting AS peFS WITH (NOLOCK)
			WHERE peFS.EmployeeId = empS.Id
				AND @PeriodId = peFS.PeriodId
			)
		OR EXISTS (
			SELECT 1
			FROM dbo.PeriodTargetSetting AS peTS WITH (NOLOCK)
			WHERE peTS.EmployeeId = empS.Id
				AND @PeriodId = peTS.PeriodId
			)

	IF ISNULL(@ExistedEmployeeIds, '') <> ''
	BEGIN
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@ExistedEmployeeIds, ',')
	END
END