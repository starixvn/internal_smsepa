/****** Object:  Stored Procedure dbo.EpaAuto_CheckEndEmployeeEvaluationPeriod    Script Date: 5/16/2016 2:20:08 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_CheckEndEmployeeEvaluationPeriod    Script Date: 5/16/2016 2:15:30 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_CheckEndEmployeeEvaluationPeriod    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_CheckEndEmployeeEvaluationPeriod    Script Date: 4/6/2016 11:27:43 AM ******/

CREATE PROCEDURE dbo.EpaAuto_CheckEndEmployeeEvaluationPeriod
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Check end evaluation period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Yesterday DATETIME

	SET @Yesterday = DATEADD(DAY, - 1, GETDATE())

	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
	WHERE CAST(EndDate AS DATE) = CAST(@Yesterday AS DATE)
		AND ProcessId IN (
			SELECT MAX(pc1.ProcessId)
			FROM dbo.EPAProcess AS pc1 WITH (NOLOCK)
			WHERE pc1.EPAId = epa.EPAId
			)
		AND epa.[Status] = 0
END