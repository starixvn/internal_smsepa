/****** Object:  Stored Procedure dbo.EpaEmployee_GetByDomainUsername    Script Date: 5/16/2016 2:20:07 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_GetByDomainUsername    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_GetByDomainUsername    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_GetByDomainUsername    Script Date: 4/6/2016 11:27:44 AM ******/

CREATE PROCEDURE dbo.EpaEmployee_GetByDomainUsername --'sms\thuthi'
	@DomainUsername NVARCHAR(50) = NULL
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get user by domain user name.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmployeeId INT

	SET @EmployeeId = (
			SELECT EmployeeId AS EmployeeId
			FROM dbo.Employee
			WHERE DomainUsername = @DomainUsername
				AND [Enabled] = 1
				AND IsDeleted = 0
			)

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
	WHERE emp.DomainUsername = @DomainUsername
		AND emp.Enabled = 1
		AND emp.IsDeleted = 0

	-- role
	SELECT emRo.RoleId
		,loDa.Code AS RoleCode
	FROM dbo.EmployeeRole AS emRo WITH (NOLOCK)
		INNER JOIN dbo.LookupData loDa WITH (NOLOCK) ON emRo.RoleId = loDa.Value
	WHERE emRO.EmployeeId = @EmployeeId
		AND loDa.LookupId = 2 -- System Role

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