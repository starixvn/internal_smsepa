
/****** Object:  Stored Procedure dbo.EPA_GetEPAToUpdateFinalScore    Script Date: 7/21/2016 2:45:43 PM ******/
CREATE PROCEDURE [dbo].[EPA_GetEPAToUpdateFinalScore] @PeriodId INT
	,@GroupId INT = NULL
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
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TblEPAId IntId
		,@EPAIds NVARCHAR(MAX)
		,@Yesterday DATE
		,@TargetTypeId INT
		,@ToDay DATE

	CREATE TABLE #DisciplineScores (
		EPAId INT
		,SystemDisciplineScore FLOAT
		,DisciplineScore FLOAT
		,OtherDisciplines FLOAT
		)

	SET @ToDay = CAST(GETDATE() AS DATE)
	SET @Yesterday = DATEADD(DAY, - 1, GETDATE())

	SELECT @TargetTypeId = TargetTypeId
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId
		AND IsActive = 1

	IF @TargetTypeId IN (2, 3)
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
		WHERE CAST(pe.EndDate AS DATE) <= CAST(GETDATE() AS DATE)
			AND CAST(pc.EndDate AS DATE) < CAST(pe.EndDate AS DATE)
			AND pe.TargetTypeId IN (2, 3) -- monthly, semi annual
			AND epa.[Status] IN (0, 1) -- open/closed
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			AND (
				ISNULL(@GroupId, '') = ''
				OR gr.GroupId = @GroupId
				)
			AND (
				@IsLocal IS NULL
				OR emp.IsLocal = @IsLocal
				)
			AND (
				ISNULL(@EmployeeId, '') = ''
				OR emp.EmployeeId = @EmployeeId
				)
		
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
		WHERE CAST(pc.EndDate AS DATE) <= @Yesterday
			AND CAST(pc.EndDate AS DATE) >= CAST(pe.EndDate AS DATE)
			AND pe.TargetTypeId IN (2, 3) -- monthly, semi annul
			AND epa.[Status] IN (0, 1) -- open/closed
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			AND (
				ISNULL(@GroupId, '') = ''
				OR gr.GroupId = @GroupId
				)
			AND (
				@IsLocal IS NULL
				OR emp.IsLocal = @IsLocal
				)
			AND (
				ISNULL(@EmployeeId, '') = ''
				OR emp.EmployeeId = @EmployeeId
				)
	END
	ELSE IF @TargetTypeId IN (1, 5) -- probation/promotion
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
		WHERE CAST(pc.EndDate AS DATE) <= @Yesterday
			AND pe.TargetTypeId IN (1, 5) -- probation/promotion
			AND epa.[Status] IN (0, 1) -- open/closed
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			AND (
				ISNULL(@GroupId, '') = ''
				OR gr.GroupId = @GroupId
				)
			AND (
				@IsLocal IS NULL
				OR emp.IsLocal = @IsLocal
				)
			AND (
				ISNULL(@EmployeeId, '') = ''
				OR emp.EmployeeId = @EmployeeId
				)
	END

	SELECT @EPAIds = COALESCE(@EPAIds + ',', '') + CAST(Id AS NVARCHAR)
	FROM @TblEPAId

	INSERT INTO #DisciplineScores
	EXECUTE dbo.EmployeeDisciplineSummary_GetByEPA @EPAIds

	-- General Info
	SELECT DISTINCT epa.EPAId
		,epa.FinalScore
		,epa.OldFinalScore
		,(CASE srt.ScoreRatingTypeId
			WHEN 1 THEN 1
			ELSE 0 END
			) AS HasDisciplineScore
		,(CASE 
			WHEN ds.DisciplineScore IS NULL THEN 0
			ELSE ds.DisciplineScore END
			) AS DisciplineScore
		,epa.[Status] AS StatusId
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
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
		LEFT JOIN #DisciplineScores AS ds ON epa.EPAId = ds.EPAId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
	ORDER BY emp.NickName

	-- Process
	EXECUTE dbo.EPA_GetProcess @TblEPAId

	DROP TABLE #DisciplineScores
END