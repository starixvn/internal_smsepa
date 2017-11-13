
CREATE PROCEDURE [dbo].[EpaEmployee_Get] @Query NVARCHAR(100) = NULL
	,@GroupId INT = NULL
	,@DepartmentId INT = NULL
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

	CREATE TABLE #tblEmployee (EmployeeId INT)

	INSERT INTO #tblEmployee (EmployeeId)
	SELECT DISTINCT emp.EmployeeId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE emp.IsDeleted = 0
		AND ISNULl(@GroupId, gr.GroupId) = gr.GroupId
		AND (
			@Query IS NULL
			OR emp.FullName LIKE '%' + @Query + '%'
			OR emp.NickName LIKE '%' + @Query + '%'
			)
		AND ISNULL(@IsLocal, emp.IsLocal) = emp.IsLocal
		AND ISNULL(@IsSpecialUser, emp.IsSpecialUser) = emp.IsSpecialUser
		AND (
			@DepartmentId IS NULL
			OR (
				@DepartmentId < 0
				AND gr.GroupId = - @DepartmentId
				)
			OR (
				@DepartmentId > 0
				AND EXISTS (
					SELECT 1
					FROM dbo.Department AS dept WITH (NOLOCK)
					WHERE dept.DepartmentId = @DepartmentId
						AND dept.lft <= gr.lft
						AND dept.rgt >= gr.rgt
					)
				)
			)
		AND (
			(
				@IsDisabledUser IS NULL
				AND emp.[Enabled] = 1
				)
			OR (
				@IsDisabledUser IS NOT NULL
				AND emp.[Enabled] = ~ @IsDisabledUser
				)
			)
		AND (
			@ApproverId IS NULL
			OR (
				@IncludeVerifier IS NULL
				AND EXISTS (
					SELECT 1
					FROM dbo.HRMGroupSupervisor AS grS WITH (NOLOCK)
					WHERE gr.GroupId = grS.GroupId
						AND @ApproverId = grS.OwnerId
						AND grS.ActionType = 3 -- Approver
					)
				)
			OR (
				@IncludeVerifier IS NOT NULL
				AND (
					EXISTS (
						SELECT 1
						FROM dbo.EmployeeRole AS empR
						WHERE empR.EmployeeId = @ApproverId
							AND RoleId IN (1, 2, 3)
						)
					OR EXISTS (
						SELECT 1
						FROM dbo.HRMGroupSupervisor AS grS WITH (NOLOCK)
						WHERE gr.GroupId = grS.GroupId
							AND @ApproverId = grS.OwnerId
							AND grS.ActionType IN (2, 3) -- Approver and Verfier
						)
					)
				)
			)
		AND (
			@HasMonthlyEPA IS NULL
			OR emp.HasMonthlyEPA = @HasMonthlyEPA
			)

	SELECT DISTINCT emp.EmployeeId
		,emp.Code
		,emp.DomainUsername
		,emp.NickName
		,emp.FullName
		,emp.IsSpecialUser
		,emp.[Password]
		,emp.[Enabled]
		,emp.IsLocal
		,(CASE 
			WHEN emp.IsLocal = 1 THEN 'Local'
			ELSE 'Expat' END
			) AS Category
		,emp.[Email]
		,emp.HasMonthlyEPA
		,emp.JoinedDate
		,emp.JobTitle AS JobTitleId
		,job.[Name] AS JobTitleName
		,gr.GroupId
		,gr.[Name] AS GroupName
		,gr.Code AS GroupCode
		,dept.DepartmentId
		,dept.Code
		,dept.[Name]
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblEmployee AS tbl WITH (NOLOCK) ON emp.EmployeeId = tbl.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id

	SELECT ro.EmployeeId
		,ro.RoleId
		,ref.Value AS RoleCode
	FROM dbo.EmployeeRole AS ro WITH (NOLOCK)
		INNER JOIN #tblEmployee AS emp WITH (NOLOCK) ON ro.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON ro.RoleId = ref.ResourceId
		AND ref.RefName = 'SystemRole'
	WHERE @RoleId IS NULL
		OR ro.EmployeeId IN (
			SELECT EmployeeId
			FROM dbo.EmployeeRole WITH (NOLOCK)
			WHERE RoleId = @RoleId
			)

	DROP TABLE #tblEmployee
END