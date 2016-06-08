
CREATE PROCEDURE [dbo].[EpaAttendanceSummary_Get_xtest] @StartDate DATETIME
	,@EndDate DATETIME
	,@DepartmentId INT = NULL
	,@EmployeeId INT = NULL
	,@ReasonId INT = NULL
	,@IsLocal BIT = NULL
	,@ApproverId INT
	,@ShowAll BIT = 0
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Get attendance summary of employees.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmployeeViolation TABLE (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[MissingCheckInOut] INT
		,[LateIn] INT
		,[EarlyOut] INT
		,[Absent] INT
		,[WorkingBelowTime] INT
		,[UnpaidLeave] INT
		,[ChangeShift] INT
		,[UnpaidLeaveAbsent] INT
		,[OtherViolationsScore] FLOAT
		)

	INSERT INTO @EmployeeViolation (
		[WorkDate]
		,[EmpId]
		,[MissingCheckInOut]
		,[LateIn]
		,[EarlyOut]
		,[Absent]
		,[WorkingBelowTime]
		,[UnpaidLeave]
		,[ChangeShift]
		,[UnpaidLeaveAbsent]
		,[OtherViolationsScore]
		)
	SELECT [WorkDate]
		,[EmpId]
		,SUM([MissingCheckInOut])
		,SUM([LateIn])
		,SUM([EarlyOut])
		,SUM([Absent])
		,SUM([WorkingBelowTime])
		,SUM([UnpaidLeave])
		,SUM([ChangeShift])
		,SUM([UnpaidLeaveAbsent])
		,SUM([OtherViolationsScore])
	FROM (
		SELECT [WorkDate]
			,[EmpId]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 2 -- Missing Check In/Out
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = [EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [MissingCheckInOutAutoIgnore] <> 1 THEN [MissingCheckInOut]
						ELSE 0
						END
				ELSE 0 END
				) AS [MissingCheckInOut]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 3 -- Late In
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND [EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [LateInAutoIgnore] <> 1 THEN [LateIn]
						ELSE 0
						END
				ELSE 0 END
				) AS [LateIn]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 4 -- Early Out
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND [EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [EarlyOutAutoIgnore] <> 1 THEN [EarlyOut]
						ELSE 0
						END
				ELSE 0 END
				) AS [EarlyOut]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 5 -- Absent
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND [EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [AbsentAutoIgnore] <> 1 THEN [Absent]
						ELSE 0
						END
				ELSE 0 END
				) AS [Absent]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 7 -- Working under time
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND [EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [WorkingBelowTimeAutoIgnore] <> 1 THEN [WorkingBelowTime]
						ELSE 0
						END
				ELSE 0 END
				) AS [WorkingBelowTime]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 6 -- Unpaid Leave
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND [EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [UnpaidLeaveAutoIgnore] <> 1 
						THEN CASE WHEN [UnpaidLeave] = 0.5 THEN 1 ELSE [UnpaidLeave] END
						ELSE 0
						END
				ELSE 0 END
				) AS [UnpaidLeave]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 1 -- Change Shift
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND [EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [ChangeShiftAutoIgnore] <> 1 THEN [ChangeShift]
						ELSE 0
						END
				ELSE 0 END
				) AS [ChangeShift]
			,(CASE (
					SELECT [Value]
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 8 -- Unpaid Leave Absent
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE attS.[GroupId] = [GroupId]
								AND [EmpId] = [EmployeeId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				WHEN 1 THEN CASE 
						WHEN [UnpaidLeaveAbsentAutoIgnore] <> 1 
						THEN CASE WHEN [UnpaidLeaveAbsent] = 0.5 THEN 1 ELSE [UnpaidLeaveAbsent] END
						ELSE 0
						END
				ELSE 0 END
				) AS [UnpaidLeaveAbsent]
			,0 AS OtherViolationsScore
			,'' AS OtherViolationsName
		FROM [dbo].[HRMEmployeeAttendanceViolation] WITH (NOLOCK)
		
		UNION ALL
		SELECT [WorkDate]
			,[EmpId]
			,0 AS MissingCheckInOut
			,0 AS LateIn
			,0 AS EarlyOut
			,0 AS [Absent]
			,0 AS WorkingBelowTime
			,0 AS UnpaidLeave
			,0 AS ChangeShift
			,0 AS UnpaidLeaveAbsent
			,[Score] AS OtherViolationsScore
			,[Name] AS OtherViolationsName
		FROM [dbo].[EmployeeOtherViolation] WITH (NOLOCK)
		) AS empV
	WHERE CAST([WorkDate] AS DATE) >= CAST(@StartDate AS DATE)
		AND CAST([WorkDate] AS DATE) <= CAST(@EndDate AS DATE)
	GROUP BY [EmpId]
		,[WorkDate]

	DECLARE @EmployeeViolationSystemScore TABLE (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[MissingCheckInOut] INT
		,[LateIn] INT
		,[EarlyOut] INT
		,[Absent] INT
		,[WorkingBelowTime] INT
		,[UnpaidLeave] INT
		,[ChangeShift] INT
		,[UnpaidLeaveAbsent] INT
		,[OtherViolationsScore] FLOAT
		,[SystemSubtractScore] FLOAT
		,[ApproverIgnoredScore] FLOAT
		)

	INSERT INTO @EmployeeViolationSystemScore (
		[WorkDate]
		,[EmpId]
		,[MissingCheckInOut]
		,[LateIn]
		,[EarlyOut]
		,[Absent]
		,[WorkingBelowTime]
		,[UnpaidLeave]
		,[ChangeShift]
		,[UnpaidLeaveAbsent]
		,[OtherViolationsScore]
		,[SystemSubtractScore]
		,[ApproverIgnoredScore]
		)
	SELECT [WorkDate]
		,[EmpId]
		,[MissingCheckInOut]
		,[LateIn]
		,[EarlyOut]
		,[Absent]
		,[WorkingBelowTime]
		,[UnpaidLeave]
		,[ChangeShift]
		,[UnpaidLeaveAbsent]
		,[OtherViolationsScore]
		,(
			[MissingCheckInOut] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 2 -- Missing Check In/Out
				) + empV.[LateIn] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 3 -- Late In
				) + empV.[EarlyOut] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 4 -- Early Out
				) + empV.[Absent] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 5 -- Absent
				) + empV.[WorkingBelowTime] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 7 -- Working below 7H
				) + empV.[UnpaidLeave] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 6 -- Unpaid Leave
				) + empV.[ChangeShift] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 1 -- Change Shift
				) + empV.[UnpaidLeaveAbsent] * (
				SELECT [Score]
				FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
				WHERE [Id] = 8 -- Unpaid Leave Absent
				)
			) AS SystemSubtractScore
		,(
			SELECT SUM(attV.[Score])
			FROM [dbo].[AttendanceDetailIgnored] AS attI WITH (NOLOCK)
				INNER JOIN [dbo].[AttendanceViolation] AS attV WITH (NOLOCK) ON attI.[ViolationId] = attV.[Id]
			WHERE attI.[EmployeeId] = empV.[EmpId]
				AND CAST(attI.[WorkDate] AS DATE) >= CAST(@StartDate AS DATE)
				AND CAST(attI.[WorkDate] AS DATE) <= CAST(@EndDate AS DATE)
				AND CAST(attI.[WorkDate] AS DATE) = CAST(empV.[WorkDate] AS DATE)
			GROUP BY attI.[EmployeeId]
			) AS ApproverIgnoredScore
	FROM @EmployeeViolation AS empV
	WHERE (
			[Absent] > 0
			OR [ChangeShift] > 0
			OR [EarlyOut] > 0
			OR [LateIn] > 0
			OR [UnpaidLeave] > 0
			OR [WorkingBelowTime] > 0
			OR [UnpaidLeaveAbsent] > 0
			OR [OtherViolationsScore] > 0
			OR [MissingCheckInOut] > 0
			)

	DECLARE @EmployeeViolationFinalScore TABLE (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[SystemSubtractScore] FLOAT
		,[ApproverSubtractScore] FLOAT
		)

	INSERT INTO @EmployeeViolationFinalScore (
		[WorkDate]
		,[EmpId]
		,[SystemSubtractScore]
		,[ApproverSubtractScore]
		)
	SELECT [WorkDate]
		,[EmpId]
		,(CASE 
			WHEN [SystemSubtractScore] < 3.5 THEN [SystemSubtractScore]
			ELSE 3.5 END
			) AS SystemSubtractScore
		,(CASE 
			WHEN [SystemSubtractScore] - (CASE 
					WHEN ISNULL([ApproverIgnoredScore], '') <> '' THEN [ApproverIgnoredScore]
					ELSE 0 END
					) < 3.5 THEN [SystemSubtractScore] - (CASE 
					WHEN ISNULL([ApproverIgnoredScore], '') <> '' THEN [ApproverIgnoredScore]
					ELSE 0 END
					)
			ELSE 3.5 END
			) AS ApproverSubtractScore
	FROM @EmployeeViolationSystemScore

	DECLARE @Is_HR_MGT_SA BIT

	SET @Is_HR_MGT_SA = CASE 
			WHEN EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[EmployeeRole] AS empR
					WHERE [empR].[EmployeeId] = @ApproverId
						AND RoleId IN (1, 2, 3)
					) THEN 1
			ELSE 0
			END

	IF @ShowAll = 0
	BEGIN
		SELECT SUM(empV.[MissingCheckInOut]) AS MissingCheckInOut
			,SUM(empV.[LateIn]) AS LateIn
			,SUM(empV.[EarlyOut]) AS EarlyOut
			,SUM(empV.[Absent]) AS [Absent]
			,SUM(empV.[WorkingBelowTime]) AS WorkingBelowTime
			,SUM(empV.[UnpaidLeave]) AS UnpaidLeave
			,SUM(empV.[ChangeShift]) AS ChangeShift
			,SUM(empV.[UnpaidLeaveAbsent]) AS UnpaidLeaveAbsent
			,SUM(empV.[OtherViolationsScore]) AS OtherViolationsScore
			,SUM(empF.[SystemSubtractScore] + empV.[OtherViolationsScore]) AS SystemSubtractScore
			,SUM(empF.[ApproverSubtractScore] + empV.[OtherViolationsScore]) AS ApproverSubtractScore
			,Permission = CASE 
				WHEN EXISTS (
						SELECT TOP 1 1
						FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
						WHERE empS.[OwnerId] = @ApproverId
							AND empS.[EmpId] = emp.[EmployeeId]
							AND empS.[ActionType] = 3
							AND empS.[EmpId] <> @ApproverId
						)
					OR EXISTS (
						SELECT TOP 1 1
						FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
						WHERE gr.[GroupId] = grS.[GroupId]
							AND @ApproverId = grS.[OwnerId]
							AND grS.[ActionType] = 3
							AND emp.[EmployeeId] <> @ApproverId
							--AND NOT EXISTS (
							--	SELECT TOP 1 1
							--	FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
							--	WHERE [OwnerId] <> @ApproverId
							--		AND [EmpId] = emp.[EmployeeId]
							--		AND [ActionType] = 3
								--)
						) THEN 2 -- Host				
				ELSE 1
				END
			,empV.[EmpId] AS EmployeeId
			,emp.[NickName]
			,emp.[FullName]
			,lk.[Name] AS Category
			,emp.[Code]
			,gr.[GroupId]
			,gr.[Name] AS GroupName
			,emp.JobTitle AS JobTitleId
			,job.[Name] AS JobTitleName
		FROM @EmployeeViolationFinalScore AS empF
			INNER JOIN @EmployeeViolationSystemScore AS empV ON empF.[WorkDate] = empV.[WorkDate]
			AND empF.[EmpId] = empV.[EmpId]
			INNER JOIN [dbo].[Employee] AS emp WITH (NOLOCK) ON empV.[EmpId] = emp.[EmployeeId]
			LEFT JOIN [dbo].[Group] AS gr WITH (NOLOCK) ON emp.[GroupId] = gr.[GroupId]
			INNER JOIN [dbo].[LookupData] AS lk WITH (NOLOCK) ON emp.[IsLocal] = CAST(lk.[Value] AS BIT)
			AND lk.[LookupId] = 11
			LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
		WHERE emp.[Enabled] = 1
			AND emp.[IsDeleted] = 0
			AND (
				@Is_HR_MGT_SA = 1 -- SA, HR and Management can view all member of company
				--OR 
				--EXISTS (
				--	SELECT TOP 1 1
				--	FROM [dbo].[EmployeeRole] AS empR
				--		INNER JOIN [dbo].[Employee] AS empT ON empR.[EmployeeId] = empT.[EmployeeId]
				--	WHERE [empR].[EmployeeId] = @ApproverId
				--		AND RoleId IN (4, 5)
				--		AND empT.[GroupId] = gr.[GroupId]
				--	)
				OR EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
					WHERE empS.[OwnerId] = @ApproverId
						AND empS.[EmpId] = emp.[EmployeeId]
						AND empS.[ActionType] IN (2, 3)
					)
				OR EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
					WHERE gr.[GroupId] = grS.[GroupId]
						AND @ApproverId = grS.[OwnerId]
						AND grS.[ActionType] IN (2, 3) -- Approver and Verifier can view all member of section that they has role apporver or verifier
						AND NOT EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
							WHERE [OwnerId] <> @ApproverId
								AND [EmpId] = emp.[EmployeeId]
								AND [ActionType] IN (2, 3)
							)
					)
				OR EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[EmployeeRole] AS empR
					WHERE [empR].[EmployeeId] = @ApproverId
						AND RoleId = 6 -- Employee can view their discipline
						AND empV.[EmpId] = @ApproverId
					)
				)
			AND (
				ISNULL(@DepartmentId, '') = ''
				OR (
					@DepartmentId < 0
					AND gr.[GroupId] = - @DepartmentId
					)
				OR (
					@DepartmentId > 0
					AND EXISTS (
						SELECT TOP 1 1
						FROM [dbo].[Department] WITH (NOLOCK)
						WHERE [DepartmentId] = @DepartmentId
							AND [lft] <= gr.[lft]
							AND [rgt] >= gr.[rgt]
						)
					)
				)
			AND (
				ISNULL(@EmployeeId, '') = ''
				OR empV.[EmpId] = @EmployeeId
				)
			AND (
				ISNULL(@ReasonId, '') = ''
				OR (
					@ReasonId = 1
					AND [ChangeShift] > 0
					)
				OR (
					@ReasonId = 2
					AND [MissingCheckInOut] > 0
					)
				OR (
					@ReasonId = 3
					AND [LateIn] > 0
					)
				OR (
					@ReasonId = 4
					AND [EarlyOut] > 0
					)
				OR (
					@ReasonId = 5
					AND [Absent] > 0
					)
				OR (
					@ReasonId = 6
					AND [UnpaidLeave] > 0
					)
				OR (
					@ReasonId = 7
					AND [WorkingBelowTime] > 0
					)
				OR (
					@ReasonId = 8
					AND [UnpaidLeaveAbsent] > 0
					)
				)
			AND (
				@IsLocal IS NULL
				OR emp.[IsLocal] = @IsLocal
				)
		GROUP BY empV.[EmpId]
			,emp.[EmployeeId]
			,emp.[NickName]
			,emp.[FullName]
			,emp.[JobTitle]
			,job.[Name]
			,lk.[Name]
			,emp.[Code]
			,gr.[GroupId]
			,gr.[Name]
		ORDER BY gr.[GroupId]
			,emp.[Code]
	END
	ELSE
	BEGIN
		SELECT SUM(empV.[MissingCheckInOut]) AS MissingCheckInOut
			,SUM(empV.[LateIn]) AS LateIn
			,SUM(empV.[EarlyOut]) AS EarlyOut
			,SUM(empV.[Absent]) AS [Absent]
			,SUM(empV.[WorkingBelowTime]) AS WorkingBelowTime
			,SUM(empV.[UnpaidLeave]) AS UnpaidLeave
			,SUM(empV.[ChangeShift]) AS ChangeShift
			,SUM(empV.[UnpaidLeaveAbsent]) AS UnpaidLeaveAbsent
			,SUM(empV.[OtherViolationsScore]) AS OtherViolationsScore
			,SUM(empF.[SystemSubtractScore] + empV.[OtherViolationsScore]) AS SystemSubtractScore
			,SUM(empF.[ApproverSubtractScore] + empV.[OtherViolationsScore]) AS ApproverSubtractScore
			,Permission = CASE 
				WHEN EXISTS (
						SELECT TOP 1 1
						FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
						WHERE empS.[OwnerId] = @ApproverId
							AND empS.[EmpId] = emp.[EmployeeId]
							AND empS.[ActionType] = 3
							AND empS.[EmpId] <> @ApproverId
						)
					OR EXISTS (
						SELECT TOP 1 1
						FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
						WHERE gr.[GroupId] = grS.[GroupId]
							AND @ApproverId = grS.[OwnerId]
							AND grS.[ActionType] = 3
							AND emp.[EmployeeId] <> @ApproverId
							AND NOT EXISTS (
								SELECT TOP 1 1
								FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
								WHERE [OwnerId] <> @ApproverId
									AND [EmpId] = emp.[EmployeeId]
									AND [ActionType] = 3
								)
						) THEN 2 -- Host				
				ELSE 1
				END
			,emp.[EmployeeId]
			,emp.[NickName]
			,emp.[FullName]
			,lk.[Name] AS Category
			,emp.[Code]
			,gr.[GroupId]
			,gr.[Name] AS GroupName
			,emp.[JobTitle] AS JobTitleId
			,job.[Name] AS JobTitleName
		FROM [dbo].[Employee] AS emp WITH (NOLOCK)
			LEFT JOIN @EmployeeViolationFinalScore AS empF ON emp.[EmployeeId] = empF.[EmpId]
			LEFT JOIN @EmployeeViolationSystemScore AS empV ON empF.[WorkDate] = empV.[WorkDate]
			AND empF.[EmpId] = empV.[EmpId]
			LEFT JOIN [dbo].[Group] AS gr WITH (NOLOCK) ON emp.[GroupId] = gr.[GroupId]
			INNER JOIN [dbo].[LookupData] AS lk WITH (NOLOCK) ON emp.[IsLocal] = CAST(lk.[Value] AS BIT)
			AND lk.[LookupId] = 11
			LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
		WHERE emp.[Enabled] = 1
			AND emp.[IsDeleted] = 0
			AND (
				--@Is_HR_MGT_SA = 1 -- SA, HR and Management can view all member of company
				--OR 
				EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[EmployeeRole] AS empR
						INNER JOIN [dbo].[Employee] AS empT ON empR.[EmployeeId] = empT.[EmployeeId]
					WHERE [empR].[EmployeeId] = @ApproverId
						AND RoleId IN (4, 5)
						AND empT.[GroupId] = gr.[GroupId]
					)
				OR EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
					WHERE empS.[OwnerId] = @ApproverId
						AND empS.[EmpId] = emp.[EmployeeId]
						AND empS.[ActionType] IN (2, 3)
					)
				OR EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
					WHERE gr.[GroupId] = grS.[GroupId]
						AND @ApproverId = grS.[OwnerId]
						AND grS.[ActionType] IN (2, 3) -- Approver and Verifier can view all member of section that they has role apporver or verifier
						AND NOT EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
							WHERE [OwnerId] <> @ApproverId
								AND [EmpId] = emp.[EmployeeId]
								AND [ActionType] IN (2, 3)
							)
					)
				OR EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[EmployeeRole] AS empR
					WHERE [empR].[EmployeeId] = @ApproverId
						AND RoleId = 6 -- Employee can view their discipline
						AND empV.[EmpId] = @ApproverId
					)
				)
			AND (
				ISNULL(@DepartmentId, '') = ''
				OR (
					@DepartmentId < 0
					AND gr.[GroupId] = - @DepartmentId
					)
				OR (
					@DepartmentId > 0
					AND EXISTS (
						SELECT TOP 1 1
						FROM [dbo].[Department] WITH (NOLOCK)
						WHERE [DepartmentId] = @DepartmentId
							AND [lft] <= gr.[lft]
							AND [rgt] >= gr.[rgt]
						)
					)
				)
			AND (
				ISNULL(@EmployeeId, '') = ''
				OR emp.[EmployeeId] = @EmployeeId
				)
			AND (
				ISNULL(@ReasonId, '') = ''
				OR (
					@ReasonId = 1
					AND [ChangeShift] > 0
					)
				OR (
					@ReasonId = 2
					AND [MissingCheckInOut] > 0
					)
				OR (
					@ReasonId = 3
					AND [LateIn] > 0
					)
				OR (
					@ReasonId = 4
					AND [EarlyOut] > 0
					)
				OR (
					@ReasonId = 5
					AND [Absent] > 0
					)
				OR (
					@ReasonId = 6
					AND [UnpaidLeave] > 0
					)
				OR (
					@ReasonId = 7
					AND [WorkingBelowTime] > 0
					)
				OR (
					@ReasonId = 8
					AND [UnpaidLeaveAbsent] > 0
					)
				)
			AND (
				@IsLocal IS NULL
				OR emp.[IsLocal] = @IsLocal
				)
		GROUP BY emp.[EmployeeId]
			,emp.[NickName]
			,emp.[FullName]
			,emp.[JobTitle]
			,job.[Name]
			,lk.[Name]
			,emp.[Code]
			,gr.[GroupId]
			,gr.[Name]
		ORDER BY gr.[GroupId]
			,emp.[Code]
	END
END