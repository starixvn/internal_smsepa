/****** Object:  Stored Procedure dbo.TargetType_Get    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.TargetType_Get    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.TargetType_Get    Script Date: 5/16/2016 1:59:45 PM ******/
/****** Object:  Stored Procedure dbo.TargetType_Get    Script Date: 4/6/2016 11:27:49 AM ******/

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
	SET NOCOUNT ON;

	CREATE TABLE #TargetTypeId (TargetTypeId INT)

	IF ISNULL(@ExcludeTargetTypeIds, '') <> ''
	BEGIN
		INSERT INTO #TargetTypeId
		SELECT DISTINCT val
		FROM fnc_strSplit(@ExcludeTargetTypeIds, ',')
	END

	SELECT [TargetTypeId]
		,[Name]
	FROM dbo.TargetType WITH (NOLOCK)
	WHERE (
			@IsActive IS NULL
			OR IsActive = @IsActive
			)
		AND TargetTypeId NOT IN (
			SELECT DISTINCT TargetTypeId
			FROM #TargetTypeId
			)
	ORDER BY DisplayOrder

	DROP TABLE #TargetTypeId
END