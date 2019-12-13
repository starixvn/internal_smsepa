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

	DECLARE @Today DATE = GETDATE()

	SELECT epa.EPAId
		,pc.ProcessId
		,(CASE 
			WHEN pc.StartDate > @Today THEN 1
			WHEN @Today BETWEEN pc.StartDate
					AND pc.EndDate THEN 2
			WHEN pc.EndDate > @Today THEN 3 END
			) AS StatusId -- 1:Open; 2:In Progress; 3:Closed
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
	ORDER BY epa.EPAId ASC
		,pc.ProcessId ASC
		,pc.[Priority] ASC
END