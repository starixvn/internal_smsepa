/****** Object:  Stored Procedure dbo.Sync_EmployeeRequestHistory_GetMaxModifiedDate    Script Date: 5/16/2016 2:20:12 PM ******/
/****** Object:  Stored Procedure dbo.Sync_EmployeeRequestHistory_GetMaxModifiedDate    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.Sync_EmployeeRequestHistory_GetMaxModifiedDate    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.Sync_EmployeeRequestHistory_GetMaxModifiedDate    Script Date: 4/6/2016 11:27:48 AM ******/

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