
CREATE PROCEDURE [dbo].[EpaEmployee_GetByDepartmentId] @DepartmentId INT = NULL
AS
/*
Created: 20150722@Tim.Trinh
Task: Get employees by department
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF @DepartmentId IS NULL
	BEGIN
		SET @DepartmentId = 1 -- root
	END

	SELECT e.[EmployeeId]
		,e.[Code]
		,[DomainUsername]
		,[FullName]
		,[NickName]
		,[Email]
		,e.HasMonthlyEPA
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		INNER JOIN (
		SELECT DepartmentId
		FROM dbo.RelatedDepartment(@DepartmentId)
		
		UNION
		SELECT @DepartmentId
		) AS d ON g.DepartmentId = d.DepartmentId
	WHERE ISNULL(e.IsDeleted, 0) = 0
		AND e.[Enabled] = 1
	ORDER BY e.Code ASC
END