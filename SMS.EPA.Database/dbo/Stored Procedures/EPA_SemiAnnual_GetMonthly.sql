
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

	IF ISNULL(@EPAIds, '') <> ''
	BEGIN
		DECLARE @TblEPAId IntId
			,@MonthlyEPAId IntId
			,@ToDay DATE

		SET @ToDay = CAST(GETDATE() AS DATE)

		INSERT INTO @TblEPAId (Id)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@EPAIds, ',')

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
			,StartDateOfReviewer DATETIME
			,[Month] INT
			,[Year] INT
			)

		INSERT INTO #tblMonthlyEPA
		SELECT DISTINCT epa.EPAId
			,(CASE 
				WHEN CAST(pc.EndDate AS DATE) > CAST(period.EndDate AS DATE) THEN pc.EndDate
				ELSE period.EndDate END
				) AS EndDate
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.TargetTypeId = 2 -- monthly
			INNER JOIN (
			SELECT DISTINCT EPAId
				,MAX(EndDate) AS EndDate
			FROM dbo.EPAProcess WITH (NOLOCK)
			GROUP BY EPAId
			) AS pc ON epa.EPAId = pc.EPAId

		INSERT INTO #tblSemiEPA
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
				AND period.TargetTypeId = 3 -- semi
			) AS semiPC ON semi.EPAId = semiPC.EPAId
			INNER JOIN dbo.SemiEPADetail AS semiDt WITH (NOLOCK) ON semi.EPAId = semiDt.EPAId
			INNER JOIN dbo.Period AS semiPeriod WITH (NOLOCK) ON semi.PeriodId = semiPeriod.PeriodId
			AND semiPeriod.TargetTypeId = 3 -- semi

		INSERT INTO #tblEPA (
			EPAParentId
			,EPAId
			)
		SELECT DISTINCT semi.EPAId
			,monthly.EPAId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.TargetTypeId = 2 -- monthly
			INNER JOIN #tblMonthlyEPA AS monthly WITH (NOLOCK) ON epa.EPAId = monthly.EPAId
			INNER JOIN #tblSemiEPA AS semi WITH (NOLOCK) ON epa.EmployeeId = semi.EmployeeId
			AND MONTH(monthly.EndDate) = semi.[Month]
			AND YEAR(monthly.EndDate) = semi.[Year]

		INSERT INTO @MonthlyEPAId
		SELECT DISTINCT EPAId
		FROM #tblEPA

		-- General Info
		SELECT DISTINCT epa.EPAId
			,tbl.EPAParentId
			,epa.FinalScore
			,(CASE 
				WHEN tt.TargetTypeId IN (2, 3) THEN (CASE 
						WHEN CAST(period.EndDate AS DATE) >= CAST(period.EndDate AS DATE) THEN (CASE 
								WHEN CAST(period.EndDate AS DATE) < @ToDay THEN 1
								ELSE 0 END
								)
						ELSE (CASE 
								WHEN CAST(period.EndDate AS DATE) <= @ToDay THEN 1
								ELSE 0 END
								) END
						)
				ELSE (CASE 
						WHEN CAST(period.EndDate AS DATE) < @ToDay THEN 1
						ELSE 0 END
						) END
				) AS StatusId
			,emp.EmployeeId
			,emp.Code
			,emp.NickName
			,emp.FullName
			,emp.Email
			,CAST(lkd.Value AS TINYINT) AS RatingTypeId
			,lkd.Code
			,lkd.[Name]
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
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.IsActive = 1
			INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
			INNER JOIN dbo.LookupData AS lkd WITH (NOLOCK) ON epa.RatingTypeId = CAST(lkd.Value AS TINYINT)
			INNER JOIN dbo.[Lookup] AS lk WITH (NOLOCK) ON lkd.LookupId = lk.Id
			AND lk.Id = 8 -- rating type
			INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
		ORDER BY emp.NickName
			,emp.Code
			,period.StartDate

		-- Process
		EXECUTE dbo.EPA_GetProcess @MonthlyEPAId

		DROP TABLE #tblEPA

		DROP TABLE #tblMonthlyEPA

		DROP TABLE #tblSemiEPA
	END
END