/****** Object:  Stored Procedure dbo.Sync_EmployeeAttendance_GetMaxModifiedDate    Script Date: 7/21/2016 3:01:45 PM ******/
/****** Object:  Stored Procedure dbo.Sync_EmployeeAttendance_GetMaxModifiedDate    Script Date: 7/21/2016 2:45:59 PM ******/

CREATE PROCEDURE [dbo].[Sync_EmployeeAttendance_GetMaxModifiedDate]
AS
/*
Created: 20150828@Quinn.Vo
Task: Get max modified date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @ModifiedDate DATETIME

	SELECT @ModifiedDate  = ISNULL(MAX(ModifiedDate), '2015-01-01')
	FROM dbo.HRMEmployeeAttendance WITH (NOLOCK)

	SELECT ISNULL(@ModifiedDate , '2015-01-01')  AS ModifiedDate
END