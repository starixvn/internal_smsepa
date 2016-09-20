
CREATE PROCEDURE [dbo].[EpaJobTitle_Get]
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get job title.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT [Id] AS JobTitleId
		,[Name] AS JobTitleName
	FROM [dbo].[HRMJobTitle] WITH (NOLOCK)
END