
CREATE PROCEDURE [dbo].[EPA_GetEvaluator] @EPAs IntId READONLY
AS
/*
Created: 20150827@Tim.Trinh
Task: Get list Evaluator info by list epas.
DB: SPM
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ToDay DATE

	SET @ToDay = CAST(GETDATE() AS DATE)

	SELECT epa.EPAId
		,pc.ProcessId
		,(CASE 
			WHEN (
					pc.IsEndProcess = 0
					AND CAST(pc.StartDate AS DATE) <= @ToDay
					AND CAST(pc.EndDate AS DATE) >= @ToDay
					) THEN 0 -- in progress
			WHEN pc.IsEndProcess = 1 THEN 1 -- end process
			WHEN (
					pc.IsEndProcess = 0
					AND CAST(pc.StartDate AS DATE) > @ToDay
					) THEN 2 END -- pending
			) AS StatusEndProcess
		,pc.IsEndProcess
		,eva.EmployeeId AS EvaluatorId
		,eva.Code
		,eva.NickName AS NickName
		,eva.FullName AS FullName
		,eva.Email
		,pc.[Priority]
		,pc.StartDate
		,pc.EndDate
	FROM @EPAs AS epat
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.Id = epa.EPAId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON pc.EvaluatorId = eva.EmployeeId
	WHERE pc.ProcessId IN (1, 3, 4, 5)
	ORDER BY epa.EPAId
		,pc.ProcessId
		,pc.[Priority]
END
