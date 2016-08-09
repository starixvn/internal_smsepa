CREATE PROCEDURE [dbo].[Login_GetEmployee] @Code NVARCHAR(20) = NULL
	,@DomainUsername NVARCHAR(50) = NULL
AS
/*
Created: 2016-08-09@Tim.Trinh
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
	FROM dbo.Employee WITH (NOLOCK)
	WHERE (
			Code = @Code
			OR DomainUsername = @DomainUsername
			)
		AND [Enabled] = 1
		AND IsDeleted = 0

	-- general info
	SELECT DISTINCT emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.[Password]
		,emp.IsSpecialUser
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE emp.EmployeeId = @EmployeeId

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