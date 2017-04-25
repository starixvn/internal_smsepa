/****** Object:  Stored Procedure dbo.Sync_EmployeeCalendar_GetMaxModifiedDate    Script Date: 7/21/2016 3:01:45 PM ******/
/****** Object:  Stored Procedure dbo.Sync_EmployeeCalendar_GetMaxModifiedDate    Script Date: 7/21/2016 2:46:00 PM ******/

CREATE PROCEDURE [dbo].[Sync_EmployeeCalendar_GetMaxModifiedDate]
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

	SELECT @ModifiedDate  = MAX(ModifiedDate)
	FROM dbo.HRMEmployeeCalendar AS e WITH (NOLOCK)

	SELECT ISNULL(@ModifiedDate ,  '2015-01-01')  AS ModifiedDate
END