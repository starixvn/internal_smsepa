/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_GetOtherViolations    Script Date: 5/16/2016 2:20:11 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_GetOtherViolations    Script Date: 5/16/2016 2:15:33 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_GetOtherViolations    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_GetOtherViolations    Script Date: 4/6/2016 11:27:42 AM ******/

CREATE PROCEDURE [dbo].[EpaAttendanceSummary_GetOtherViolations] @EmployeeIds VARCHAR(MAX)
	,@StartDate DATETIME
	,@EndDate DATETIME
	,@WorkDate DATETIME = NULL
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Add other violations to employee.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #Employee ([EmployeeId] INT)

	INSERT INTO #Employee ([EmployeeId])
	SELECT DISTINCT val
	FROM fnc_strSplit(@EmployeeIds, ',')

	SELECT [WorkDate]
		,[EmpId] AS EmployeeId
		,emp.[NickName]
		,[Name]
		,[Score]
		,[Comment]
	FROM [dbo].[EmployeeOtherViolation] AS empO WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS emp WITH (NOLOCK) ON empO.[EmpId] = emp.[EmployeeId]
	WHERE CAST([WorkDate] AS DATE) >= CAST(@StartDate AS DATE)
		AND CAST([WorkDate] AS DATE) <= CAST(@EndDate AS DATE)
		AND (
			ISNULL(@WorkDate, '') = ''
			OR [WorkDate] = @WorkDate
			)
		AND EXISTS (
			SELECT TOP 1 1
			FROM #Employee
			WHERE [EmployeeId] = empO.[EmpId]
			)
END