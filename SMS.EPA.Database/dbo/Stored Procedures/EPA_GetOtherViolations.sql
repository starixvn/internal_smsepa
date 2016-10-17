
CREATE PROCEDURE [dbo].[EPA_GetOtherViolations] @EPAId INT
AS
/*
Created: 20151113@Tim.Trinh
Task: Get other violations detail by EPAId.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT dis.WorkDate
		,dtype.[Name]
		,dtype.Score
		,dis.ApproverComment AS Comment
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND ISNULL(emp.IsDeleted, 0) = 0
		AND emp.[Enabled] = 1
		INNER JOIN dbo.EmployeeDiscipline AS dis WITH (NOLOCK) ON emp.EmployeeId = dis.EmpId
		AND ISNULL(dis.IsDeleted, 0) = 0
		AND CAST(dis.WorkDate AS DATE) BETWEEN CAST(period.AttendanceStartDate AS DATE)
			AND CAST(period.AttendanceEndDate AS DATE)
		INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON dis.DisciplineTypeId = dtype.DisciplineTypeId
		AND dtype.DisciplineTypeId < 0
	WHERE epa.EPAId = @EPAId
	ORDER BY dis.WorkDate
END