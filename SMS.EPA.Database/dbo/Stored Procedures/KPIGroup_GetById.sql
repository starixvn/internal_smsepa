/****** Object:  Stored Procedure dbo.KPIGroup_GetById    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.KPIGroup_GetById    Script Date: 5/16/2016 2:15:32 PM ******/
/****** Object:  Stored Procedure dbo.KPIGroup_GetById    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.KPIGroup_GetById    Script Date: 4/6/2016 11:27:47 AM ******/

CREATE PROCEDURE [dbo].[KPIGroup_GetById] @KPIGroupId INT
AS
/*
Created: 20150710@Tim.Trinh
Task: Get KPI Group by Id.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @NumberKPI INT

	SELECT @NumberKPI = COUNT(k.KPIId)
	FROM dbo.KPI AS k WITH (NOLOCK)
		INNER JOIN dbo.KPIDetail AS kd WITH (NOLOCK) ON k.KPIId = kd.KPIId
	WHERE kd.KPIGroupId = @KPIGroupId

	SELECT g.KPIGroupId
		,g.NAME
		,g.Description
		,@NumberKPI AS NumberKPI
		,g.IsActive
	FROM dbo.KPIGroup AS g WITH (NOLOCK)
	WHERE g.KPIGroupId = @KPIGroupId
END