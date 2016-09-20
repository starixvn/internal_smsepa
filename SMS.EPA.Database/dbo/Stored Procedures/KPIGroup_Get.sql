/****** Object:  Stored Procedure dbo.KPIGroup_Get    Script Date: 7/21/2016 3:01:43 PM ******/
/****** Object:  Stored Procedure dbo.KPIGroup_Get    Script Date: 7/21/2016 2:45:57 PM ******/

CREATE PROCEDURE [dbo].[KPIGroup_Get] 
     @IsActive BIT = NULL
AS
/*
Created: 20150708@Tim.Trinh
Task: Get all KPI group.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT g.KPIGroupId
		,g.NAME
		,g.[Description]
		,k.NumberKPI
		,g.IsActive
		,g.CreatedBy
		,g.CreatedDate
		,g.ModifiedBy
		,g.ModifiedDate
	FROM dbo.KPIGroup AS g WITH (NOLOCK)
		LEFT JOIN (
		SELECT KPIGroupId
			,COUNT(k.KPIId) AS NumberKPI
		FROM dbo.KPI AS k WITH (NOLOCK)
			INNER JOIN dbo.KPIDetail AS kg WITH (NOLOCK) ON k.KPIId = kg.KPIId
		GROUP BY kg.KPIGroupId
		) AS k ON g.KPIGroupId = k.KPIGroupId
	WHERE (
			@IsActive IS NULL
			OR g.IsActive = @IsActive
			)
END