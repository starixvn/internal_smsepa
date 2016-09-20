
CREATE PROCEDURE [dbo].[TargetTemplateEmployee_Get] @GroupId INT = NULL
	,@EmployeeId INT = NULL
	,@TargetTypeId INT = NULL
	,@ShowAll BIT = NULL
AS
/*
Created: 2016-09-13@Quinn.Vo
Task: Get employee target template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT 
		tar.RatingTypeId
		,emp.[EmployeeId]
		,emp.[Code]
		,emp.[NickName]
		,emp.[FullName]
		,emp.[Enabled]
		,empGr.[GroupId]
		,empGr.[Code] AS GroupCode
		,empGr.[Name] AS GroupName
		,tarEmp.[TargetTemplateId]
		,tar.[Name]
		,tar.[TargetTypeId]
		,tarTy.[Name] AS TargetTypeName
	FROM [dbo].[Employee] AS emp WITH (NOLOCK)
		LEFT JOIN [dbo].[Group] AS empGr WITH (NOLOCK) ON emp.[GroupId] = empGr.[GroupId]
		LEFT JOIN [dbo].[TargetTemplateEmployee] AS tarEmp WITH (NOLOCK) ON emp.EmployeeId = tarEmp.EmployeeId
		INNER JOIN [dbo].[TargetTemplate] AS tar WITH (NOLOCK) ON tarEmp.[TargetTemplateId] = tar.[TargetTemplateId]
		INNER JOIN [dbo].[TargetType] AS tarTy WITH (NOLOCK) ON tar.[TargetTypeId] = tarTy.[TargetTypeId]
	WHERE emp.[Enabled] = 1
		AND emp.[IsDeleted] = 0
		AND (
			ISNULL(@GroupId, '') = ''
			OR empGr.[GroupId] = @GroupId
			)
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR emp.[EmployeeId] = @EmployeeId
			)
		AND (
			(
				ISNULl(@ShowAll, '') = ''
				AND emp.[EmployeeId] IN (
					SELECT DISTINCT [EmployeeId]
					FROM [dbo].[TargetTemplateEmployee] WITH (NOLOCK)
					)
				)
			OR @ShowAll = 1
			)
		AND (
			ISNULL(@TargetTypeId, '') = ''
			OR ISNULL(tar.TargetTypeId, 0) = @TargetTypeId
			)
	ORDER BY emp.[EmployeeId]

END