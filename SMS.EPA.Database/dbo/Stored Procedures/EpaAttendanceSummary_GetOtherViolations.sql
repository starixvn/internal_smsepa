CREATE PROCEDURE [dbo].[EpaAttendanceSummary_GetOtherViolations] @EmployeeId INT
	,@StartDate DATE
	,@EndDate DATE
	,@WorkDate DATE = NULL
AS
/*
Created: 2015-08-26@Josh.Phan
Task: get other violations to employee.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT dis.WorkDate
		,emp.EmployeeId
		,emp.NickName
		,dtype.[Name]
		,dtype.Score
		,dis.ApproverComment AS Comment
	FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
		INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON dis.DisciplineTypeId = dtype.DisciplineTypeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON dis.EmpId = emp.EmployeeId
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1
	WHERE dtype.DisciplineTypeId < 0
		AND dis.WorkDate BETWEEN @StartDate
			AND @EndDate
		AND (
			ISNULL(@WorkDate, '') = ''
			OR dis.WorkDate = @WorkDate
			)
END