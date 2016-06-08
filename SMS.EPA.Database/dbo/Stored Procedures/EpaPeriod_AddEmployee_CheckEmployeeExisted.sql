/****** Object:  Stored Procedure dbo.EpaPeriod_AddEmployee_CheckEmployeeExisted    Script Date: 5/16/2016 2:20:11 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_AddEmployee_CheckEmployeeExisted    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_AddEmployee_CheckEmployeeExisted    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_AddEmployee_CheckEmployeeExisted    Script Date: 4/6/2016 11:27:45 AM ******/

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
			FROM dbo.PeriodFlowSetting AS peFS
			WHERE peFS.EmployeeId = empS.Id
				AND @PeriodId = peFS.PeriodId
			)
		OR EXISTS (
			SELECT 1
			FROM dbo.PeriodTargetSetting AS peTS
			WHERE peTS.EmployeeId = empS.Id
				AND @PeriodId = peTS.PeriodId
			)

	IF ISNULL(@ExistedEmployeeIds, '') <> ''
	BEGIN
		SELECT DISTINCT val
		FROM fnc_strSplit(@ExistedEmployeeIds, ',')
	END
END