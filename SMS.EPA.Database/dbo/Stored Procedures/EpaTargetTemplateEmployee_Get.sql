
CREATE PROCEDURE [dbo].[EpaTargetTemplateEmployee_Get] @GroupId INT = NULL
	,@EmployeeId INT = NULL
	,@TargetTypeId INT = NULL
	,@ShowAll BIT = 0
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get users and their target template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.[Enabled]
		,gr.GroupId
		,gr.[Name] AS GroupName
		,gr.Code AS GroupCode
	FROM dbo.Employee AS emp WITH (NOLOCK)
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND (
			ISNULL(@GroupId, '') = ''
			OR gr.[GroupId] = @GroupId
			)
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR emp.EmployeeId = @EmployeeId
			)
		AND (
			(
				@ShowAll = 0
				AND emp.EmployeeId IN (
					SELECT DISTINCT EmployeeId
					FROM dbo.TargetTemplateEmployee WITH (NOLOCK)
					)
				)
			OR @ShowAll = 1
			)
		AND (
			ISNULL(@TargetTypeId, '') = ''
			OR emp.[EmployeeId] IN (
				SELECT emp.[EmployeeId]
				FROM [dbo].[Employee] AS emp WITH (NOLOCK)
					INNER JOIN [dbo].[TargetTemplateEmployee] AS tarEmp WITH (NOLOCK) ON emp.[EmployeeId] = tarEmp.[EmployeeId]
					INNER JOIN [dbo].[TargetTemplate] AS tarTemp WITH (NOLOCK) ON tarEmp.[TargetTemplateId] = tarTemp.[TargetTemplateId]
					INNER JOIN [dbo].[TargetType] AS tar WITH (NOLOCK) ON tarTemp.[TargetTypeId] = tar.[TargetTypeId]
				WHERE tar.[TargetTypeId] = @TargetTypeId
				)
			)
	ORDER BY emp.[EmployeeId]

	SELECT tarEmp.[EmployeeId]
		,tarEmp.[TargetTemplateId]
		,tar.[Name]
		,tar.[TargetTypeId]
		,tarTy.[Name] AS TargetTypeName
	FROM [dbo].[TargetTemplateEmployee] AS tarEmp WITH (NOLOCK)
		INNER JOIN [dbo].[TargetTemplate] AS tar WITH (NOLOCK) ON tarEmp.[TargetTemplateId] = tar.[TargetTemplateId]
		INNER JOIN [dbo].[TargetType] AS tarTy WITH (NOLOCK) ON tar.[TargetTypeId] = tarTy.[TargetTypeId]
	WHERE (
			ISNULL(@EmployeeId, '') = ''
			OR tarEmp.[EmployeeId] = @EmployeeId
			)
	ORDER BY tarEmp.[EmployeeId]
		,tar.[TargetTypeId]
END