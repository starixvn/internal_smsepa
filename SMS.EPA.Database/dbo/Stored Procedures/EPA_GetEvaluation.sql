/****** Object:  Stored Procedure dbo.EPA_GetEvaluation    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetEvaluation    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetEvaluation    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetEvaluation    Script Date: 4/6/2016 11:27:41 AM ******/

CREATE PROCEDURE [dbo].[EPA_GetEvaluation] @EPAs IntId READONLY
	,@Processes ProcessTable READONLY
AS
/*
Created: 20150827@Tim.Trinh
Task: Get KPI by list epas.
DB: SPM
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT epa.EPAId
		,dt.ProcessId
		,dt.KPIGroupId
		,dt.KPIId
		,dt.KPIDisplayOrder AS DisplayOrder
		,dt.Score
		,dt.Rate
		,dt.IsMaxScore
		,dt.Comment AS Comment
		,emp.EmployeeId AS EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM @EPAs AS epat
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.Id = epa.EPAId
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON dt.EvaluatorId = emp.EmployeeId
		INNER JOIN dbo.KPIHistory AS kpihis WITH (NOLOCK) ON dt.KPIId = kpihis.KPIId
		AND dt.KPILogNo = kpihis.LogNo
		INNER JOIN @Processes AS pct ON dt.ProcessId = pct.ProcessId
		AND dt.EvaluatorId = pct.EvaluatorId
	ORDER BY dt.ProcessId
		,dt.KPIGroupId
		,dt.KPIDisplayOrder
END