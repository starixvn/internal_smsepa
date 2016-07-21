


/****** Object:  Stored Procedure dbo.Epa_GetEPAHistory    Script Date: 4/6/2016 11:27:40 AM ******/

CREATE PROCEDURE [dbo].[Epa_GetEPAHistory] @PeriodId INT = NULL
	,@EvaluatorId INT
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
					FROM [dbo].[EmployeeRole] AS empR
					WHERE [empR].[EmployeeId] = @EvaluatorId
						AND RoleId IN (1, 2)
					) THEN 1
			ELSE 0
			END

	SELECT epaA.[EncodeFinalScore]
		,epaA.[EncodeGrade]
		,epaA.[HasEPAForm]
		,epaA.[EPAId]
		,epaA.[HasMonthlyEPA]
		,epaA.[PeriodId]
		,epaA.[Name]
		,epaA.[StartDate]
		,epaA.[EndDate]
		,epaA.[TargetTypeId]
		,epaA.[EmployeeId]
		,epaA.[NickName]
		,epaA.[FullName]
		,epaA.[Code]
	FROM (
		SELECT epa.[FinalScore] AS EncodeFinalScore
			,epa.[Grade] AS EncodeGrade
			,1 AS HasEPAForm
			,epa.[EPAId]
			,epa.HasMonthlyEPA
			,pe.[PeriodId]
			,pe.[Name]
			,pe.[StartDate]
			,pe.[EndDate]
			,pe.[TargetTypeId]
			,emp.[EmployeeId]
			,emp.[NickName]
			,emp.[FullName]
			,emp.[Code]
		FROM [dbo].[EPA] AS epa WITH (NOLOCK)
			INNER JOIN [dbo].[Period] AS pe WITH (NOLOCK) ON epa.[PeriodId] = pe.[PeriodId]
			INNER JOIN [dbo].[Employee] AS emp WITH (NOLOCK) ON epa.[EmployeeId] = emp.[EmployeeId]
			INNER JOIN [dbo].[Group] AS gr WITH (NOLOCK) ON emp.[GroupId] = gr.[GroupId]
		WHERE (
				ISNULL(@PeriodId, '') = ''
				OR (
					epa.PeriodId = @PeriodId
					AND @PeriodId > 0
					)
				)
			AND epa.[Status] = 1 -- Closed
			AND pe.TargetTypeId <> 2 -- monthly
			AND (
				@Is_HR_SA = 1 -- SA, HR can view all member of period
				OR EXISTS (
					SELECT 1
					FROM [dbo].[EmployeeRole] AS empR WITH (NOLOCK)
						INNER JOIN [dbo].[Employee] AS empT ON empR.[EmployeeId] = empT.[EmployeeId]
					WHERE [empR].[EmployeeId] = @EvaluatorId
						AND RoleId IN (3) -- MGT
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
													AND epaT.[EmployeeId] = epa.[EmployeeId]
													AND peT.[TargetTypeId] = pe.[TargetTypeId]
													AND epaPT.[ProcessId] = 1
												GROUP BY epaT.[EmployeeId]
												)
										)
									AND epaP.[EvaluatorId] = empR.[EmployeeId]
									AND epaP.[ProcessId] = 5 -- MGT								
								)
							)
					)
				OR EXISTS (
					SELECT 1
					FROM [dbo].[EmployeeRole] AS empR WITH (NOLOCK)
						INNER JOIN [dbo].[Employee] AS empT ON empR.[EmployeeId] = empT.[EmployeeId]
					WHERE [empR].[EmployeeId] = @EvaluatorId
						AND RoleId IN (4) -- Reviewer
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
													AND epaT.[EmployeeId] = epa.[EmployeeId]
													AND peT.[TargetTypeId] = pe.[TargetTypeId]
													AND epaPT.[ProcessId] = 1
												GROUP BY epaT.[EmployeeId]
												)
										)
									AND epaP.[EvaluatorId] = empR.[EmployeeId]
									AND epaP.[ProcessId] = 4 -- Reviewer								
								)
							)
					)
				OR EXISTS (
					SELECT 1
					FROM [dbo].[EmployeeRole] AS empR WITH (NOLOCK)
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
													AND epaT.[EmployeeId] = epa.[EmployeeId]
													AND peT.[TargetTypeId] = pe.[TargetTypeId]
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
						AND emp.[EmployeeId] = @EvaluatorId
					)
				)
			AND (
				ISNULL(@EmployeeId, '') = ''
				OR emp.[EmployeeId] = @EmployeeId
				)
		
		UNION ALL
		SELECT [FinalScore] AS EncodeFinalScore
			,NULL AS EncodeGrade
			,0 AS HasEPAForm
			,NULL AS EPAId
			,0 AS HasMonthlyEPA
			,[PeriodId]
			,[PeriodName] AS NAME
			,[PeriodDate] AS StartDate
			,DATEADD(DAY, 29, [PeriodDate]) AS EndDate
			,NULL as TargetTypeId
			,[Id] AS EmployeeId
			,epa.[NickName]
			,epa.[FullName]
			,epa.[Code]
		FROM [dbo].[EPASummaryImport] AS epa WITH (NOLOCK)
			INNER JOIN [dbo].[Employee] AS emp WITH (NOLOCK) ON epa.[Code] = emp.[Code]
			INNER JOIN [dbo].[Group] AS gr WITH (NOLOCK) ON emp.[GroupId] = gr.[GroupId]
		WHERE (
				ISNULL(@PeriodId, '') = ''
				OR (
					epa.PeriodId = @PeriodId
					AND @PeriodId < 0
					)
				)
			AND (
				@Is_HR_SA = 1 -- SA, HR can view all member of period
				OR EXISTS (
					SELECT 1
					FROM [dbo].[EmployeeRole] AS empR WITH (NOLOCK)
						INNER JOIN [dbo].[Employee] AS empT ON empR.[EmployeeId] = empT.[EmployeeId]
					WHERE [empR].[EmployeeId] = @EvaluatorId
						AND RoleId IN (3) -- MGT
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
													AND epaT.[EmployeeId] = emp.[EmployeeId]
													AND peT.[TargetTypeId] = 3 -- Semi annual
													AND epaPT.[ProcessId] = 1
												GROUP BY epaT.[EmployeeId]
												)
										)
									AND epaP.[EvaluatorId] = empR.[EmployeeId]
									AND epaP.[ProcessId] = 5 -- MGT								
								)
							)
					)
				OR EXISTS (
					SELECT 1
					FROM [dbo].[EmployeeRole] AS empR WITH (NOLOCK)
						INNER JOIN [dbo].[Employee] AS empT ON empR.[EmployeeId] = empT.[EmployeeId]
					WHERE [empR].[EmployeeId] = @EvaluatorId
						AND RoleId IN (4) -- Reviewer
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
													AND epaT.[EmployeeId] = emp.[EmployeeId]
													AND peT.[TargetTypeId] = 3 -- Semi annual
													AND epaPT.[ProcessId] = 1
												GROUP BY epaT.[EmployeeId]
												)
										)
									AND epaP.[EvaluatorId] = empR.[EmployeeId]
									AND epaP.[ProcessId] = 4 -- Reviewer								
								)
							)
					)
				OR EXISTS (
					SELECT 1
					FROM [dbo].[EmployeeRole] AS empR WITH (NOLOCK)
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
													AND epaT.[EmployeeId] = emp.[EmployeeId]
													AND peT.[TargetTypeId] = 3 -- Semi annual
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
						AND emp.[EmployeeId] = @EvaluatorId
					)
				)
			AND (
				ISNULL(@EmployeeId, '') = ''
				OR emp.[EmployeeId] = @EmployeeId
				)
		) AS epaA
	ORDER BY epaA.[StartDate] DESC
		,epaA.[Code]
END


