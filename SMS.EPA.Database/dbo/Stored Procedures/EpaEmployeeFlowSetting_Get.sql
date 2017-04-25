

CREATE PROCEDURE [dbo].[EpaEmployeeFlowSetting_Get] @FlowTemplateId INT
	,@DepartmentId INT = NULL
	,@GroupId INT = NULL
	,@IsMonthlyPeriod BIT = NULL
AS
/*
Created: 20150807@Tim.Trinh
Task: Get employees
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	IF @DepartmentId IS NOT NULL
	BEGIN
		SELECT e.EmployeeId
			,e.Code
			,DomainUsername
			,FullName
			,NickName
			,Email
			,(CASE 
				WHEN e.HasMonthlyEPA = 1 THEN 1
				ELSE 0 END
				) AS HasMonthlyEPA
			,(CASE 
				WHEN ef.EmployeeId IS NULL THEN 0
				ELSE 1 END
				) AS IsExistsSetting
		FROM dbo.Employee AS e WITH (NOLOCK)
			INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
			INNER JOIN (
			SELECT DepartmentId
			FROM dbo.RelatedDepartment(@DepartmentId)
			
			UNION
			SELECT @DepartmentId
			) AS d ON g.DepartmentId = d.DepartmentId
			LEFT JOIN dbo.SettingEmployeeFlow AS ef WITH (NOLOCK) ON e.EmployeeId = ef.EmployeeId
			AND ef.FlowTemplateId = @FlowTemplateId
			AND ef.ProcessId = 2
		WHERE ISNULL(e.IsDeleted, 0) = 0
			AND e.[Enabled] = 1
			AND (
				@IsMonthlyPeriod IS NULL
				OR e.HasMonthlyEPA = @IsMonthlyPeriod
				)
		ORDER BY e.Code
	END
	ELSE IF @GroupId IS NOT NULL
	BEGIN
		SELECT e.EmployeeId
			,e.Code
			,DomainUsername
			,FullName
			,NickName
			,Email
			,(CASE 
				WHEN e.HasMonthlyEPA = 1 THEN 1
				ELSE 0 END
				) AS HasMonthlyEPA
			,(CASE 
				WHEN ef.EmployeeId IS NULL THEN 0
				ELSE 1 END
				) AS IsExistsSetting
		FROM dbo.Employee AS e WITH (NOLOCK)
			LEFT JOIN dbo.SettingEmployeeFlow AS ef WITH (NOLOCK) ON e.EmployeeId = ef.EmployeeId
			AND ef.FlowTemplateId = @FlowTemplateId
			AND ef.ProcessId = 2
		WHERE GroupId = @GroupId
			AND ISNULL(e.IsDeleted, 0) = 0
			AND e.[Enabled] = 1
			AND (
				@IsMonthlyPeriod IS NULL
				OR e.HasMonthlyEPA = @IsMonthlyPeriod
				)
		ORDER BY Code
	END
	ELSE
	BEGIN
		SELECT NULL AS EmployeeId
			,NULL AS Code
			,NULL AS DomainUsername
			,NULL AS FullName
			,NULL AS NickName
			,NULL AS Email
			,NULL AS HasMonthlyEPA
			,NULL AS IsExistsSetting
		WHERE 0 = 1
	END
END