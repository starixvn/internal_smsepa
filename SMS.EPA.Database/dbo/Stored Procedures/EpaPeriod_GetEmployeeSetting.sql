/****** Object:  Stored Procedure dbo.EpaPeriod_GetEmployeeSetting    Script Date: 5/16/2016 2:20:08 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_GetEmployeeSetting    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_GetEmployeeSetting    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaMonthlyPeriod_Get    Script Date: 4/6/2016 11:27:45 AM ******/

CREATE PROCEDURE [dbo].[EpaPeriod_GetEmployeeSetting] @PeriodId INT
	,@DepartmentId INT = NULL
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

	SELECT @TargetTypeId = TargetTypeId
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	SELECT DISTINCT pe.PeriodId
		,period.[Name]
		,(CASE 
			WHEN peEx.PeriodId IS NULL THEN 1
			ELSE 0 END
			) AS IsIncluded
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,emp.JoinedDate
		,emp.HasMonthlyEPA
		,lkd.[Name] AS Category
		,dept.DepartmentId
		,dept.[Name]
		,gr.GroupId
		,gr.[Name] AS GroupName
		,gr.Code AS GroupCode
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
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
		LEFT JOIN dbo.PeriodTargetSetting AS peTar WITH (NOLOCK) ON pe.PeriodId = peTar.PeriodId
		AND pe.EmployeeId = peTar.EmployeeId
		LEFT JOIN dbo.TargetTemplate AS tarTemp WITH (NOLOCK) ON peTar.TargetTemplateId = tarTemp.TargetTemplateId
		LEFT JOIN dbo.TargetType AS tar WITH (NOLOCK) ON tarTemp.TargetTypeId = tar.TargetTypeId
		LEFT JOIN dbo.PeriodExcludedEmployee AS peEx WITH (NOLOCK) ON pe.PeriodId = peEx.PeriodId
		AND pe.EmployeeId = peEx.EmployeeId
		LEFT JOIN dbo.LookupData AS lkd WITH (NOLOCK) ON emp.IsLocal = CAST(lkd.Value AS BIT)
		AND lkd.LookupId = 11 -- employee category
	WHERE pe.PeriodId = @PeriodId
		AND period.TargetTypeId = @TargetTypeId
		AND (
			(
				@IsExcludedEmployee IS NULL
				OR @IsExcludedEmployee = 0
				)
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.PeriodExcludedEmployee
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
			ISNULL(@DepartmentId, '') = ''
			OR (
				@DepartmentId < 0
				AND gr.GroupId = - @DepartmentId
				)
			OR (
				@DepartmentId > 0
				AND EXISTS (
					SELECT 1
					FROM dbo.Department WITH (NOLOCK)
					WHERE DepartmentId = @DepartmentId
						AND Department.lft <= gr.lft
						AND Department.rgt >= gr.rgt
					)
				)
			)
		AND (
			@IsLocal IS NULL
			OR emp.IsLocal = @IsLocal
			)
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
								AND CAST(StartDate AS DATE) <= CAST(GETDATE() AS DATE)
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
			@HasMonthlyEPA IS NULL
			OR @HasMonthlyEPA = 0
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
		,HasEvaluationGenerated = CASE 
			WHEN pfs.ProcessId = 2
				AND ISNULL(epaP.EPAId, '') <> '' THEN 1
			ELSE 0
			END
		,ProcessStatus = CASE 
			WHEN ISNULL(epa.EPAId, '') <> '' THEN CASE 
					WHEN epaP.IsEndProcess = 0 THEN CASE 
							WHEN CAST(pfs.StartDate AS DATE) > CAST(GETDATE() AS DATE) THEN 1 -- Open
							ELSE 2 -- In Progress
							END
					ELSE 3 -- Closed
					END
			ELSE 0 -- None
			END
		,ProcessDetailStatus = CASE 
			WHEN ISNULL(epaP.EPAId, '') <> '' THEN CASE 
					WHEN ISNULL(epaP.TotalScore, '') <> '' THEN 2 -- Has Evaluated
					ELSE 1 -- None Evaluated
					END
			ELSE 0
			END
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
			WHEN ISNULL(epaP.EPAId, '') <> '' THEN 1
			ELSE 0 END
			)
		AND (
			@IsEvaluated IS NULL
			OR (
				@IsEvaluated = 1
				AND ISNULL(epa.EPAId, '') <> ''
				AND EXISTS (
					SELECT 1
					FROM dbo.EPAProcess WITH (NOLOCK)
					WHERE EPAId = epa.EPAId
						AND ISNULL(TotalScore, '') = ''
						AND CAST(StartDate AS DATE) <= CAST(GETDATE() AS DATE)
						AND ProcessId <> 1
					)
				)
			OR (
				@IsEvaluated = 2
				AND ISNULL(epa.EPAId, '') <> ''
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
END