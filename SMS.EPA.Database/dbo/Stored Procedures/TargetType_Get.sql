
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

	CREATE TABLE #tblTargetTypeIds (Id TINYINT)

	IF ISNULL(@ExcludeTargetTypeIds, '') <> ''
	BEGIN
		INSERT INTO #tblTargetTypeIds (Id)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@ExcludeTargetTypeIds, ',')
	END

	SELECT TargetTypeId
		,[Name]
	FROM dbo.TargetType WITH (NOLOCK)
	WHERE ISNULL(@IsActive, IsActive) = IsActive
		AND TargetTypeId NOT IN (
			SELECT DISTINCT Id
			FROM #tblTargetTypeIds
			)
	ORDER BY DisplayOrder ASC

	DROP TABLE #tblTargetTypeIds
END