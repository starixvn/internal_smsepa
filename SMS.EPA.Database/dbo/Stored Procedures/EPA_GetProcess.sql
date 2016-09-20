
CREATE PROCEDURE [dbo].[EPA_GetProcess] @EPAIds IntId READONLY
AS
/*
Created: 20150827@Tim.Trinh
Task: Get epa process by list epas.
DB: SPM
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ToDay DATE

	SET @ToDay = CAST(GETDATE() AS DATE)

	SELECT DISTINCT epa.EPAId
		,pc.ProcessId
		,pc.TotalScore
		,pc.AdjustScore
		,pc.Comment
		,pc.StartDate
		,pc.EndDate
		,(CASE 
			WHEN (
					pc.IsEndProcess = 0
					AND @ToDay BETWEEN CAST(pc.StartDate AS DATE)
						AND CAST(pc.EndDate AS DATE)
					) THEN 0 -- in progress
			WHEN pc.IsEndProcess = 1 THEN 1 -- end process
			WHEN (
					pc.IsEndProcess = 0
					AND CAST(pc.StartDate AS DATE) > @ToDay
					) THEN 2 END -- pending
			) AS StatusEndProcess
		,pc.IsEndProcess
		,pc.EvaluateDate
		,pc.[Priority]
		,(CASE 
			WHEN pc.ProcessId = 2 THEN srt.SelfEvaluated
			WHEN pc.ProcessId = 3 THEN srt.Evaluator
			WHEN pc.ProcessId = 4 THEN srt.Reviewer
			WHEN pc.ProcessId = 5 THEN srt.Management END
			) AS MaxTotalScore
		,(CASE 
			WHEN pc.ProcessId IN (2, 3) THEN (CASE 
					WHEN epa.HasMonthlyEPA = 1
						AND period.TargetTypeId = 3 THEN 1
					ELSE (CASE 
							WHEN tbl.NumKPINonEvaluated IS NOT NULL THEN 0
							ELSE 1 END
							) END
					)
			WHEN pc.ProcessId IN (4, 5) THEN (CASE 
					WHEN pc.EvaluateDate IS NOT NULL THEN 1
					ELSE 0 END
					) END
			) AS Evaluated
		,tbl.NumKPINonEvaluated
		,emp.EmployeeId AS EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM @EPAIds AS epat
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epat.Id = epa.EPAId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		LEFT JOIN (
		SELECT dt1.EPAId
			,dt1.ProcessId
			,dt1.EvaluatorId
			,COUNT(*) AS NumKPINonEvaluated
		FROM dbo.EPADetail AS dt1 WITH (NOLOCK)
			INNER JOIN @EPAIds AS epa1 ON dt1.EPAId = epa1.Id
		WHERE dt1.ProcessId IN (2, 3) -- self, evaluator
			AND dt1.Score IS NULL
		GROUP BY dt1.EPAId
			,dt1.ProcessId
			,dt1.EvaluatorId
		) AS tbl ON pc.EPAId = tbl.EPAId
		AND pc.ProcessId = tbl.ProcessId
		AND pc.EvaluatorId = tbl.EvaluatorId
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pc.EvaluatorId = emp.EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
	ORDER BY epa.EPAId
		,pc.ProcessId
		,pc.[Priority]
END