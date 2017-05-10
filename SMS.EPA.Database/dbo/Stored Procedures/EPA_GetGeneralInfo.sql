
CREATE PROCEDURE [dbo].[EPA_GetGeneralInfo] @TblEPAIds IntId READONLY
AS
/*
Created: 20150827@Tim.Trinh
Task: Get empoyee info by list epas.
DB: SMSEPA
Original: 

Revisions:  
    20160226@Razor.Nguyen: Monthly
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EPAIds NVARCHAR(MAX)
		,@RequiredCommentOnMaxScore BIT
		,@ToDay DATE

	CREATE TABLE #tblDisciplineScores (
		EPAId INT
		,SystemDisciplineScore FLOAT
		,ApproverSubtractScore FLOAT
		,OtherDisciplines FLOAT
		)

	CREATE TABLE #tblLastEPA (
		EPAId INT
		,LastEPAId INT
		,EmployeeId INT
		,Code NVARCHAR(20)
		,FinalScore NVARCHAR(100)
		,Grade NVARCHAR(100)
		,DisciplineScore FLOAT
		)

	CREATE TABLE #tblOtherViolation (
		EPAId INT
		,NumberOtherViolation INT
		)

	CREATE TABLE #tblEPAStatus (
		EPAId INT
		,StatusId TINYINT
		)

	SET @ToDay = CAST(GETDATE() AS DATE)

	SELECT @RequiredCommentOnMaxScore = CAST(lkd.Value AS BIT)
	FROM dbo.LookupData AS lkd WITH (NOLOCK)
		INNER JOIN dbo.[Lookup] AS lk WITH (NOLOCK) ON lkd.LookupId = lk.Id
		AND lk.Id = 7 -- required comment on max score

	SELECT @EPAIds = COALESCE(@EPAIds + ',', '') + CAST(Id AS NVARCHAR)
	FROM @TblEPAIds

	INSERT INTO #tblDisciplineScores (
		EPAId
		,SystemDisciplineScore
		,ApproverSubtractScore
		,OtherDisciplines
		)
	EXECUTE dbo.EmployeeDisciplineSummary_GetByEPA @EPAIds

	INSERT INTO #tblLastEPA (
		EPAId
		,LastEPAId
		,EmployeeId
		,Code
		,FinalScore
		,Grade
		,DisciplineScore
		)
	EXECUTE dbo.EPA_GetLastScore @TblEPAIds

	INSERT INTO #tblOtherViolation (
		EPAId
		,NumberOtherViolation
		)
	SELECT epa.EPAId
		,COUNT(1) AS NumberOtherViolation
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN @TblEPAIds AS epat ON epa.EPAId = epat.Id
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.EmployeeDiscipline AS dis WITH (NOLOCK) ON epa.EmployeeId = dis.EmpId
		AND CAST(dis.WorkDate AS DATE) BETWEEN CAST(period.AttendanceStartDate AS DATE)
			AND CAST(period.AttendanceEndDate AS DATE)
		INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON dis.DisciplineTypeId = dtype.DisciplineTypeId
		AND dtype.DisciplineTypeId < 0
	GROUP BY epa.EPAId

	INSERT INTO #tblEPAStatus (
		EPAId
		,StatusId
		)
	SELECT epa.EPAId
		,(CASE 
			WHEN tt.TargetTypeId IN (2, 3) THEN (CASE 
					WHEN CAST(pc.EndDate AS DATE) >= CAST(period.EndDate AS DATE) THEN (CASE 
							WHEN CAST(pc.EndDate AS DATE) < @ToDay THEN 1
							ELSE 0 END
							)
					ELSE (CASE 
							WHEN CAST(period.EndDate AS DATE) <= @ToDay THEN 1
							ELSE 0 END
							) END
					)
			ELSE (CASE 
					WHEN CAST(pc.EndDate AS DATE) < @ToDay THEN 1
					ELSE 0 END
					) END
			)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN @TblEPAIds AS epaId ON epa.EPAId = epaId.Id
		INNER JOIN (
		SELECT pc.EPAId
			,MAX(pc.EndDate) AS EndDate
		FROM dbo.EPAProcess AS pc WITH (NOLOCK)
			INNER JOIN @TblEPAIds AS epaId1 ON pc.EPAId = epaId1.Id
		GROUP BY pc.EPAId
		) AS pc ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId

	SELECT DISTINCT epa.EPAId
		,(CASE 
			WHEN tblstatus.StatusId = 1 THEN epa.FinalScore
			ELSE NULL END
			) AS FinalScore
		,(CASE 
			WHEN tblstatus.StatusId = 1 THEN epa.Grade
			ELSE NULL END
			) AS Grade
		,(CASE srt.ScoreRatingTypeId
			WHEN 1 THEN 1
			ELSE 0 END
			) AS HasDisciplineScore
		,(CASE 
			WHEN tblstatus.StatusId = 1 THEN epa.DisciplineScore
			ELSE (CASE 
					WHEN ds.ApproverSubtractScore IS NULL
						OR ds.ApproverSubtractScore < 0 THEN 0
					ELSE ds.ApproverSubtractScore END
					) END
			) AS DisciplineScore
		,(CASE 
			WHEN ds.SystemDisciplineScore IS NULL THEN 0
			ELSE ds.SystemDisciplineScore END
			) AS SystemDisciplineScore
		,srt.Attendance AS MaxDisciplineScore
		,ds.OtherDisciplines
		,(CASE 
			WHEN tbl1.NumberOtherViolation > 0 THEN 1
			ELSE 0 END
			) AS HasOtherViolation
		,ls.LastEPAId AS LastEPAId
		,ls.FinalScore AS LastFinalScore
		,epa.HasMonthlyEPA AS HasMonthlyEPA
		,ls.Grade AS LastGrade
		,ls.DisciplineScore AS LastDisciplineScore
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EPADetail AS dt WITH (NOLOCK)
					WHERE dt.EPAId = epa.EPAId
						AND dt.IsMaxScore = 1
					) THEN 1
			ELSE 0 END
			) AS HasMaxScoreOnKPI
		,@RequiredCommentOnMaxScore AS RequiredCommentOnMaxScore
		,tblstatus.StatusId
		,emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.Email
		,emp.JoinedDate
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,gr.GroupId
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,job.Id AS JobTitleId
		,job.[Name] AS JobTitleName
		,CAST(lkd.Value AS TINYINT) AS RatingTypeId
		,lkd.Code
		,lkd.[Name]
		,period.PeriodId
		,period.[Name]
		,CAST(pc.StartDate AS DATE) AS StartDate
		,(CASE 
			WHEN tt.TargetTypeId IN (2, 3) THEN CASE 
					WHEN CAST(pc.EndDate AS DATE) >= CAST(period.EndDate AS DATE) THEN CAST(pc.EndDate AS DATE)
					ELSE period.EndDate
					END
			ELSE CAST(pc.EndDate AS DATE) END
			) AS EndDate
		,tt.TargetTypeId
		,tt.[Name] AS TargetTypeName
		,srt.ScoreRatingTypeId
		,srt.[Name]
		,srt.SelfEvaluated
		,srt.Evaluator
		,srt.Reviewer
		,srt.Management
		,srt.Attendance
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN @TblEPAIds AS epaId ON epa.EPAId = epaId.Id
		INNER JOIN (
		SELECT pc1.EPAId
			,MIN(pc1.StartDate) AS StartDate
			,MAX(pc1.EndDate) AS EndDate
		FROM dbo.EPAProcess AS pc1 WITH (NOLOCK)
			INNER JOIN @TblEPAIds AS epaId1 ON pc1.EPAId = epaId1.Id
		GROUP BY pc1.EPAId
		) AS pc ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.[JobTitle] = job.[Id]
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
		INNER JOIN dbo.LookupData AS lkd WITH (NOLOCK) ON epa.RatingTypeId = CAST(lkd.Value AS TINYINT)
		INNER JOIN dbo.[Lookup] AS lk WITH (NOLOCK) ON lkd.LookupId = lk.Id
		AND lk.Id = 8 -- rating type
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
		INNER JOIN #tblEPAStatus AS tblstatus WITH (NOLOCK) ON epa.EPAId = tblstatus.EPAId
		LEFT JOIN #tblDisciplineScores AS ds ON epa.EPAId = ds.EPAId
		LEFT JOIN #tblLastEPA AS ls ON epa.EPAId = ls.EPAId
		AND emp.Code = ls.Code
		LEFT JOIN #tblOtherViolation AS tbl1 ON epa.EPAId = tbl1.EPAId
	ORDER BY emp.NickName
		,emp.Code

	DROP TABLE #tblDisciplineScores

	DROP TABLE #tblLastEPA

	DROP TABLE #tblOtherViolation

	DROP TABLE #tblEPAStatus
END