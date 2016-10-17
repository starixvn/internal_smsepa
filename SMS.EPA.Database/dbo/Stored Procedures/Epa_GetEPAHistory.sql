
CREATE PROCEDURE [dbo].[Epa_GetEPAHistory] @PeriodId INT = NULL
	,@EvaluatorId INT
	,@DepartmentId INT = NULL
	,@IsLocal BIT = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 2015-10-19@Josh.Phan
Task: Get EPA history.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Is_HR_SA BIT

	SET @Is_HR_SA = CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EmployeeRole AS empR
					WHERE empR.EmployeeId = @EvaluatorId
						AND RoleId IN (1, 2)
					) THEN 1
			ELSE 0
			END

	SELECT data.EncodeFinalScore
		,data.EncodeGrade
		,data.HasEPAForm
		,data.HasMonthlyEPA
		,data.EPAId
		,data.PeriodId
		,data.PeriodName AS [Name]
		,data.StartDate
		,data.EndDate
		,data.TargetTypeId
		,data.EmployeeId
		,data.NickName
		,data.FullName
		,data.Code
		,data.IsLocal
		,data.GroupId
		,data.GroupName
	FROM (
		SELECT epa.FinalScore AS EncodeFinalScore
			,epa.Grade AS EncodeGrade
			,1 AS HasEPAForm
			,epa.HasMonthlyEPA
			,epa.EPAId
			,epa.[Status]
			,pe.PeriodId
			,pe.[Name] AS PeriodName
			,pe.StartDate
			,pe.EndDate
			,pe.TargetTypeId
			,emp.EmployeeId
			,emp.NickName
			,emp.FullName
			,emp.Code
			,emp.IsLocal
			,dept.DepartmentId
			,dept.[Name] AS DepartmentName
			,gr.GroupId
			,gr.[Name] AS GroupName
			,gr.Code AS GroupCode
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON epa.PeriodId = pe.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		
		UNION ALL
		SELECT FinalScore AS EncodeFinalScore
			,NULL AS EncodeGrade
			,0 AS HasEPAForm
			,0 AS HasMonthlyEPA
			,NULL AS EPAId
			,1 AS [Status]
			,PeriodId
			,PeriodName
			,PeriodDate AS StartDate
			,DATEADD(DAY, 29, [PeriodDate]) AS EndDate
			,3 AS TargetTypeId
			,emp.EmployeeId
			,emp.NickName
			,emp.FullName
			,emp.Code
			,emp.IsLocal
			,dept.DepartmentId
			,dept.[Name] AS DepartmentName
			,gr.GroupId
			,gr.[Name] AS GroupName
			,gr.Code AS GroupCode
		FROM dbo.EPASummaryImport AS epa WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.Code = emp.Code
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		) AS data
	WHERE (
			ISNULL(@PeriodId, '') = ''
			OR (
				data.PeriodId = @PeriodId
				AND @PeriodId > 0
				)
			)
		AND (
			ISNULL(@DepartmentId, '') = ''
			OR (
				@DepartmentId < 0
				AND data.GroupId = ABS(@DepartmentId)
				)
			OR (
				@DepartmentId > 0
				AND (
					data.DepartmentId = @DepartmentId
					OR data.DepartmentId IN (
						SELECT DepartmentId
						FROM dbo.RelatedDepartment(@DepartmentId)
						
						UNION
						SELECT @DepartmentId
						)
					)
				)
			)
		
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR data.EmployeeId = @EmployeeId
			)
		AND (
			@IsLocal IS NULL
			OR data.IsLocal = @IsLocal
			)
		AND data.[Status] = 1 -- Closed
		AND data.TargetTypeId <> 2 -- monthly
		--AND CAST(pe.[EndDate] AS DATE) <= CAST(GETDATE() AS DATE)
		AND (
			@Is_HR_SA = 1 -- SA, HR can view all member of period
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empR WITH (NOLOCK)
					INNER JOIN dbo.Employee AS empT ON empR.EmployeeId = empT.EmployeeId
				WHERE empR.EmployeeId = @EvaluatorId
					AND RoleId IN (3) -- MGT
					AND (
						EXISTS (
							SELECT 1
							FROM dbo.EPAProcess AS epaP WITH (NOLOCK)
							WHERE EXISTS (
									SELECT 1
									FROM dbo.EPAProcess AS epa1 WITH (NOLOCK)
									WHERE epa1.EPAId = epaP.EPAId
										AND epa1.ProcessId = 1
										AND epa1.StartDate = (
											SELECT MAX(epaPT.StartDate)
											FROM dbo.EPAProcess AS epaPT WITH (NOLOCK)
												INNER JOIN dbo.EPA AS epaT WITH (NOLOCK) ON epaPT.EPAId = epaT.EPAId
												INNER JOIN dbo.Period AS peT WITH (NOLOCK) ON epaT.PeriodId = peT.PeriodId
											WHERE epaPT.EPAId = epa1.EPAId
												AND epaT.EmployeeId = data.EmployeeId
												AND peT.TargetTypeId = data.TargetTypeId
												AND epaPT.ProcessId = 1
											GROUP BY epaT.EmployeeId
											)
									)
								AND epaP.EvaluatorId = empR.EmployeeId
								AND epaP.ProcessId = 5 -- MGT								
							)
						)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS empR WITH (NOLOCK)
					INNER JOIN dbo.Employee AS empT ON empR.EmployeeId = empT.EmployeeId
				WHERE empR.EmployeeId = @EvaluatorId
					AND RoleId IN (4) -- Reviewer
					AND (
						EXISTS (
							SELECT 1
							FROM dbo.EPAProcess AS epaP WITH (NOLOCK)
							WHERE EXISTS (
									SELECT 1
									FROM dbo.EPAProcess AS epa1 WITH (NOLOCK)
									WHERE epa1.EPAId = epaP.EPAId
										AND epa1.ProcessId = 1
										AND epa1.StartDate = (
											SELECT MAX(epaPT.StartDate)
											FROM dbo.EPAProcess AS epaPT WITH (NOLOCK)
												INNER JOIN dbo.EPA AS epaT WITH (NOLOCK) ON epaPT.EPAId = epaT.EPAId
												INNER JOIN dbo.Period AS peT WITH (NOLOCK) ON epaT.PeriodId = peT.PeriodId
											WHERE epaPT.EPAId = epa1.EPAId
												AND epaT.EmployeeId = data.EmployeeId
												AND peT.TargetTypeId = data.TargetTypeId
												AND epaPT.ProcessId = 1
											GROUP BY epaT.EmployeeId
											)
									)
								AND epaP.EvaluatorId = empR.EmployeeId
								AND epaP.ProcessId = 4 -- Reviewer								
							)
						)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.[EmployeeRole] AS empR WITH (NOLOCK)
					INNER JOIN [dbo].[Employee] AS empT ON empR.[EmployeeId] = empT.[EmployeeId]
				WHERE [empR].[EmployeeId] = @EvaluatorId
					AND RoleId IN (5) -- Evaluator
					AND (
						EXISTS (
							SELECT 1
							FROM [dbo].[EPAProcess] AS epaP WITH (NOLOCK)
							WHERE EXISTS (
									SELECT 1
									FROM [dbo].[EPAProcess] AS epa1 WITH (NOLOCK)
									WHERE epa1.[EPAId] = epaP.[EPAId]
										AND epa1.[ProcessId] = 1
										AND epa1.[StartDate] = (
											SELECT MAX(epaPT.[StartDate])
											FROM [dbo].[EPAProcess] AS epaPT WITH (NOLOCK)
												INNER JOIN [dbo].[EPA] AS epaT WITH (NOLOCK) ON epaPT.[EPAId] = epaT.[EPAId]
												INNER JOIN [dbo].[Period] AS peT WITH (NOLOCK) ON epaT.[PeriodId] = peT.[PeriodId]
											WHERE epaPT.[EPAId] = epa1.[EPAId]
												AND epaT.[EmployeeId] = data.[EmployeeId]
												AND peT.[TargetTypeId] = data.[TargetTypeId]
												AND epaPT.[ProcessId] = 1
											GROUP BY epaT.[EmployeeId]
											)
									)
								AND epaP.[EvaluatorId] = empR.[EmployeeId]
								AND epaP.[ProcessId] = 3 -- Evaluator								
							)
						)
				)
			--OR EXISTS (
			--	SELECT TOP 1 1
			--	FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
			--	WHERE gr.[GroupId] = grS.[GroupId]
			--		AND @EvaluatorId = grS.[OwnerId]
			--		AND grS.[ActionType] IN (3) -- Approver
			--	)
			OR EXISTS (
				SELECT 1
				FROM [dbo].[EmployeeRole] AS empR WITH (NOLOCK)
				WHERE [empR].[EmployeeId] = @EvaluatorId
					AND RoleId = 6 -- Employee can view their EPA
					AND data.[EmployeeId] = @EvaluatorId
				)
			)
	ORDER BY data.StartDate DESC
		,data.GroupCode
		,data.Code
END