/****** Object:  Stored Procedure dbo.EpaEmployee_Get    Script Date: 7/21/2016 3:01:37 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_Get    Script Date: 7/21/2016 2:45:50 PM ******/

CREATE PROCEDURE [dbo].[EpaEmployee_Get] @Query NVARCHAR(100) = NULL
	,@GroupId INT = NULL
	,@IsLocal BIT = NULL
	,@IsSpecialUser BIT = NULL
	,@RoleId INT = NULL
	,@IsDisabledUser BIT = NULL
	,@ApproverId INT = NULL
	,@IncludeVerifier BIT = NULL
	,@HasMonthlyEPA BIT = NULL
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get users.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT emp.[EmployeeId]
		,emp.[Code]
		,emp.[DomainUsername]
		,emp.[NickName]
		,emp.[FullName]
		,emp.[IsSpecialUser]
		,emp.[Password]
		,emp.[Enabled]
		,emp.[IsLocal]
		,emp.[Email]
		,emp.HasMonthlyEPA
		,emp.[JobTitle] AS JobTitleId
		,job.[Name] AS JobTitleName
		,empGr.[GroupId]
		,empGr.[Name] AS GroupName
		,empGr.[Code] AS GroupCode
	FROM [dbo].[Employee] AS emp WITH (NOLOCK)
		LEFT JOIN [dbo].[Group] AS empGr WITH (NOLOCK) ON emp.[GroupId] = empGr.[GroupId]
		LEFT JOIN [dbo].[HRMJobTitle] AS job WITH (NOLOCK) ON emp.[JobTitle] = job.[Id]
		LEFT JOIN [dbo].[EmployeeRole] AS empRo WITH (NOLOCK) ON emp.[EmployeeId] = empRo.[EmployeeId]
	WHERE emp.[IsDeleted] = 0
		AND (
			ISNULl(@GroupId, '') = ''
			OR empGr.[GroupId] = @GroupId
			)
		AND (
			ISNULL(@Query, '') = ''
			OR emp.[FullName] LIKE '%' + @Query + '%'
			OR emp.[NickName] LIKE '%' + @Query + '%'
			)
		AND (
			@IsLocal IS NULL
			OR emp.IsLocal = @IsLocal
			)
		AND (
			ISNULL(@IsSpecialUser, '') = ''
			OR emp.[IsSpecialUser] = @IsSpecialUser
			)
		AND (
			(
				ISNULL(@IsDisabledUser, '') = ''
				AND [emp].[Enabled] = 1
				)
			OR (
				ISNULL(@IsDisabledUser, '') <> ''
				AND emp.[Enabled] = ~ @IsDisabledUser
				)
			)
		AND (
			ISNULL(@ApproverId, '') = ''
			OR (
				ISNULL(@IncludeVerifier, '') = ''
				AND EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
					WHERE empGr.[GroupId] = grS.[GroupId]
						AND @ApproverId = grS.[OwnerId]
						AND grS.[ActionType] = 3 -- Approver
					)
				)
			OR (
				ISNULL(@IncludeVerifier, '') <> ''
				AND (
					EXISTS (
						SELECT TOP 1 1
						FROM [dbo].[EmployeeRole] AS empR
						WHERE [empR].[EmployeeId] = @ApproverId
							AND RoleId IN (1, 2, 3)
						)
					OR EXISTS (
						SELECT TOP 1 1
						FROM [dbo].[HRMGroupSupervisor] AS grS WITH (NOLOCK)
						WHERE empGr.[GroupId] = grS.[GroupId]
							AND @ApproverId = grS.[OwnerId]
							AND grS.[ActionType] IN (2, 3) -- Approver and Verfier
						)
					)
				)
			)
		AND (
			@HasMonthlyEPA IS NULL
			OR emp.HasMonthlyEPA = @HasMonthlyEPA
			)

	SELECT ro.[EmployeeId]
		,ro.[RoleId]
		,lo.[Code] AS RoleCode
	FROM [dbo].[EmployeeRole] AS ro WITH (NOLOCK)
		INNER JOIN [dbo].[LookupData] AS lo WITH (NOLOCK) ON ro.[RoleId] = lo.[Value]
	WHERE lo.[LookupId] = 2
		AND (
			ISNULL(@RoleId, '') = ''
			OR [EmployeeId] IN (
				SELECT [EmployeeId]
				FROM [dbo].[EmployeeRole] WITH (NOLOCK)
				WHERE [RoleId] = @RoleId
				)
			)
END