/****** Object:  Stored Procedure dbo.Sync_EmployeeRequestHistory_GetMaxModifiedDate    Script Date: 7/21/2016 3:01:46 PM ******/
/****** Object:  Stored Procedure dbo.Sync_EmployeeRequestHistory_GetMaxModifiedDate    Script Date: 7/21/2016 2:46:00 PM ******/

CREATE PROCEDURE [dbo].[Sync_EmployeeRequestHistory_GetMaxModifiedDate]
AS
/*
Created: 20150909@Quinn.Vo
Task: Get max modified date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @ModifiedDate DATETIME

	SELECT @ModifiedDate  = ISNULL(MAX(InsertedDate), '2015-01-01')
	FROM dbo.HRMEmployeeRequestHistory WITH (NOLOCK)

	SELECT ISNULL(@ModifiedDate , '2015-01-01')  AS ModifiedDate
END