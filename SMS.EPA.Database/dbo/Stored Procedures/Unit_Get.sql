
CREATE PROCEDURE [dbo].[Unit_Get] @IsActive BIT = 1
AS
/*
Created: 30082023@Tim.Trinh
Task: Get unit
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT u.UnitId
		,u.[Code]
		,u.[Name]
		,u.IsActive
	FROM dbo.Unit AS u WITH (NOLOCK)
	WHERE @IsActive IS NULL
		OR u.IsActive = @IsActive
END