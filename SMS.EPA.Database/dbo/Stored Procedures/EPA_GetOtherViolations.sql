/****** Object:  Stored Procedure dbo.EPA_GetOtherViolations    Script Date: 5/16/2016 2:20:08 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetOtherViolations    Script Date: 5/16/2016 2:15:30 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetOtherViolations    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetOtherViolations    Script Date: 4/6/2016 11:27:41 AM ******/

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

	SELECT DISTINCT ov.WorkDate
		,ov.[Name]
		,- ov.Score AS Score
		,ov.Comment
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN [dbo].[EmployeeOtherViolation] AS ov WITH (NOLOCK) ON emp.EmployeeId = ov.EmpId
		AND CAST(ov.WorkDate AS DATE) BETWEEN CAST(period.AttendanceStartDate AS DATE)
			AND CAST(period.AttendanceEndDate AS DATE)
	WHERE epa.EPAId = @EPAId
END