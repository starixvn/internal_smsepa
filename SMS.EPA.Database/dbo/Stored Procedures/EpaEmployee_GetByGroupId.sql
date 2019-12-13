
CREATE PROCEDURE [dbo].[EpaEmployee_GetByGroupId] @GroupId INT
AS
/*
Created: 20150722@Tim.Trinh
Task: Get employees by group
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.[EmployeeId]
		,e.[Code]
		,[DomainUsername]
		,[FullName]
		,[NickName]
		,[Email]
		,e.HasMonthlyEPA
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE GroupId = @GroupId
		AND ISNULL(e.IsDeleted, 0) = 0
		AND e.[Enabled] = 1
	ORDER BY Code ASC
END