/****** Object:  Stored Procedure dbo.KPI_Get    Script Date: 7/21/2016 3:01:42 PM ******/
/****** Object:  Stored Procedure dbo.KPI_Get    Script Date: 7/21/2016 2:45:56 PM ******/

CREATE PROCEDURE [dbo].[KPI_Get]
     @KPIGroupId INT = NULL
	,@IsActive BIT = NULL
	,@IsRequiredEvaluate BIT = NULL
AS
/*
Created: 20150707@Tim.Trinh
Task: Get all KPI.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#KPI') IS NOT NULL
	BEGIN
		DROP TABLE #KPI;
	END;

	CREATE TABLE #KPI (KPIId INT);

	INSERT INTO #KPI
	SELECT DISTINCT k.KPIId
	FROM dbo.KPI AS k WITH (NOLOCK)
		INNER JOIN dbo.KPIDetail AS kg WITH (NOLOCK) ON k.KPIId = kg.KPIId
		INNER JOIN dbo.KPIGroup AS g WITH (NOLOCK) ON kg.KPIGroupId = g.KPIGroupId
	WHERE (
			@IsActive IS NULL
			OR k.IsActive = @IsActive
			)
	     AND
		  (
		     @IsRequiredEvaluate IS NULL
		     OR k.IsRequiredEvaluate = @IsRequiredEvaluate
		     )
		AND (
			@KPIGroupId IS NULL
			OR g.KPIGroupId = @KPIGroupId
			);

	SELECT DISTINCT k.*
	FROM dbo.KPI AS k WITH (NOLOCK)
		INNER JOIN #KPI AS tmp WITH (NOLOCK) ON k.KPIId = tmp.KPIId;

	SELECT k.KPIId
		,g.KPIGroupId
		,g.[Name]
	FROM #KPI AS k WITH (NOLOCK)
		INNER JOIN dbo.KPIDetail AS kg WITH (NOLOCK) ON k.KPIId = kg.KPIId
		INNER JOIN dbo.KPIGroup AS g WITH (NOLOCK) ON kg.KPIGroupId = g.KPIGroupId;

	DROP TABLE #KPI;
END;