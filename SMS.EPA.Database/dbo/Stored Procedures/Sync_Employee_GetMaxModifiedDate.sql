
CREATE PROCEDURE [dbo].[Sync_Employee_GetMaxModifiedDate]
AS
/*
Created: 20150828@Quinn.Vo
Task: Get max modified date
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ModifiedDate DATETIME

	SELECT @ModifiedDate = ISNULL(MAX(ModifiedDate), '2010-01-01')
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE ISNULL(e.IsSpecialUser, 0) = 0

	SELECT ISNULL(@ModifiedDate, '2010-01-01') AS ModifiedDate
END