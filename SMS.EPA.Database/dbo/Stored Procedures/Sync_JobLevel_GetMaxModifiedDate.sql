CREATE PROCEDURE [dbo].[Sync_JobLevel_GetMaxModifiedDate]
AS
/*
Created: 20210426@Lilac.Nguyen
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
	FROM dbo.JobLevel WITH (NOLOCK)

	SELECT ISNULL(@ModifiedDate, '2010-01-01') AS ModifiedDate
END