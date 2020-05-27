

CREATE PROCEDURE [dbo].[EPA_SemiAnnual_GetMonthly] @EPAIds NVARCHAR(MAX)
AS
/*
Created: 20150904@Tim.Trinh
Task: Get monthly of semi.
DB: SMSEPA
Original: 

Revisions:  
    20160606@Razor.Nguyen: Show EPA Monthly is Not finish
Param's Explanation:
    @EPAIds = '1,2,3'
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TblEPAId IntId
		,@MonthlyEPAId IntId
		,@Today DATE = GETDATE()
	-- target type
	DECLARE @MonthlyId INT = 2
		,@SemiId INT = 3

	IF @EPAIds IS NOT NULL
	BEGIN
		INSERT INTO @TblEPAId (Id)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@EPAIds, ',')
	END

	CREATE TABLE #tblEPA (
		EPAParentId INT
		,EPAId INT
		)

	CREATE TABLE #tblMonthlyEPA (
		EPAId INT
		,EndDate DATE
		)

	CREATE TABLE #tblSemiEPA (
		EPAId INT
		,EmployeeId INT
		,StartDateOfReviewer DATE
		,[Month] INT
		,[Year] INT
		)

	INSERT INTO #tblSemiEPA (
		EPAId
		,EmployeeId
		,StartDateOfReviewer
		,[Month]
		,[Year]
		)
	SELECT DISTINCT semi.EPAId
		,semi.EmployeeId
		,semiPC.StartDate
		,semiDt.[Month]
		,semiDt.[Year]
	FROM dbo.EPA AS semi WITH (NOLOCK)
		INNER JOIN @TblEPAId tblId ON semi.EPAId = tblId.Id
		INNER JOIN (
		SELECT DISTINCT pc.EPAId
			,pc.StartDate
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN @TblEPAId AS tblId1 ON epa.EPAId = tblId1.Id
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			AND pc.ProcessId = 4 -- reviewer
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.TargetTypeId = @SemiId
		) AS semiPC ON semi.EPAId = semiPC.EPAId
		INNER JOIN dbo.SemiEPADetail AS semiDt WITH (NOLOCK) ON semi.EPAId = semiDt.EPAId
		INNER JOIN dbo.Period AS semiPeriod WITH (NOLOCK) ON semi.PeriodId = semiPeriod.PeriodId
		AND semiPeriod.TargetTypeId = @SemiId

	INSERT INTO #tblMonthlyEPA (
		EPAId
		,EndDate
		)
	SELECT DISTINCT epa.EPAId
		,(CASE 
			WHEN pc.EndDate > period.EndDate THEN pc.EndDate
			ELSE period.EndDate END
			) AS EndDate
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.TargetTypeId = @MonthlyId
		INNER JOIN (
		SELECT DISTINCT epa1.EPAId
			,MAX(pc.EndDate) AS EndDate
		FROM dbo.EPAProcess AS pc WITH (NOLOCK)
			INNER JOIN dbo.EPA AS epa1 WITH (NOLOCK) ON pc.EPAId = epa1.EPAId
			INNER JOIN dbo.Period AS period1 WITH (NOLOCK) ON epa1.PeriodId = period1.PeriodId
			AND period1.TargetTypeId = @MonthlyId
		GROUP BY epa1.EPAId
		) AS pc ON epa.EPAId = pc.EPAId
	WHERE ISNULL(epa.[Status], 0) IN (0, 1)
		AND YEAR(pc.EndDate) IN (
			SELECT DISTINCT [Year]
			FROM #tblSemiEPA WITH (NOLOCK)
			)

	INSERT INTO #tblEPA (
		EPAParentId
		,EPAId
		)
	SELECT DISTINCT semi.EPAId
		,monthly.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.TargetTypeId = @MonthlyId
		INNER JOIN #tblMonthlyEPA AS monthly WITH (NOLOCK) ON epa.EPAId = monthly.EPAId
		INNER JOIN #tblSemiEPA AS semi WITH (NOLOCK) ON epa.EmployeeId = semi.EmployeeId
		AND MONTH(monthly.EndDate) = semi.[Month]
		AND YEAR(monthly.EndDate) = semi.[Year]

	INSERT INTO @MonthlyEPAId (Id)
	SELECT DISTINCT EPAId
	FROM #tblEPA WITH (NOLOCK)

	-- General Info
	SELECT DISTINCT epa.EPAId
		,tbl.EPAParentId
		,epa.FinalScore
		,(CASE 
			WHEN tt.TargetTypeId IN (@MonthlyId, @SemiId) THEN (CASE 
					WHEN pc.EndDate >= period.EndDate THEN (CASE 
							WHEN pc.EndDate < @Today THEN 1
							ELSE 0 END
							)
					ELSE (CASE 
							WHEN period.EndDate <= @Today THEN 1
							ELSE 0 END
							) END
					)
			ELSE (CASE 
					WHEN period.EndDate < @Today THEN 1
					ELSE 0 END
					) END
			) AS StatusId
		,emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.Email
		,ref.ResourceId AS RatingTypeId
		,ref.Value AS [Name]
		,period.PeriodId
		,period.[Name]
		,period.StartDate
		,period.EndDate
		,tt.TargetTypeId
		,tt.[Name] AS TargetTypeName
		,srt.ScoreRatingTypeId
		,srt.[Name]
		,srt.SelfEvaluated
		,srt.Evaluator
		,srt.Reviewer
		,srt.Management
		,srt.Attendance
	FROM #tblEPA AS tbl WITH (NOLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.EPAId = epa.EPAId
		INNER JOIN (
		SELECT pc.EPAId
			,MAX(pc.EndDate) AS EndDate
		FROM dbo.EPAProcess AS pc WITH (NOLOCK)
			INNER JOIN #tblEPA AS epaId WITH (NOLOCK) ON pc.EPAId = epaId.EPAId
		GROUP BY pc.EPAId
		) AS pc ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
		INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON epa.RatingTypeId = ref.ResourceId
		AND ref.RefName = 'RatingType'
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
	ORDER BY emp.NickName ASC
		,emp.Code ASC
		,period.StartDate ASC

	-- Process
	EXECUTE dbo.EPA_GetProcess @MonthlyEPAId

	DROP TABLE #tblEPA

	DROP TABLE #tblMonthlyEPA

	DROP TABLE #tblSemiEPA
END