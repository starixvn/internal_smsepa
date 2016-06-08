/****** Object:  Stored Procedure dbo.Sync_EmployeeAttendanceViolation_GetFromDate    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  Stored Procedure dbo.Sync_EmployeeAttendanceViolation_GetFromDate    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  Stored Procedure dbo.Sync_EmployeeAttendanceViolation_GetFromDate    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  Stored Procedure dbo.Sync_EmployeeAttendanceViolation_GetFromDate    Script Date: 4/6/2016 11:27:48 AM ******/

CREATE PROCEDURE [dbo].[Sync_EmployeeAttendanceViolation_GetFromDate]
AS
/*
Created: 20151109@Quinn.Vo
Task: Get from date to sync attendance violation.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @FromDate DATETIME
	,@CurrentDate DATETIME

	SELECT @CurrentDate = GETDATE()

	IF(NOT EXISTS(SELECT 1 FROM dbo.Period WITH (NOLOCK) 
		WHERE TargetTypeId = 3 
		AND @CurrentDate BETWEEN StartDate AND EndDate))
	BEGIN
		SELECT @FromDate = MAX(AttendanceEndDate)
		FROM dbo.Period WITH (NOLOCK)
		WHERE TargetTypeId = 3
			AND @CurrentDate > EndDate
		SELECT @FromDate = DATEADD(DD, 1, @FromDate)
	END
	ELSE
	BEGIN
		SELECT @FromDate = MIN(AttendanceStartDate)
		FROM dbo.Period WITH (NOLOCK)
		WHERE TargetTypeId = 3
			AND @CurrentDate BETWEEN StartDate AND EndDate
	END

	SELECT ISNULL(@FromDate , '2015-01-01')  AS FromDate
END