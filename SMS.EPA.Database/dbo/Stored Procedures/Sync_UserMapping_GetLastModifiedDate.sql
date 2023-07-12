
CREATE PROCEDURE [dbo].[Sync_UserMapping_GetLastModifiedDate]
AS
/*
Created: 20221129@Otis.vu
Task: get last modifiedDate
DB: EPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT MAX(UC_ModifiedDate)
	FROM dbo.Employee WITH (NOLOCK)
	WHERE IsSpecialUser = 1
END