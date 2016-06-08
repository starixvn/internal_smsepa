/****** Object:  Stored Procedure dbo.EPA_GetInfoById    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetInfoById    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetInfoById    Script Date: 5/16/2016 1:59:45 PM ******/

CREATE PROCEDURE [dbo].[EPA_GetInfoById] @EPAId INT
AS
/*
Created: 20150904@Tim.Trinh
Task: Get EPA info by Id.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EPAIds IntId
		,@Processes ProcessTable

	INSERT INTO @EPAIds (Id)
	VALUES (@EPAId)

	INSERT INTO @Processes (
		ProcessId
		,EvaluatorId
		)
	SELECT DISTINCT pc.ProcessId
		,pc.EvaluatorId
	FROM @EPAIds AS epat
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.Id = epa.EPAId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId <> 1 -- ignore hr phase

	-- General Info
	EXECUTE dbo.EPA_GetGeneralInfo @EPAIds

	-- Process
	EXECUTE dbo.EPA_GetProcess @EPAIds

	-- KPI Group
	EXECUTE dbo.EPA_GetKPIGroup @EPAIds

	-- KPI
	EXECUTE dbo.EPA_GetKPI @EPAIds

	-- Evaluation per KPI
	EXECUTE EPA_GetEvaluation @EPAIds
		,@Processes

	-- Last performance score
	EXECUTE dbo.EPA_GetLastPerformanceScore @EPAId
END