
CREATE PROCEDURE [dbo].[KPIGroup_Get] @IsActive BIT = NULL
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
		,g.[Name]
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
	WHERE ISNULL(@IsActive, g.IsActive) = g.IsActive
END