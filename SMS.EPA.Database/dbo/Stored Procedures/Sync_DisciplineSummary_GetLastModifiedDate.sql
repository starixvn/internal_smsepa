
CREATE PROCEDURE [dbo].[Sync_DisciplineSummary_GetLastModifiedDate]
AS
/*
Created: 20160916@Quinn.Vo
Task: Get last modified date
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @ModifiedDate DATETIME

	SELECT @ModifiedDate  = ISNULL(MAX(ModifiedDate), '2015-01-01')
	FROM dbo.EmployeeDisciplineSummary WITH (NOLOCK)

	SELECT ISNULL(@ModifiedDate , '2015-01-01')  AS ModifiedDate
END