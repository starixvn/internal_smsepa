
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
END