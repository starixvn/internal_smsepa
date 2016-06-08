/****** Object:  Stored Procedure dbo.EPA_GetInfoByEmployee    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetInfoByEmployee    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetInfoByEmployee    Script Date: 5/16/2016 1:59:44 PM ******/

CREATE PROCEDURE [dbo].[EPA_GetInfoByEmployee] @EPAId INT
	,@EmployeeId INT
AS
/*
Created: 20150811@Tim.Trinh
Task: Get EPA info.
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
	VALUES (
		2
		,@EmployeeId
		)

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