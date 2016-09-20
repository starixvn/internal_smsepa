
CREATE PROCEDURE [dbo].[Sync_Department_GetMaxModifiedDate]
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

	SELECT @ModifiedDate = ISNULL(MAX(ModifiedDate), '2010-01-01')
	FROM dbo.Department WITH (NOLOCK)

	SELECT ISNULL(@ModifiedDate, '2010-01-01') AS ModifiedDate
END