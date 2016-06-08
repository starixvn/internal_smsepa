/****** Object:  Stored Procedure dbo.Sync_Employee_GetMaxModifiedDate    Script Date: 5/16/2016 2:20:12 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Employee_GetMaxModifiedDate    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Employee_GetMaxModifiedDate    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.Sync_Employee_GetMaxModifiedDate    Script Date: 4/6/2016 11:27:48 AM ******/

CREATE PROCEDURE [dbo].[Sync_Employee_GetMaxModifiedDate]
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
	SELECT @ModifiedDate  = ISNULL(MAX(ModifiedDate), '2010-01-01')
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE ISNULL(e.IsSpecialUser, 0) = 0

	SELECT ISNULL(@ModifiedDate , '2010-01-01') AS ModifiedDate
END