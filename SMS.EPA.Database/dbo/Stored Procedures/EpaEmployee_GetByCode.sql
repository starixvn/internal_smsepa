
CREATE PROCEDURE [dbo].[EpaEmployee_GetByCode] @Code NVARCHAR(20) = NULL
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get user by code.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmployeeId INT

	SELECT @EmployeeId = EmployeeId
	FROM dbo.Employee
	WHERE Code = @Code
		AND [Enabled] = 1
		AND IsDeleted = 0

	-- general info
	SELECT DISTINCT emp.EmployeeId
		,emp.Code
		,emp.DomainUsername
		,emp.NickName
		,emp.FullName
		,emp.[Password]
		,emp.IsSpecialUser
		,emp.[Enabled]
		,emp.IsLocal
		,emp.JoinedDate
		,emp.HasMonthlyEPA
		,emp.JobTitle AS JobTitleId
		,job.[Name] AS JobTitleName
		,dept.DepartmentId
		,dept.Code
		,dept.[Name]
		,empGr.GroupId
		,empGr.[Name] AS GroupName
		,empGr.Code AS GroupCode
	FROM dbo.Employee AS emp WITH (NOLOCK)
		LEFT JOIN dbo.[Group] AS empGr WITH (NOLOCK) ON emp.GroupId = empGr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON empGr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
	WHERE emp.Code = @Code
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0

	-- role
	SELECT ro.RoleId
		,ref.Value AS RoleCode
	FROM dbo.EmployeeRole AS ro WITH (NOLOCK)
		INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON ro.RoleId = ref.ResourceId
		AND ref.RefName = 'SystemRole'
	WHERE ro.EmployeeId = @EmployeeId

	SELECT f.FunctionId
		,f.FunctionKey
		,CAST(MAX(IsRead) AS BIT) AS IsRead
		,CAST(MAX(IsHost) AS BIT) AS IsHost
	FROM (
		SELECT RoleId
			,FunctionId
			,CAST(IsRead AS INT) AS IsRead
			,CAST(IsHost AS INT) AS IsHost
		FROM dbo.RoleFunction WITH (NOLOCK)
		) AS rf
		INNER JOIN dbo.FunctionKey AS f WITH (NOLOCK) ON rf.FunctionId = f.FunctionId
		INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON rf.RoleId = er.RoleId
	WHERE er.EmployeeId = @EmployeeId
	GROUP BY f.FunctionId
		,FunctionKey

	-- menu
	SELECT DISTINCT me.MenuId
		,me.[Name] AS MenuName
		,me.Url AS MenuLink
		,me.ParentId
		,me.FunctionId
		,me.DisplayOrder
		,me.IsActive
	FROM dbo.EmployeeRole AS empRo WITH (NOLOCK)
		INNER JOIN dbo.RoleFunction AS roFunc WITH (NOLOCK) ON empRo.RoleId = roFunc.RoleId
		AND roFunc.IsRead = 1
		INNER JOIN dbo.Menu AS me WITH (NOLOCK) ON roFunc.FunctionId = me.FunctionId
		AND me.IsActive = 1
	WHERE empRo.EmployeeId = @EmployeeId
END