
CREATE PROCEDURE [dbo].[EpaRatingType_Get]
AS
/*
Created: 20151014@Josh.Phan
Task: Get rating type.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT ResourceId AS RatingTypeId
		,Value AS [Name]
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'RatingType'
END