
CREATE PROCEDURE [dbo].[EpaAuto_GetEPAEndEvaluationPeriod]
AS
/*
Created: 20160329@Tim.Trinh
Task: Get EPA end evaluation period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TblEPAId IntId
		,@EPAIds NVARCHAR(MAX)
		,@Today DATE = GETDATE()
		,@Yesterday DATE
	-- target type
	DECLARE @MonthlyId INT = 2
		,@SemiId INT = 3

	CREATE TABLE #tblDisciplineScores (
		EPAId INT
		,SystemDisciplineScore FLOAT
		,DisciplineScore FLOAT
		,OtherDisciplines FLOAT
		)

	SET @Yesterday = DATEADD(DAY, - 1, @Today)

	INSERT INTO @TblEPAId (Id)
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS epaP WITH (NOLOCK) ON epa.EPAId = epaP.EPAId
		INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON epa.PeriodId = pe.PeriodId
	WHERE (
			(
				pe.EndDate = @Today
				AND epaP.EndDate < pe.EndDate
				)
			OR (
				epaP.EndDate = @Yesterday
				AND epaP.EndDate >= pe.EndDate
				)
			)
		AND ProcessId IN (
			SELECT MAX(ProcessId)
			FROM dbo.EPAProcess WITH (NOLOCK)
			WHERE EPAId = epaP.EPAId
			)
		AND pe.TargetTypeId IN (@MonthlyId, @SemiId)
		AND ISNULL(epa.[Status], 0) <> 2
		AND pe.IsActive = 1
	
	UNION
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPAProcess AS epaP WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epaP.EPAId = epa.EPAId
		INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
	WHERE epaP.EndDate = @Yesterday
		AND ProcessId IN (
			SELECT MAX(ProcessId)
			FROM dbo.EPAProcess WITH (NOLOCK)
			WHERE EPAId = epa.EPAId
			)
		AND pe.TargetTypeId NOT IN (@MonthlyId, @SemiId)
		AND ISNULL(epa.[Status], 0) <> 2
		AND pe.IsActive = 1
	
	UNION
	SELECT EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
	WHERE epa.[Status] = 1
		AND ISNULL(epa.FinalScore, '') = ''
		AND pe.EndDate < @Today
		AND pe.IsActive = 1

	SELECT @EPAIds = COALESCE(@EPAIds + ',', '') + CAST(Id AS NVARCHAR)
	FROM @TblEPAId

	INSERT INTO #tblDisciplineScores (
		EPAId
		,SystemDisciplineScore
		,DisciplineScore
		,OtherDisciplines
		)
	EXECUTE dbo.DisciplineSummary_GetByEPA @EPAIds

	-- General Info
	SELECT DISTINCT epa.EPAId
		,(CASE srt.ScoreRatingTypeId
			WHEN 1 THEN 1
			ELSE 0 END
			) AS HasDisciplineScore
		,(CASE 
			WHEN ds.DisciplineScore IS NULL
				OR ds.DisciplineScore < 0 THEN 0
			ELSE ds.DisciplineScore END
			) AS DisciplineScore
		,epa.[Status] AS StatusId
		,emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.Email
		,period.PeriodId
		,period.[Name]
		,tt.TargetTypeId
		,tt.[Name] AS TargetTypeName
	FROM @TblEPAId AS tbl
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.Id = epa.EPAId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
		LEFT JOIN #tblDisciplineScores AS ds WITH (NOLOCK) ON epa.EPAId = ds.EPAId
	ORDER BY epa.EPAId

	-- Process
	EXECUTE dbo.EPA_GetProcess @TblEPAId

	DROP TABLE #tblDisciplineScores
END