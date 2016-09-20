
CREATE PROCEDURE [dbo].[TargetType_Get] @ExcludeTargetTypeIds NVARCHAR(200) = NULL
	,@IsActive BIT = 1
AS
/*
Created: 20150709@Tim.Trinh
Task: Get all Target type.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @TargetTypeIds IntId

	IF ISNULL(@ExcludeTargetTypeIds, '') <> ''
	BEGIN
		INSERT INTO @TargetTypeIds (Id)
		SELECT DISTINCT val
		FROM fnc_strSplit(@ExcludeTargetTypeIds, ',')
	END

	SELECT TargetTypeId
		,[Name]
	FROM dbo.TargetType WITH (NOLOCK)
	WHERE (
			@IsActive IS NULL
			OR IsActive = @IsActive
			)
		AND TargetTypeId NOT IN (
			SELECT DISTINCT Id
			FROM @TargetTypeIds
			)
	ORDER BY DisplayOrder
END