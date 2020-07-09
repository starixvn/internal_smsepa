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
		,@RequiredCommentOnMaxScoreForEvaluator BIT
		,@Today DATE = GETDATE()
		,@MonthlyTarget TINYINT = 2
		,@SemiTarget TINYINT = 3

	CREATE TABLE #tblDisciplineScores (
		EPAId INT
		,SystemDisciplineScore FLOAT
		,DisciplineScore FLOAT
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

	CREATE TABLE #tblEPAStatus (
		EPAId INT
		,StatusId TINYINT
		)

	SELECT @RequiredCommentOnMaxScore = CAST(Value AS BIT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'RequiredCommentOnMaxScore'

	SELECT @RequiredCommentOnMaxScoreForEvaluator = CAST(Value AS BIT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'RequiredCommentOnMaxScoreForEvaluator'

	SELECT @EPAIds = COALESCE(@EPAIds + ',', '') + CAST(Id AS NVARCHAR)
	FROM @TblEPAIds

	INSERT INTO #tblDisciplineScores (
		EPAId
		,SystemDisciplineScore
		,DisciplineScore
		,OtherDisciplines
		)
	EXECUTE dbo.DisciplineSummary_GetByEPA @EPAIds

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

	INSERT INTO #tblEPAStatus (
		EPAId
		,StatusId
		)
	SELECT epa.EPAId
		,(CASE 
			WHEN tt.TargetTypeId IN (@MonthlyTarget, @SemiTarget) THEN (CASE 
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
					WHEN pc.EndDate < @Today THEN 1
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
					WHEN ds.DisciplineScore IS NULL
						OR ds.DisciplineScore < 0 THEN 0
					ELSE ds.DisciplineScore END
					) END
			) AS DisciplineScore
		,(CASE 
			WHEN ds.SystemDisciplineScore IS NULL THEN 0
			ELSE ds.SystemDisciplineScore END
			) AS SystemDisciplineScore
		,srt.Attendance AS MaxDisciplineScore
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
		,@RequiredCommentOnMaxScoreForEvaluator AS RequiredCommentOnMaxScoreForEvaluator
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
		,ref.ResourceId AS RatingTypeId
		,ref.Value AS [Name]
		,period.PeriodId
		,period.[Name]
		,pc.StartDate
		,(CASE 
			WHEN tt.TargetTypeId IN (@MonthlyTarget, @SemiTarget) THEN CASE 
					WHEN pc.EndDate >= period.EndDate THEN pc.EndDate
					ELSE period.EndDate
					END
			ELSE pc.EndDate END
			) AS EndDate
		,tt.TargetTypeId
		,tt.[Name] AS TargetTypeName
		,ttem.Name as TargetTemplate
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
		INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON epa.RatingTypeId = ref.ResourceId
		AND ref.RefName = 'RatingType'
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
		INNER JOIN dbo.PeriodTargetSetting AS pertar WITH (NOLOCK) ON epa.PeriodId = pertar.PeriodId and epa.EmployeeId = pertar.EmployeeId
		INNER JOIN dbo.TargetTemplate AS ttem WITH (NOLOCK) ON ttem.TargetTemplateId = pertar.TargetTemplateId
		INNER JOIN #tblEPAStatus AS tblstatus WITH (NOLOCK) ON epa.EPAId = tblstatus.EPAId
		LEFT JOIN #tblDisciplineScores AS ds WITH (NOLOCK) ON epa.EPAId = ds.EPAId
		LEFT JOIN #tblLastEPA AS ls WITH (NOLOCK) ON epa.EPAId = ls.EPAId
		AND emp.Code = ls.Code
	ORDER BY emp.NickName ASC
		,emp.Code ASC

	DROP TABLE #tblDisciplineScores

	DROP TABLE #tblLastEPA

	DROP TABLE #tblEPAStatus
END