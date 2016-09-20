
CREATE PROCEDURE [dbo].[EPA_CheckPermission] @EPAId INT
	,@ActorUserId INT
	,@ProcessId INT = NULL
AS
/*
Created: 20151013@Tim.Trinh
Task: Check permission of user login with EPA.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Permission INT
	DECLARE @ToDay DATE

	SET @ToDay = CAST(GETDATE() AS DATE)

	IF EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
			WHERE epa.EPAId = @EPAId
				AND epa.[Status] = 0 -- open
			)
	BEGIN
		IF @ProcessId IS NOT NULL
		BEGIN
			IF EXISTS (
					SELECT 1
					FROM dbo.EPA AS epa WITH (NOLOCK)
						INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
						AND pc.EvaluatorId = @ActorUserId
						AND pc.ProcessId = @ProcessId
						AND CAST(pc.StartDate AS DATE) <= @ToDay
					WHERE epa.EPAId = @EPAId
					)
				SET @Permission = 1
			ELSE
				SET @Permission = 0 -- don't permission
		END
		ELSE
		BEGIN
			IF EXISTS (
					SELECT 1
					FROM dbo.EPA AS epa WITH (NOLOCK)
						INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
						AND pc.EvaluatorId = @ActorUserId
					WHERE epa.EPAId = @EPAId
					)
				SET @Permission = 1
			ELSE
				SET @Permission = 0 -- don't permission
		END
	END
	ELSE
	BEGIN		
		IF @ProcessId IS NULL
			AND (
				EXISTS (
					SELECT 1
					FROM dbo.EmployeeRole AS ro WITH (NOLOCK)
						INNER JOIN dbo.LookupData AS lo WITH (NOLOCK) ON ro.RoleId = lo.Value
					WHERE lo.LookupId = 2 -- role
						AND ro.EmployeeId = @ActorUserId
						AND ro.RoleId IN (1, 2) -- SA, HR
					)
				OR EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[EmployeeRole] AS empR
						INNER JOIN [dbo].[Employee] AS empT ON empR.[EmployeeId] = empT.[EmployeeId]
					WHERE [empR].[EmployeeId] = @ActorUserId
						AND RoleId IN (3) -- MGT
						AND (
							EXISTS (
								SELECT TOP 1 1
								FROM [dbo].[EPAProcess] AS epaP WITH (NOLOCK)
									INNER JOIN [dbo].[EPA] AS epa WITH (NOLOCK) ON epaP.[EPAId] = epa.[EPAId]
									INNER JOIN [dbo].[Period] AS pe WITH (NOLOCK) ON epa.[PeriodId] = pe.[PeriodId]
								WHERE EXISTS (
										SELECT TOP 1 1
										FROM [dbo].[EPA] AS epaC WITH (NOLOCK)
										WHERE epaC.[EmployeeId] = epa.[EmployeeId]
											AND epaC.[EPAId] = @EPAId
										)
									AND epaP.[EvaluatorId] = empR.[EmployeeId]
									AND epaP.[ProcessId] = 5 --MGT
									AND EXISTS (
										SELECT TOP 1 1
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
								)
							)
					)
				OR EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[EmployeeRole] AS empR
						INNER JOIN [dbo].[Employee] AS empT ON empR.[EmployeeId] = empT.[EmployeeId]
					WHERE [empR].[EmployeeId] = @ActorUserId
						AND RoleId IN (4) -- Reviewer
						AND (
							EXISTS (
								SELECT TOP 1 1
								FROM [dbo].[EPAProcess] AS epaP WITH (NOLOCK)
									INNER JOIN [dbo].[EPA] AS epa WITH (NOLOCK) ON epaP.[EPAId] = epa.[EPAId]
									INNER JOIN [dbo].[Period] AS pe WITH (NOLOCK) ON epa.[PeriodId] = pe.[PeriodId]
								WHERE EXISTS (
										SELECT TOP 1 1
										FROM [dbo].[EPA] AS epaC WITH (NOLOCK)
										WHERE epaC.[EmployeeId] = epa.[EmployeeId]
											AND epaC.[EPAId] = @EPAId
										)
									AND epaP.[EvaluatorId] = empR.[EmployeeId]
									AND epaP.[ProcessId] = 4 -- Reviewer
									AND EXISTS (
										SELECT TOP 1 1
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
								)
							)
					)
				OR EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[EmployeeRole] AS empR WITH (NOLOCK)
						INNER JOIN [dbo].[Employee] AS empT ON empR.[EmployeeId] = empT.[EmployeeId]
					WHERE [empR].[EmployeeId] = @ActorUserId
						AND RoleId IN (5) -- Evaluator
						AND (
							EXISTS (
								SELECT TOP 1 1
								FROM [dbo].[EPAProcess] AS epaP WITH (NOLOCK)
									INNER JOIN [dbo].[EPA] AS epa WITH (NOLOCK) ON epaP.[EPAId] = epa.[EPAId]
									INNER JOIN [dbo].[Period] AS pe WITH (NOLOCK) ON epa.[PeriodId] = pe.[PeriodId]
								WHERE EXISTS (
										SELECT TOP 1 1
										FROM [dbo].[EPA] AS epaC WITH (NOLOCK)
										WHERE epaC.[EmployeeId] = epa.[EmployeeId]
											AND epaC.[EPAId] = @EPAId
										)
									AND epaP.[EvaluatorId] = empR.[EmployeeId]
									AND epaP.[ProcessId] = 3 -- Evaluator	
									AND EXISTS (
										SELECT TOP 1 1
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
								)
							)
					)
				--OR EXISTS (
				--	SELECT TOP 1 1
				--	FROM dbo.EPA AS epa WITH (NOLOCK)
				--		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.[EmployeeId]
				--		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.[GroupId] = gr.[GroupId]
				--		INNER JOIN dbo.HRMGroupSupervisor AS grS WITH (NOLOCK) ON gr.[GroupId] = grS.[GroupId]
				--		AND grS.OwnerId = @ActorUserId
				--		AND grS.ActionType IN (3) -- Approver
				--	)
				OR EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[EPA] AS epa WITH (NOLOCK)
					WHERE epa.[EPAId] = @EPAId
						AND epa.[EmployeeId] = @ActorUserId
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[EmployeeRole] WITH (NOLOCK)
							WHERE RoleId = 6 -- Employee can view their EPA						
							)
					)
				)
			SET @Permission = 1
		ELSE
			SET @Permission = 0 -- don't permission
	END

	SELECT @Permission
END