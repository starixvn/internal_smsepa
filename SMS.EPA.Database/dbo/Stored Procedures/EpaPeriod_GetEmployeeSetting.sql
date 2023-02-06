

CREATE PROCEDURE [dbo].[EpaPeriod_GetEmployeeSetting] @PeriodId INT
	,@TeamIds VARCHAR(MAX) = NULL
	,@IsLocal BIT = NULL
	,@IsExcludedEmployee BIT = NULL
	,@IsGeneratedEPA BIT = NULL
	,@IsEvaluated INT = NULL
	,@HasMonthlyEPA BIT = NULL
	,@EPAStatus INT = NULL
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get all employee setting of evaluation period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TargetTypeId INT
		,@Today DATE = GETDATE()

	CREATE TABLE #tmpTeam (TeamId INT)

	IF ISNULL(@TeamIds, '') != ''
	BEGIN
		INSERT INTO #tmpTeam (TeamId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
	END

	SELECT @TargetTypeId = TargetTypeId
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	SELECT DISTINCT pe.PeriodId
		,period.[Name]
		,(CASE 
			WHEN peEx.PeriodId IS NULL THEN 1
			ELSE 0 END
			) AS IsIncluded
		,dis.StartDate AS DisciplineStartDate
		,dis.EndDate AS DisciplineEndDate
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,section.Name AS Section
		,emp.JoinedDate
		,emp.HasMonthlyEPA
		,(CASE 
			WHEN emp.IsLocal = 1 THEN 'Local'
			ELSE 'Expat' END
			) AS Category
		,dept.DepartmentId
		,dept.[Name]
		,team.GroupId
		,team.[Name] AS GroupName
		,team.Code AS GroupCode
		,emp.JobTitle AS JobTitleId
		,job.[Name] AS JobTitleName
		,peTar.TargetTemplateId
		,tarTemp.[Name]
		,tar.TargetTypeId
		,tar.[Name] AS TargetTypeName
	FROM (
		SELECT DISTINCT pfs.PeriodId
			,pfs.EmployeeId
		FROM dbo.PeriodFlowSetting AS pfs WITH (NOLOCK)
		
		UNION
		SELECT pts.PeriodId
			,pts.EmployeeId
		FROM dbo.PeriodTargetSetting AS pts WITH (NOLOCK)
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.PeriodFlowSetting AS pft WITH (NOLOCK)
				WHERE pft.EmployeeId = pts.EmployeeId
					AND pft.PeriodId = pts.PeriodId
				)
		) AS pe
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pe.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pe.EmployeeId = emp.EmployeeId
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1
		AND emp.ResignedDate IS NULL
		INNER JOIN dbo.[Group] AS team WITH (NOLOCK) ON emp.GroupId = team.GroupId
		LEFT JOIN dbo.Department AS section WITH (NOLOCK) ON team.DepartmentId = section.DepartmentId
		AND section.IsActive = 1
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON section.ParentId = dept.DepartmentId
		AND dept.IsActive = 1
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
		LEFT JOIN dbo.PeriodTargetSetting AS peTar WITH (NOLOCK) ON pe.PeriodId = peTar.PeriodId
		AND pe.EmployeeId = peTar.EmployeeId
		LEFT JOIN dbo.TargetTemplate AS tarTemp WITH (NOLOCK) ON peTar.TargetTemplateId = tarTemp.TargetTemplateId
		LEFT JOIN dbo.TargetType AS tar WITH (NOLOCK) ON tarTemp.TargetTypeId = tar.TargetTypeId
		LEFT JOIN dbo.PeriodExcludedEmployee AS peEx WITH (NOLOCK) ON pe.PeriodId = peEx.PeriodId
		AND pe.EmployeeId = peEx.EmployeeId
		LEFT JOIN dbo.PeriodDisciplineSetting AS dis WITH (NOLOCK) ON period.PeriodId = dis.PeriodId
		AND emp.EmployeeId = dis.EmployeeId
	WHERE pe.PeriodId = @PeriodId
		AND period.TargetTypeId = @TargetTypeId
		AND (
			(
				@IsExcludedEmployee IS NULL
				OR @IsExcludedEmployee = 0
				)
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.PeriodExcludedEmployee WITH (NOLOCK)
				WHERE PeriodId = pe.PeriodId
					AND EmployeeId = pe.EmployeeId
				)
			OR (
				@IsExcludedEmployee = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.PeriodExcludedEmployee WITH (NOLOCK)
					WHERE PeriodId = pe.PeriodId
						AND EmployeeId = pe.EmployeeId
					)
				)
			)
		AND (
			ISNULL(@TeamIds, '') = ''
			OR EXISTS (
				SELECT 1
				FROM #tmpTeam AS tmp WITH (NOLOCK)
				WHERE team.GroupId = tmp.TeamId
				)
			)
		AND ISNULL(@IsLocal, emp.IsLocal) = emp.IsLocal
		AND (
			@IsGeneratedEPA IS NULL
			OR @IsGeneratedEPA = CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EPA WITH (NOLOCK)
					WHERE PeriodId = pe.PeriodId
						AND EmployeeId = emp.EmployeeId
					) THEN 1
			ELSE 0 END
			)
		AND (
			@IsEvaluated IS NULL
			OR (
				@IsEvaluated = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.EPA AS epa WITH (NOLOCK)
					WHERE epa.PeriodId = pe.PeriodId
						AND epa.EmployeeId = emp.EmployeeId
						AND EXISTS (
							SELECT 1
							FROM dbo.EPAProcess WITH (NOLOCK)
							WHERE EPAId = epa.EPAId
								AND ISNULL(TotalScore, '') = ''
								AND StartDate <= @Today
								AND ProcessId <> 1
							)
					)
				)
			OR (
				@IsEvaluated = 2
				AND EXISTS (
					SELECT 1
					FROM dbo.EPA AS epa WITH (NOLOCK)
					WHERE epa.PeriodId = pe.PeriodId
						AND epa.EmployeeId = emp.EmployeeId
						AND EXISTS (
							SELECT 1
							FROM dbo.EPAProcess WITH (NOLOCK)
							WHERE EPAId = epa.EPAId
								AND ISNULL(TotalScore, '') <> ''
							)
					)
				)
			)
		AND (
			ISNULL(@HasMonthlyEPA, 0) = 0
			OR emp.HasMonthlyEPA = @HasMonthlyEPA
			)
		AND (
			@EPAStatus IS NULL
			OR EXISTS (
				SELECT 1
				FROM dbo.EPA AS epa WITH (NOLOCK)
				WHERE epa.PeriodId = pe.PeriodId
					AND epa.EmployeeId = emp.EmployeeId
					AND epa.[Status] = @EPAStatus
				)
			)

	SELECT pfs.PeriodId
		,pfs.EmployeeId
		,pfs.ProcessId
		,pfs.StartDate
		,pfs.EndDate
		,(CASE 
			WHEN pfs.ProcessId = 2
				AND epaP.EPAId IS NOT NULL THEN 1
			ELSE 0 END
			) AS HasEvaluationGenerated
		,(CASE 
			WHEN epa.EPAId IS NOT NULL THEN CASE 
					WHEN pfs.StartDate > @Today THEN 1
					WHEN @Today BETWEEN pfs.StartDate
							AND pfs.EndDate THEN 2
					ELSE 3
					END
			ELSE 0 END
			) AS ProcessStatus -- 0:None, 1:Open, 2:In progress, 3:Closed
		,(CASE 
			WHEN epaP.EPAId IS NOT NULL THEN CASE 
					WHEN ISNULL(epaP.TotalScore, '') <> ''
						AND epaP.EvaluateDate IS NOT NULL THEN 2
					ELSE 1
					END
			ELSE 0 END
			) AS ProcessDetailStatus -- 1:None Evaluated, 2:Has Evaluated
		,eva.EmployeeId AS EvaluatorId
		,eva.NickName
		,eva.FullName
	FROM dbo.PeriodFlowSetting AS pfs WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pfs.PeriodId = period.PeriodId
		LEFT JOIN dbo.Employee AS eva WITH (NOLOCK) ON pfs.EvaluatorId = eva.EmployeeId
		LEFT JOIN dbo.EPA AS epa WITH (NOLOCK) ON pfs.PeriodId = epa.PeriodId
		AND pfs.EmployeeId = epa.EmployeeId
		LEFT JOIN dbo.EPAProcess AS epaP WITH (NOLOCK) ON epa.EPAId = epaP.EPAId
		AND pfs.ProcessId = epaP.ProcessId
		AND pfs.EvaluatorId = epaP.EvaluatorId
	WHERE pfs.PeriodId = @PeriodId
		AND period.TargetTypeId = @TargetTypeId
		AND (
			@IsGeneratedEPA IS NULL
			OR @IsGeneratedEPA = CASE 
			WHEN epaP.EPAId IS NOT NULL THEN 1
			ELSE 0 END
			)
		AND (
			@IsEvaluated IS NULL
			OR (
				@IsEvaluated = 1
				AND epa.EPAId IS NOT NULL
				AND EXISTS (
					SELECT 1
					FROM dbo.EPAProcess WITH (NOLOCK)
					WHERE EPAId = epa.EPAId
						AND ISNULL(TotalScore, '') = ''
						AND StartDate <= @Today
						AND ProcessId <> 1
					)
				)
			OR (
				@IsEvaluated = 2
				AND epa.EPAId IS NOT NULL
				AND EXISTS (
					SELECT 1
					FROM dbo.EPAProcess WITH (NOLOCK)
					WHERE EPAId = epa.EPAId
						AND ISNULL(TotalScore, '') <> ''
					)
				)
			)
		AND (
			@EPAStatus IS NULL
			OR EXISTS (
				SELECT 1
				FROM dbo.EPA AS epa WITH (NOLOCK)
				WHERE epa.PeriodId = period.PeriodId
					AND epa.EmployeeId = pfs.EmployeeId
					AND epa.[Status] = @EPAStatus
				)
			)

	DROP TABLE #tmpTeam
END