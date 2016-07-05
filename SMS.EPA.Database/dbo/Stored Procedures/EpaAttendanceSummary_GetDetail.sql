CREATE PROCEDURE [dbo].[EpaAttendanceSummary_GetDetail] @StartDate DATETIME
	,@EndDate DATETIME
	,@DepartmentId INT = NULL
	,@EmployeeId INT = NULL
	,@ReasonId INT = NULL
	,@ApproverId INT
	,@IsLocal BIT = NULL
	,@HasIgnored BIT = NULL
	,@ShowAll BIT = 0
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Get attendance detail of employees.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT emp.[EmployeeId]
		,emp.[Code]
		,emp.[NickName]
		,emp.[FullName]
		,emp.JoinedDate
		,lk.[Name] AS Category
		,gr.[GroupId]
		,gr.[Name] AS GroupName
		,emp.JobTitle AS JobTitleId
		,job.[Name] AS JobTitleName
	FROM [dbo].[Employee] AS emp WITH (NOLOCK)
		INNER JOIN [dbo].[Group] AS gr WITH (NOLOCK) ON emp.[GroupId] = gr.[GroupId]
		INNER JOIN [dbo].[LookupData] AS lk WITH (NOLOCK) ON emp.[IsLocal] = CAST(lk.[Value] AS BIT)
		AND lk.[LookupId] = 11
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
	WHERE emp.[Enabled] = 1
		AND emp.[IsDeleted] = 0
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR emp.[EmployeeId] = @EmployeeId
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
					SELECT 1
					FROM [dbo].[Department] WITH (NOLOCK)
					WHERE [DepartmentId] = @DepartmentId
						AND [lft] <= gr.[lft]
						AND [rgt] >= gr.[rgt]
					)
				)
			)
		AND (
			EXISTS (
				SELECT 1
				FROM [dbo].[EmployeeRole] AS empR
				WHERE [empR].[EmployeeId] = @ApproverId
					AND RoleId IN (1, 2, 3)
				)
			--OR EXISTS (
			--	SELECT 1
			--	FROM [dbo].[EmployeeRole] AS empR
			--		INNER JOIN [dbo].[Employee] AS empT ON empR.[EmployeeId] = empT.[EmployeeId]
			--	WHERE [empR].[EmployeeId] = @ApproverId
			--		AND RoleId IN (4, 5)
			--		AND empT.[GroupId] = gr.[GroupId]
			--	)
			OR EXISTS (
				SELECT 1
				FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
				WHERE empS.[OwnerId] = @ApproverId
					AND empS.[EmpId] = emp.[EmployeeId]
					AND empS.[ActionType] IN (2, 3)
				)
			OR EXISTS (
				SELECT 1
				FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
				WHERE gr.[GroupId] = grS.[GroupId]
					AND @ApproverId = grS.[OwnerId]
					AND grS.[ActionType] IN (2, 3) -- Approver or Verifier
					AND NOT EXISTS (
						SELECT 1
						FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
						WHERE [OwnerId] <> @ApproverId
							AND [EmpId] = emp.[EmployeeId]
							AND [ActionType] IN (2, 3)
						)
				)
			OR EXISTS (
				SELECT 1
				FROM [dbo].[EmployeeRole] AS empR
				WHERE [empR].[EmployeeId] = @ApproverId
					AND RoleId = 6 -- Employee can view their discipline
					AND emp.[EmployeeId] = @ApproverId
				)
			)
		AND (
			@IsLocal IS NULL
			OR emp.[IsLocal] = @IsLocal
			)
		AND (
			@HasIgnored IS NULL
			OR (
				@HasIgnored = 0
				AND EXISTS (
					SELECT 1
					FROM [dbo].[HRMEmployeeAttendanceViolation] AS empV WITH (NOLOCK)
					WHERE empV.[EmpId] = emp.[EmployeeId]
						AND NOT EXISTS (
							SELECT 1
							FROM [dbo].[AttendanceDetailIgnored] WITH (NOLOCK)
							WHERE [EmployeeId] = empV.[EmpId]
								AND [WorkDate] = empV.[WorkDate]
							)
					)
				)
			OR (
				@HasIgnored = 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] AS attd WITH (NOLOCK)
					WHERE attd.[EmployeeId] = emp.[EmployeeId]
					AND EXISTS (
							SELECT 1
							FROM [dbo].[HRMEmployeeAttendanceViolation] AS empV
							WHERE attd.[EmployeeId] = empV.[EmpId]
								AND attd.[WorkDate] = empV.[WorkDate]
							)
					)
				)
			)
	ORDER BY gr.[GroupId]
		,emp.[Code]

	SELECT empV.[WorkDate]
		,empV.[EmpId] AS EmployeeId
		,empV.[ShiftName]
		,empV.[InFixed]
		,empV.[OutFixed]
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[Period] AS pe WITH (NOLOCK)
						INNER JOIN [dbo].[EPA] AS epa WITH (NOLOCK) ON pe.[PeriodId] = epa.[PeriodId]
					WHERE epa.[EmployeeId] = empV.[EmpId]
						AND CAST(pe.[EndDate] AS DATE) <= CAST(GETDATE() AS DATE)
						AND CAST(empV.[WorkDate] AS DATE) <= CAST(pe.[AttendanceEndDate] AS DATE)
						AND CAST(empV.[WorkDate] AS DATE) >= CAST(pe.[AttendanceStartDate] AS DATE)
						AND NOT EXISTS (
							SELECT 1
							FROM [dbo].[PeriodExcludedEmployee] WITH (NOLOCK)
							WHERE [PeriodId] = pe.[PeriodId]
								AND [EmployeeId] = epa.[EmployeeId]
							)
						AND pe.[IsActive] = 1
						AND epa.[Status] <> 2
					) THEN 1
			ELSE 0 END
			) AS OutOfDate
		,2 AS MissingCheckInOutId
		,empV.[MissingCheckInOut] AS Value
		,(
			SELECT [Score]
			FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
			WHERE [Id] = 2
			) AS Score
		,(CASE 
			WHEN empV.[MissingCheckInOut] <> 0
				AND empV.[MissingCheckInOutAutoIgnore] <> 1 THEN CASE 
					WHEN EXISTS (
							SELECT 1
							FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
							WHERE [GroupId] = emp.[GroupId]
								AND [ViolationId] = 2
								AND [Value] = 1
								AND [IsLocal] = emp.[IsLocal]
							) THEN 1
					ELSE 0
					END
			ELSE 0 END
			) AS HasViolated
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] WITH (NOLOCK)
					WHERE [WorkDate] = empV.[WorkDate]
						AND [EmployeeId] = empV.[EmpId]
						AND [ViolationId] = 2
					) THEN 1
			ELSE 0 END
			) AS HasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
					WHERE empS.[OwnerId] = @ApproverId
						AND empS.[EmpId] = emp.[EmployeeId]
						AND empS.[ActionType] = 3
						AND empS.[RequestType] IN (1, 2)
					)
				OR EXISTS (
					SELECT 1
					FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
					WHERE grS.[GroupId] = emp.[GroupId]
						AND grS.[OwnerId] = @ApproverId
						AND grS.[RequestType] IN (1, 2)
						AND grS.[ActionType] = 3
						AND emp.[EmployeeId] <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
							WHERE [OwnerId] <> @ApproverId
								AND [EmpId] = emp.[EmployeeId]
								AND [ActionType] = 3
								AND [RequestType] IN (1, 2)
							)
					) THEN 1
			ELSE 0 END
			) AS HasApprovedBy
		,7 AS TotalHoursId
		,empV.[TotalWorkHours] AS Value
		,(
			SELECT [Score]
			FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
			WHERE [Id] = 7
			) AS Score
		,(CASE 
			WHEN empV.[WorkingBelowTime] <> 0
				AND empV.[WorkingBelowTimeAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
					WHERE [GroupId] = emp.[GroupId]
						AND [ViolationId] = 7
						AND [Value] = 1
						AND [IsLocal] = emp.[IsLocal]
					) THEN 1
			ELSE 0 END
			) AS HasViolated
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] WITH (NOLOCK)
					WHERE [WorkDate] = empV.[WorkDate]
						AND [EmployeeId] = empV.[EmpId]
						AND [ViolationId] = 7
					) THEN 1
			ELSE 0 END
			) AS HasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
					WHERE empS.[OwnerId] = @ApproverId
						AND empS.[EmpId] = emp.[EmployeeId]
						AND empS.[ActionType] = 3
					)
				OR EXISTS (
					SELECT 1
					FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
					WHERE grS.[GroupId] = emp.[GroupId]
						AND grS.[OwnerId] = @ApproverId
						AND grS.[ActionType] = 3
						AND emp.[EmployeeId] <> @ApproverId
						AND NOT EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
							WHERE [OwnerId] <> @ApproverId
								AND [EmpId] = emp.[EmployeeId]
								AND [ActionType] = 3
							)
					) THEN 1
			ELSE 0 END
			) AS HasApprovedBy
		,3 AS LateInId
		,empV.[LateInValue] AS Value
		,(
			SELECT [Score]
			FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
			WHERE [Id] = 3
			) AS Score
		,(CASE 
			WHEN empV.[LateIn] <> 0
				AND empV.[LateInAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
					WHERE [GroupId] = emp.[GroupId]
						AND [ViolationId] = 3
						AND [Value] = 1
						AND [IsLocal] = emp.[IsLocal]
					) THEN 1
			ELSE 0 END
			) AS HasViolated
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] WITH (NOLOCK)
					WHERE [WorkDate] = empV.[WorkDate]
						AND [EmployeeId] = empV.[EmpId]
						AND [ViolationId] = 3
					) THEN 1
			ELSE 0 END
			) AS HasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
					WHERE empS.[OwnerId] = @ApproverId
						AND empS.[EmpId] = emp.[EmployeeId]
						AND empS.[ActionType] = 3
						AND empS.[RequestType] = 3 -- Late In
					)
				OR EXISTS (
					SELECT 1
					FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
					WHERE grS.[GroupId] = emp.[GroupId]
						AND grS.[OwnerId] = @ApproverId
						AND grS.[RequestType] = 3 -- Late In
						AND grS.[ActionType] = 3
						AND emp.[EmployeeId] <> @ApproverId
						AND NOT EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
							WHERE [OwnerId] <> @ApproverId
								AND [EmpId] = emp.[EmployeeId]
								AND [ActionType] = 3
								AND [RequestType] = 3
							)
					) THEN 1
			ELSE 0 END
			) AS HasApprovedBy
		,4 AS EarlyOutId
		,empV.[EarlyOutValue] AS Value
		,(
			SELECT [Score]
			FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
			WHERE [Id] = 4
			) AS Score
		,(CASE 
			WHEN empV.[EarlyOut] <> 0
				AND empV.[EarlyOutAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
					WHERE [GroupId] = emp.[GroupId]
						AND [ViolationId] = 4
						AND [Value] = 1
						AND [IsLocal] = emp.[IsLocal]
					) THEN 1
			ELSE 0 END
			) AS HasViolated
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] WITH (NOLOCK)
					WHERE [WorkDate] = empV.[WorkDate]
						AND [EmployeeId] = empV.[EmpId]
						AND [ViolationId] = 4
					) THEN 1
			ELSE 0 END
			) AS HasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
					WHERE empS.[OwnerId] = @ApproverId
						AND empS.[EmpId] = emp.[EmployeeId]
						AND empS.[ActionType] = 3
						AND empS.[RequestType] = 4 -- Early Out
					)
				OR EXISTS (
					SELECT 1
					FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
					WHERE grS.[GroupId] = emp.[GroupId]
						AND grS.[OwnerId] = @ApproverId
						AND grS.[RequestType] = 4 -- Early Out
						AND grS.[ActionType] = 3
						AND emp.[EmployeeId] <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
							WHERE [OwnerId] <> @ApproverId
								AND [EmpId] = emp.[EmployeeId]
								AND [ActionType] = 3
								AND [RequestType] = 4
							)
					) THEN 1
			ELSE 0 END
			) AS HasApprovedBy
	FROM [dbo].[HRMEmployeeAttendanceViolation] AS empV WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS emp WITH (NOLOCK) ON empV.[EmpId] = emp.[EmployeeId]
	WHERE emp.[Enabled] = 1
		AND emp.[IsDeleted] = 0
		AND CAST(empV.[WorkDate] AS DATE) >= CAST(@StartDate AS DATE)
		AND CAST(empV.[WorkDate] AS DATE) <= CAST(@EndDate AS DATE)
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR [EmpId] = @EmployeeId
			)
		AND (
			ISNULL(@ReasonId, '') = ''
			OR (
				@ReasonId = 1
				AND [ChangeShift] > 0
				AND [ChangeShiftAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 1 -- Change Shift
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			OR (
				@ReasonId = 2
				AND [MissingCheckInOut] > 0
				AND [MissingCheckInOutAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 2 -- Missing Check In Out
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			OR (
				@ReasonId = 3
				AND empV.[LateIn] > 0
				AND [LateInAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 3 -- Late In
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			OR (
				@ReasonId = 4
				AND empV.[EarlyOut] > 0
				AND [EarlyOutAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 4 -- Early Out
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			OR (
				@ReasonId = 5
				AND [Absent] > 0
				AND [AbsentAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 5 -- Absent
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			OR (
				@ReasonId = 6
				AND [UnpaidLeave] > 0
				AND [UnpaidLeaveAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 6 -- Unpaid Leave
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			OR (
				@ReasonId = 7
				AND [WorkingBelowTime] > 0
				AND [WorkingBelowTimeAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 7 -- Working Under Time
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			OR (
				@ReasonId = 8
				AND [UnpaidLeaveAbsent] > 0
				AND [UnpaidLeaveAbsentAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 8 -- Unpaid Leave Absent
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			)
		AND (
			ISNULL(@DepartmentId, '') = ''
			OR (
				@DepartmentId < 0
				AND emp.[GroupId] = - @DepartmentId
				)
			OR (
				@DepartmentId > 0
				AND EXISTS (
					SELECT 1
					FROM [dbo].[Department] as dept WITH (NOLOCK)
					WHERE [DepartmentId] = @DepartmentId
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Group] AS gr WITH (NOLOCK)
							WHERE [GroupId] = emp.[GroupId]
								AND dept.[lft] <= gr.[lft]
								AND dept.[rgt] >= gr.[rgt]
						)
					)
				)
			)
		AND (
			EXISTS (
				SELECT 1
				FROM [dbo].[EmployeeRole] AS empR
				WHERE [empR].[EmployeeId] = @ApproverId
					AND RoleId IN (1, 2, 3)
				)
			--OR EXISTS (
			--	SELECT 1
			--	FROM [dbo].[EmployeeRole] AS empR
			--		INNER JOIN [dbo].[Employee] AS empT ON empR.[EmployeeId] = empT.[EmployeeId]
			--	WHERE [empR].[EmployeeId] = @ApproverId
			--		AND RoleId IN (4, 5)
			--		AND empT.[GroupId] = emp.[GroupId]
			--	)
			OR EXISTS (
				SELECT 1
				FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
				WHERE empS.[OwnerId] = @ApproverId
					AND empS.[EmpId] = emp.[EmployeeId]
					AND empS.[ActionType] IN (2, 3)
				)
			OR EXISTS (
				SELECT 1
				FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
				WHERE emp.[GroupId] = grS.[GroupId]
					AND @ApproverId = grS.[OwnerId]
					AND grS.[ActionType] IN (2, 3) -- Approver and Verifier can view all member of section that they has role apporver or verifier
					AND NOT EXISTS (
						SELECT 1
						FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
						WHERE [OwnerId] <> @ApproverId
							AND [EmpId] = emp.[EmployeeId]
							AND [ActionType] IN (2, 3)
						)
				)
			OR EXISTS (
				SELECT 1
				FROM [dbo].[EmployeeRole] AS empR
				WHERE [empR].[EmployeeId] = @ApproverId
					AND RoleId = 6 -- Employee can view their discipline
					AND empV.[EmpId] = @ApproverId
				)
			)
		AND (
			(
				@ShowAll = 0
				AND (
					(
						empV.[MissingCheckInOut] > 0
						AND empV.[MissingCheckInOutAutoIgnore] <> 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
							WHERE [GroupId] = emp.[GroupId]
								AND [ViolationId] = 2
								AND [Value] = 1
								AND [IsLocal] = emp.[IsLocal]
							)
						)
					OR (
						empV.[MissingCheckInOut] = 0
						AND (
							(
								[Absent] > 0
								AND [AbsentAutoIgnore] <> 1
								AND EXISTS (
									SELECT 1
									FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
									WHERE [GroupId] = emp.[GroupId]
										AND [ViolationId] = 5
										AND [Value] = 1
										AND [IsLocal] = emp.[IsLocal]
									)
								)
							OR (
								[UnpaidLeave] > 0
								AND [UnpaidLeaveAutoIgnore] <> 1
								AND EXISTS (
									SELECT 1
									FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
									WHERE [GroupId] = emp.[GroupId]
										AND [ViolationId] = 6
										AND [Value] = 1
										AND [IsLocal] = emp.[IsLocal]
									)
								)
							OR (
								[UnpaidLeaveAbsent] > 0
								AND [UnpaidLeaveAbsentAutoIgnore] <> 1
								AND EXISTS (
									SELECT 1
									FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
									WHERE [GroupId] = emp.[GroupId]
										AND [ViolationId] = 8
										AND [Value] = 1
										AND [IsLocal] = emp.[IsLocal]
									)
								)
							OR (
								[ChangeShift] > 0
								AND [ChangeShiftAutoIgnore] <> 1
								AND EXISTS (
									SELECT 1
									FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
									WHERE [GroupId] = emp.[GroupId]
										AND [ViolationId] = 1
										AND [Value] = 1
										AND [IsLocal] = emp.[IsLocal]
									)
								)
							)
						)
					)
				OR (
					empV.[LateIn] > 0
					AND empV.[LateInAutoIgnore] <> 1
					AND EXISTS (
						SELECT 1
						FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
						WHERE [GroupId] = emp.[GroupId]
							AND [ViolationId] = 3
							AND [Value] = 1
							AND [IsLocal] = emp.[IsLocal]
						)
					)
				OR (
					empV.[EarlyOut] > 0
					AND empV.[EarlyOutAutoIgnore] <> 1
					AND EXISTS (
						SELECT 1
						FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
						WHERE [GroupId] = emp.[GroupId]
							AND [ViolationId] = 4
							AND [Value] = 1
							AND [IsLocal] = emp.[IsLocal]
						)
					)
				OR (
					empV.[WorkingBelowTime] > 0
					AND empV.[WorkingBelowTimeAutoIgnore] <> 1
					AND EXISTS (
						SELECT 1
						FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
						WHERE [GroupId] = emp.[GroupId]
							AND [ViolationId] = 7
							AND [Value] = 1
							AND [IsLocal] = emp.[IsLocal]
						)
					)
				)
			OR (
				@ShowAll = 1
				AND (
					[MissingCheckInOut] > 0
					OR [LateIn] > 0
					OR [EarlyOut] > 0
					OR [WorkingBelowTime] > 0
					)
				)
			)
		AND (
			@IsLocal IS NULL
			OR emp.[IsLocal] = @IsLocal
			)
		AND (
			@HasIgnored IS NULL
			OR (
				@HasIgnored = 0
				AND NOT EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] AS attd WITH (NOLOCK)
					WHERE attd.[EmployeeId] = empV.[EmpId]
						AND attd.[WorkDate] = empV.[WorkDate]
					)
				)
			OR (
				@HasIgnored = 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] AS attd WITH (NOLOCK)
					WHERE attd.[EmployeeId] = empV.[EmpId]
						AND attd.[WorkDate] = empV.[WorkDate]
					)
				)
			)
	ORDER BY empV.[WorkDate] DESC

	SELECT empV.[WorkDate]
		,empV.[EmpId] AS EmployeeId
		,empV.[ShiftName]
		,empV.[InFixed]
		,empV.[OutFixed]
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[Period] AS pe WITH (NOLOCK)
						INNER JOIN [dbo].[EPA] AS epa WITH (NOLOCK) ON pe.[PeriodId] = epa.[PeriodId]
					WHERE epa.[EmployeeId] = empV.[EmpId]
						AND CAST(pe.[EndDate] AS DATE) <= CAST(GETDATE() AS DATE)
						AND CAST(empV.[WorkDate] AS DATE) <= CAST(pe.[AttendanceEndDate] AS DATE)
						AND CAST(empV.[WorkDate] AS DATE) >= CAST(pe.[AttendanceStartDate] AS DATE)
						AND NOT EXISTS (
							SELECT 1
							FROM [dbo].[PeriodExcludedEmployee] WITH (NOLOCK)
							WHERE [PeriodId] = pe.[PeriodId]
								AND [EmployeeId] = epa.[EmployeeId]
							)
						AND pe.[IsActive] = 1
						AND epa.[Status] <> 2
					) THEN 1
			ELSE 0 END
			) AS OutOfDate
		,5 AS AbsentId
		,(CASE 
			WHEN empV.[Absent] > 0 THEN empV.[Absent]
			ELSE NULL END
			) AS Value
		,(
			SELECT [Score]
			FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
			WHERE [Id] = 5
			) AS Score
		,(CASE 
			WHEN empV.[Absent] <> 0
				AND empV.[AbsentAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
					WHERE [GroupId] = emp.[GroupId]
						AND [ViolationId] = 5
						AND [Value] = 1
						AND [IsLocal] = emp.[IsLocal]
					) THEN 1
			ELSE 0 END
			) AS HasViolated
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] WITH (NOLOCK)
					WHERE [WorkDate] = empV.[WorkDate]
						AND [EmployeeId] = empV.[EmpId]
						AND [ViolationId] = 5
					) THEN 1
			ELSE 0 END
			) AS HasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
					WHERE empS.[OwnerId] = @ApproverId
						AND empS.[EmpId] = emp.[EmployeeId]
						AND empS.[ActionType] = 3
						AND empS.[RequestType] = 6 -- leave
					)
				OR EXISTS (
					SELECT 1
					FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
					WHERE grS.[GroupId] = emp.[GroupId]
						AND grS.[OwnerId] = @ApproverId
						AND grS.[RequestType] = 6 -- leave
						AND grS.[ActionType] = 3
						AND emp.[EmployeeId] <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
							WHERE [OwnerId] <> @ApproverId
								AND [EmpId] = emp.[EmployeeId]
								AND [ActionType] = 3
								AND [RequestType] = 6
							)
					) THEN 1
			ELSE 0 END
			) AS HasApprovedBy
		,6 AS UnpaidLeaveId
		,(CASE 
			WHEN empV.[UnpaidLeave] > 0 THEN empV.[UnpaidLeave]
			ELSE NULL END
			) AS Value
		,(
			SELECT [Score]
			FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
			WHERE [Id] = 6
			) AS Score
		,(CASE 
			WHEN empV.[UnpaidLeave] <> 0
				AND empV.[UnpaidLeaveAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
					WHERE [GroupId] = emp.[GroupId]
						AND [ViolationId] = 6
						AND [Value] = 1
						AND [IsLocal] = emp.[IsLocal]
					) THEN 1
			ELSE 0 END
			) AS HasViolated
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] WITH (NOLOCK)
					WHERE [WorkDate] = empV.[WorkDate]
						AND [EmployeeId] = empV.[EmpId]
						AND [ViolationId] = 6
					) THEN 1
			ELSE 0 END
			) AS HasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
					WHERE empS.[OwnerId] = @ApproverId
						AND empS.[EmpId] = emp.[EmployeeId]
						AND empS.[ActionType] = 3
						AND empS.[RequestType] = 6 -- leave
					)
				OR EXISTS (
					SELECT 1
					FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
					WHERE grS.[GroupId] = emp.[GroupId]
						AND grS.[OwnerId] = @ApproverId
						AND grS.[RequestType] = 6 -- leave
						AND grS.[ActionType] = 3
						AND emp.[EmployeeId] <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
							WHERE [OwnerId] <> @ApproverId
								AND [EmpId] = emp.[EmployeeId]
								AND [ActionType] = 3
								AND [RequestType] = 6
							)
					) THEN 1
			ELSE 0 END
			) AS HasApprovedBy
		,1 AS ChangeShiftId
		,(CASE 
			WHEN empV.[ChangeShift] > 0 THEN empV.[ChangeShift]
			ELSE NULL END
			) AS Value
		,(
			SELECT [Score]
			FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
			WHERE [Id] = 1
			) AS Score
		,(CASE 
			WHEN empV.[ChangeShift] <> 0
				AND empV.[ChangeShiftAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
					WHERE [GroupId] = emp.[GroupId]
						AND [ViolationId] = 1
						AND [Value] = 1
						AND [IsLocal] = emp.[IsLocal]
					) THEN 1
			ELSE 0 END
			) AS HasViolated
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] WITH (NOLOCK)
					WHERE [WorkDate] = empV.[WorkDate]
						AND [EmployeeId] = empV.[EmpId]
						AND [ViolationId] = 1
					) THEN 1
			ELSE 0 END
			) AS HasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
					WHERE empS.[OwnerId] = @ApproverId
						AND empS.[EmpId] = emp.[EmployeeId]
						AND empS.[ActionType] = 3
						AND empS.[RequestType] = 5 -- Change shift
					)
				OR EXISTS (
					SELECT 1
					FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
					WHERE grS.[GroupId] = emp.[GroupId]
						AND grS.[OwnerId] = @ApproverId
						AND grS.[RequestType] = 5 -- Change shift
						AND grS.[ActionType] = 3
						AND emp.[EmployeeId] <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
							WHERE [OwnerId] <> @ApproverId
								AND [EmpId] = emp.[EmployeeId]
								AND [ActionType] = 3
								AND [RequestType] = 5
							)
					) THEN 1
			ELSE 0 END
			) AS HasApprovedBy
		,8 AS UnpaidLeaveAbsentId
		,(CASE 
			WHEN empV.[UnpaidLeaveAbsent] > 0 THEN empV.[UnpaidLeaveAbsent]
			ELSE NULL END
			) AS Value
		,(
			SELECT [Score]
			FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
			WHERE [Id] = 8
			) AS Score
		,(CASE 
			WHEN empV.[UnpaidLeaveAbsent] <> 0
				AND empV.[UnpaidLeaveAbsentAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
					WHERE [GroupId] = emp.[GroupId]
						AND [ViolationId] = 8
						AND [Value] = 1
						AND [IsLocal] = emp.[IsLocal]
					) THEN 1
			ELSE 0 END
			) AS HasViolated
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] WITH (NOLOCK)
					WHERE [WorkDate] = empV.[WorkDate]
						AND [EmployeeId] = empV.[EmpId]
						AND [ViolationId] = 8
					) THEN 1
			ELSE 0 END
			) AS HasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
					WHERE empS.[OwnerId] = @ApproverId
						AND empS.[EmpId] = emp.[EmployeeId]
						AND empS.[ActionType] = 3
						AND empS.[RequestType] = 6 -- leave
					)
				OR EXISTS (
					SELECT 1
					FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
					WHERE grS.[GroupId] = emp.[GroupId]
						AND grS.[OwnerId] = @ApproverId
						AND grS.[RequestType] = 6 -- leave
						AND grS.[ActionType] = 3
						AND emp.[EmployeeId] <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM [dbo].[HRMEmployeeSupervisor] WITH (NOLOCK)
							WHERE [OwnerId] <> @ApproverId
								AND [EmpId] = emp.[EmployeeId]
								AND [ActionType] = 3
								AND [RequestType] = 6
							)
					) THEN 1
			ELSE 0 END
			) AS HasApprovedBy
	FROM [dbo].[HRMEmployeeAttendanceViolation] AS empV WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS emp WITH (NOLOCK) ON empV.[EmpId] = emp.[EmployeeId]
	WHERE emp.[Enabled] = 1
		AND emp.[IsDeleted] = 0
		AND CAST(empV.[WorkDate] AS DATE) >= CAST(@StartDate AS DATE)
		AND CAST(empV.[WorkDate] AS DATE) <= CAST(@EndDate AS DATE)
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR [EmpId] = @EmployeeId
			)
		AND (
			ISNULL(@ReasonId, '') = ''
			OR (
				@ReasonId = 1
				AND [ChangeShift] > 0
				AND [ChangeShiftAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 1 -- Change Shift
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			OR (
				@ReasonId = 2
				AND [MissingCheckInOut] > 0
				AND [MissingCheckInOutAutoIgnore] <> 1
				)
			OR (
				@ReasonId = 3
				AND empV.[LateIn] > 0
				AND [LateInAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 3 -- Late In
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			OR (
				@ReasonId = 4
				AND empV.[EarlyOut] > 0
				AND [EarlyOutAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 4 -- Early Out
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			OR (
				@ReasonId = 5
				AND [Absent] > 0
				AND [AbsentAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 5 -- Absent
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			OR (
				@ReasonId = 6
				AND [UnpaidLeave] > 0
				AND [UnpaidLeaveAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 6 -- Unpaid Leave
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			OR (
				@ReasonId = 7
				AND [WorkingBelowTime] > 0
				AND [WorkingBelowTimeAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 7 -- Working Under Time
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			OR (
				@ReasonId = 8
				AND [UnpaidLeaveAbsent] > 0
				AND [UnpaidLeaveAbsentAutoIgnore] <> 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceSectionRule] AS attS WITH (NOLOCK)
					WHERE [ViolationId] = 8 -- Unpaid Leave Absent
						AND [Value] = 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Employee] WITH (NOLOCK)
							WHERE [GroupId] = attS.[GroupId]
								AND [EmployeeId] = empV.[EmpId]
								AND [IsLocal] = attS.[IsLocal]
							)
					)
				)
			)
		AND (
			ISNULL(@DepartmentId, '') = ''
			OR (
				@DepartmentId < 0
				AND emp.[GroupId] = - @DepartmentId
				)
			OR (
				@DepartmentId > 0
				AND EXISTS (
					SELECT 1
					FROM [dbo].[Department] AS dept WITH (NOLOCK)
					WHERE [DepartmentId] = @DepartmentId
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Group] AS gr WITH (NOLOCK)
							WHERE gr.[GroupId] = emp.[GroupId]
								AND dept.[lft] <= gr.[lft]
								AND dept.[rgt] >= gr.[rgt]
						)
					)
				)
			)
		AND (
			EXISTS (
				SELECT 1
				FROM [dbo].[EmployeeRole] AS empR
				WHERE [empR].[EmployeeId] = @ApproverId
					AND RoleId IN (1, 2, 3)
				)
			--OR EXISTS (
			--	SELECT 1
			--	FROM [dbo].[EmployeeRole] AS empR
			--		INNER JOIN [dbo].[Employee] AS empT ON empR.[EmployeeId] = empT.[EmployeeId]
			--	WHERE [empR].[EmployeeId] = @ApproverId
			--		AND RoleId IN (4, 5)
			--		AND empT.[GroupId] = emp.[GroupId]
			--	)
			OR EXISTS (
				SELECT 1
				FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
				WHERE empS.[OwnerId] = @ApproverId
					AND empS.[EmpId] = emp.[EmployeeId]
					AND empS.[ActionType] IN (2, 3)
				)
			OR EXISTS (
				SELECT 1
				FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
				WHERE emp.[GroupId] = grS.[GroupId]
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
				SELECT 1
				FROM [dbo].[EmployeeRole] AS empR
				WHERE [empR].[EmployeeId] = @ApproverId
					AND RoleId = 6 -- Employee can view their discipline
					AND empV.[EmpId] = @ApproverId
				)
			)
		AND (
			(
				@ShowAll = 0
				AND (
					empV.[Absent] > 0
					AND empV.[AbsentAutoIgnore] <> 1
					AND EXISTS (
						SELECT 1
						FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
						WHERE [GroupId] = emp.[GroupId]
							AND [ViolationId] = 5
							AND [Value] = 1
							AND [IsLocal] = emp.[IsLocal]
						)
					)
				OR (
					(
						empV.[ChangeShift] > 0
						AND empV.[ChangeShiftAutoIgnore] <> 1
						AND EXISTS (
							SELECT 1
							FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
							WHERE [GroupId] = emp.[GroupId]
								AND [ViolationId] = 1
								AND [Value] = 1
								AND [IsLocal] = emp.[IsLocal]
							)
						)
					OR (
						(
							empV.[LateIn] > 0
							AND empV.[LateInAutoIgnore] <> 1
							AND EXISTS (
								SELECT 1
								FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
								WHERE [GroupId] = emp.[GroupId]
									AND [ViolationId] = 3
									AND [Value] = 1
									AND [IsLocal] = emp.[IsLocal]
								)
							)
						OR (
							empV.[EarlyOut] > 0
							AND empV.[EarlyOutAutoIgnore] <> 1
							AND EXISTS (
								SELECT 1
								FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
								WHERE [GroupId] = emp.[GroupId]
									AND [ViolationId] = 4
									AND [Value] = 1
									AND [IsLocal] = emp.[IsLocal]
								)
							)
						OR (
							empV.[WorkingBelowTime] > 0
							AND empV.[WorkingBelowTimeAutoIgnore] <> 1
							AND EXISTS (
								SELECT 1
								FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
								WHERE [GroupId] = emp.[GroupId]
									AND [ViolationId] = 7
									AND [Value] = 1
									AND [IsLocal] = emp.[IsLocal]
								)
							)
						)
					)
				OR (
					empV.[UnpaidLeave] > 0
					AND empV.[UnpaidLeaveAutoIgnore] <> 1
					AND EXISTS (
						SELECT 1
						FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
						WHERE [GroupId] = emp.[GroupId]
							AND [ViolationId] = 6
							AND [Value] = 1
							AND [IsLocal] = emp.[IsLocal]
						)
					)
				OR (
					empV.[UnpaidLeaveAbsent] > 0
					AND empV.[UnpaidLeaveAbsentAutoIgnore] <> 1
					AND EXISTS (
						SELECT 1
						FROM [dbo].[AttendanceSectionRule] WITH (NOLOCK)
						WHERE [GroupId] = emp.[GroupId]
							AND [ViolationId] = 8
							AND [Value] = 1
							AND [IsLocal] = emp.[IsLocal]
						)
					)
				)
			OR (
				@ShowAll = 1
				AND (
					[Absent] > 0
					OR [UnpaidLeave] > 0
					OR [UnpaidLeaveAbsent] > 0
					)
				)
			)
		AND (
			@IsLocal IS NULL
			OR emp.[IsLocal] = @IsLocal
			)
		AND (
			@HasIgnored IS NULL
			OR (
				@HasIgnored = 0
				AND NOT EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] AS attd WITH (NOLOCK)
					WHERE attd.[EmployeeId] = empV.[EmpId]
						AND attd.[WorkDate] = empV.[WorkDate]
					)
				)
			OR (
				@HasIgnored = 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] AS attd WITH (NOLOCK)
					WHERE attd.[EmployeeId] = empV.[EmpId]
						AND attd.[WorkDate] = empV.[WorkDate]
					)
				)
			)
	ORDER BY empV.[WorkDate] DESC

	SELECT empV.[WorkDate]
		,empV.[EmpId] AS EmployeeId
		,ApproverComment = 'Missing Check In/Out_ ' + ISNULL(empV.[InOutMissApproverComment], '') + '& Late In_ ' + ISNULL(empV.[LateInApproverComment], '') + '& Early Out_ ' + ISNULL(empV.[EarlyOutApproverComment], '') + '& Change Shift_ ' + ISNULL(empV.[ChangeShiftApproverComment], '') + '& Leave_ ' + ISNULL(empV.[LeaveApproverComment], '')
		,EmployeeComment = 'Missing Check In/Out_ ' + ISNULL(empV.[InOutMissEmployeeComment], '') + '& Late In_ ' + ISNULL(empV.[LateInEmployeeComment], '') + '& Early Out_ ' + ISNULL(empV.[EarlyOutEmployeeComment], '') + '& Change Shift_ ' + ISNULL(empV.[ChangeShiftEmployeeComment], '') + '& Leave_ ' + ISNULL(empV.[LeaveEmployeeComment], '')
	FROM [dbo].[HRMEmployeeAttendanceViolation] AS empV WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS emp WITH (NOLOCK) ON empV.[EmpId] = emp.[EmployeeId]
	WHERE emp.[Enabled] = 1
		AND emp.[IsDeleted] = 0
		AND CAST(empV.[WorkDate] AS DATE) >= CAST(@StartDate AS DATE)
		AND CAST(empV.[WorkDate] AS DATE) <= CAST(@EndDate AS DATE)
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR [EmpId] = @EmployeeId
			)
		AND (
			ISNULL(@DepartmentId, '') = ''
			OR (
				@DepartmentId < 0
				AND emp.[GroupId] = - @DepartmentId
				)
			OR (
				@DepartmentId > 0
				AND EXISTS (
					SELECT 1
					FROM [dbo].[Department] AS dept WITH (NOLOCK)
					WHERE [DepartmentId] = @DepartmentId
						AND EXISTS (
							SELECT 1
							FROM [dbo].[Group] AS gr WITH (NOLOCK)
							WHERE gr.[GroupId] = emp.GroupId
								AND dept.[lft] <= gr.[lft]
								AND dept.[rgt] >= gr.[rgt] 
						)
					)
				)
			)
		AND (
			EXISTS (
				SELECT 1
				FROM [dbo].[EmployeeRole] AS empR
				WHERE [empR].[EmployeeId] = @ApproverId
					AND RoleId IN (1, 2, 3)
				)
			OR EXISTS (
				SELECT 1
				FROM [dbo].[HRMEmployeeSupervisor] AS empS WITH (NOLOCK)
				WHERE empS.[OwnerId] = @ApproverId
					AND empS.[EmpId] = emp.[EmployeeId]
					AND empS.[ActionType] IN (2, 3)
				)
			OR EXISTS (
				SELECT 1
				FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
				WHERE emp.[GroupId] = grS.[GroupId]
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
				SELECT 1
				FROM [dbo].[EmployeeRole] AS empR
				WHERE [empR].[EmployeeId] = @ApproverId
					AND RoleId = 6 -- Employee can view their discipline
					AND empV.[EmpId] = @ApproverId
				)
			)
		AND (
			@IsLocal IS NULL
			OR emp.[IsLocal] = @IsLocal
			)
		AND (
			@HasIgnored IS NULL
			OR (
				@HasIgnored = 0
				AND NOT EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] AS attd WITH (NOLOCK)
					WHERE attd.[EmployeeId] = empV.[EmpId]
						AND attd.[WorkDate] = empV.[WorkDate]
					)
				)
			OR (
				@HasIgnored = 1
				AND EXISTS (
					SELECT 1
					FROM [dbo].[AttendanceDetailIgnored] AS attd WITH (NOLOCK)
					WHERE attd.[EmployeeId] = empV.[EmpId]
						AND attd.[WorkDate] = empV.[WorkDate]
					)
				)
			)
	ORDER BY empV.[WorkDate] DESC
END