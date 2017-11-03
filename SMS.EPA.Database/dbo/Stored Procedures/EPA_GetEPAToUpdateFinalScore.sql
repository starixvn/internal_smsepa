
CREATE PROCEDURE [dbo].[EPA_GetEPAToUpdateFinalScore] @PeriodId INT
	,@DepartmentId INT = NULL
	,@IsLocal BIT = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 20160329@Tim.Trinh
Task: Get EPA to update final score.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

Edited: 
20170418@Hunter.Tran: Reformat, change GroupId to IntId User define table
20170508@Hunter.Tran: if EPA status is null, include in result
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TblEPAId IntId
		,@EPAIds NVARCHAR(MAX)
		,@Yesterday DATE = DATEADD(DAY, - 1, GETDATE())
		,@TargetTypeId INT
		,@Today DATE = GETDATE()
		,@GroupIds IntId
	-- target type
	DECLARE @ProbationId TINYINT = 1
		,@MonthlyId TINYINT = 2
		,@SemiId TINYINT = 3
		,@PromotionId TINYINT = 5

	CREATE TABLE #tblDisciplineScores (
		EPAId INT
		,SystemDisciplineScore FLOAT
		,DisciplineScore FLOAT
		,OtherDisciplines FLOAT
		)

	CREATE TABLE #tblEPAStatus (
		EPAId INT
		,StatusId TINYINT
		)

	IF ISNULL(@DepartmentId, '') <> ''
	BEGIN
		IF (@DepartmentId < 0)
		BEGIN
			INSERT INTO @GroupIds
			VALUES (ABS(@DepartmentId))
		END
		ELSE
		BEGIN
			INSERT INTO @GroupIds
			SELECT g.GroupId
			FROM dbo.[Group] AS g WITH (NOLOCK)
			WHERE (
					g.DepartmentId IN (
						SELECT DepartmentId
						FROM dbo.RelatedDepartment(@DepartmentId)
						
						UNION
						SELECT @DepartmentId
						)
					OR g.DepartmentId = @DepartmentId
					)
				AND g.IsDeleted = 0
		END
	END

	SELECT @TargetTypeId = TargetTypeId
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId
		AND IsActive = 1

	IF @TargetTypeId IN (@MonthlyId, @SemiId)
	BEGIN
		INSERT INTO @TblEPAId (Id)
		SELECT DISTINCT epa.EPAId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN (
			SELECT DISTINCT EPAId
				,MAX(EndDate) AS EndDate
			FROM dbo.EPAProcess WITH (NOLOCK)
			GROUP BY EPAId
			) AS pc ON epa.EPAId = pc.EPAId
			INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON epa.PeriodId = pe.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN @GroupIds AS tblgr ON gr.GroupId = tblgr.Id
		WHERE pe.EndDate <= @Today
			AND pc.EndDate < pe.EndDate
			AND pe.TargetTypeId IN (@MonthlyId, @SemiId)
			AND ISNULL(epa.[Status], 0) IN (0, 1)
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			AND (
				@DepartmentId IS NULL
				OR tblgr.Id IS NOT NULL
				)
			AND ISNULL(@IsLocal, emp.IsLocal) = emp.IsLocal
			AND ISNULL(@EmployeeId, emp.EmployeeId) = emp.EmployeeId
		
		UNION
		SELECT DISTINCT epa.EPAId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN (
			SELECT DISTINCT EPAId
				,MAX(EndDate) AS EndDate
			FROM dbo.EPAProcess WITH (NOLOCK)
			GROUP BY EPAId
			) AS pc ON epa.EPAId = pc.EPAId
			INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN @GroupIds AS tblgr ON gr.GroupId = tblgr.Id
		WHERE pc.EndDate <= @Yesterday
			AND pc.EndDate >= pe.EndDate
			AND pe.TargetTypeId IN (@MonthlyId, @SemiId)
			AND ISNULL(epa.[Status], 0) IN (0, 1)
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			AND (
				@DepartmentId IS NULL
				OR tblgr.Id IS NOT NULL
				)
			AND ISNULL(@IsLocal, emp.IsLocal) = emp.IsLocal
			AND ISNULL(@EmployeeId, emp.EmployeeId) = emp.EmployeeId
	END
	ELSE IF @TargetTypeId IN (@ProbationId, @PromotionId)
	BEGIN
		INSERT INTO @TblEPAId (Id)
		SELECT DISTINCT epa.EPAId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN (
			SELECT DISTINCT EPAId
				,MAX(EndDate) AS EndDate
			FROM dbo.EPAProcess WITH (NOLOCK)
			GROUP BY EPAId
			) AS pc ON epa.EPAId = pc.EPAId
			INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN @GroupIds AS tblgr ON gr.GroupId = tblgr.Id
		WHERE pc.EndDate <= @Yesterday
			AND pe.TargetTypeId IN (@ProbationId, @PromotionId)
			AND ISNULL(epa.[Status], 0) IN (0, 1)
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			AND (
				@DepartmentId IS NULL
				OR tblgr.Id IS NOT NULL
				)
			AND ISNULL(@IsLocal, emp.IsLocal) = emp.IsLocal
			AND ISNULL(@EmployeeId, emp.EmployeeId) = emp.EmployeeId
	END

	SELECT @EPAIds = COALESCE(@EPAIds + ',', '') + CAST(Id AS NVARCHAR)
	FROM @TblEPAId

	INSERT INTO #tblDisciplineScores (
		EPAId
		,SystemDisciplineScore
		,DisciplineScore
		,OtherDisciplines
		)
	EXECUTE dbo.DisciplineSummary_GetByEPA @EPAIds

	INSERT INTO #tblEPAStatus (
		EPAId
		,StatusId
		)
	SELECT epa.EPAId
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
					WHEN pc.EndDate < @Today THEN 1
					ELSE 0 END
					) END
			)
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN @TblEPAId AS epaId ON epa.EPAId = epaId.Id
		INNER JOIN (
		SELECT pc.EPAId
			,MAX(pc.EndDate) AS EndDate
		FROM dbo.EPAProcess AS pc WITH (NOLOCK)
			INNER JOIN @TblEPAId AS epaId1 ON pc.EPAId = epaId1.Id
		GROUP BY pc.EPAId
		) AS pc ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId

	-- General Info
	SELECT DISTINCT epa.EPAId
		,epa.FinalScore
		,epa.OldFinalScore
		,(CASE srt.ScoreRatingTypeId
			WHEN 1 THEN 1
			ELSE 0 END
			) AS HasDisciplineScore
		,(CASE 
			WHEN ds.DisciplineScore IS NULL
				OR ds.DisciplineScore < 0 THEN 0
			ELSE ds.DisciplineScore END
			) AS DisciplineScore
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
		,period.PeriodId
		,period.[Name]
		,tt.TargetTypeId
		,tt.[Name] AS TargetTypeName
	FROM @TblEPAId AS tbl
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.Id = epa.EPAId
		INNER JOIN #tblEPAStatus AS tblstatus WITH (NOLOCK) ON epa.EPAId = tblstatus.EPAId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
		LEFT JOIN #tblDisciplineScores AS ds WITH (NOLOCK) ON epa.EPAId = ds.EPAId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
	ORDER BY emp.NickName

	-- Process
	EXECUTE dbo.EPA_GetProcess @TblEPAId

	DROP TABLE #tblDisciplineScores

	DROP TABLE #tblEPAStatus
END