
CREATE PROCEDURE [dbo].[Sync_GroupSupervisor_GetMaxModifiedDate]
AS
/*
Created: 20150921@Quinn.Vo
Task: Get max modified date
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @ModifiedDate DATETIME

	SELECT @ModifiedDate  = ISNULL(MAX(ModifiedDate), '2010-01-01')
	FROM dbo.HRMGroupSupervisor WITH (NOLOCK)

	SELECT ISNULL(@ModifiedDate , '2010-01-01')  AS ModifiedDate
END