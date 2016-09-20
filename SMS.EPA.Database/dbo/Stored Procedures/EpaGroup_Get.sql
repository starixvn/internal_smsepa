
CREATE PROCEDURE [dbo].[EpaGroup_Get]
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get group.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;	

	SELECT 
		[GroupId]
		,[Name] AS GroupName
		,[Code] AS GroupCode
	FROM [dbo].[Group] WITH (NOLOCK)
	WHERE [IsDeleted] = 0		
END