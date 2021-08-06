
CREATE PROCEDURE [dbo].[JobDescription_GetEmployeeSettingById] @EmployeeId INT
AS
/*
Created: 20210426@Tim.Trinh
Task: job description - get employee setting by id
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT emp.EmployeeId
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,gr.[Name] AS GroupName
		,gr.Code AS GroupCode
		,job.JobLevelId
		,job.[Name] AS JobLevel
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.JobLevel AS job WITH (NOLOCK) ON emp.JobLevelId = job.JobLevelId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND emp.EmployeeId = @EmployeeId

	SELECT gitem.JobDescription_GroupItemId AS GroupItemId
		,gitem.[Name]
	FROM dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gitem.GroupId = emp.GroupId
	WHERE emp.EmployeeId = @EmployeeId

	SELECT item.JobDescription_GroupItemId AS GroupItemId
		,item.JobDescription_ItemId AS ItemId
		,item.[Name]
		,CASE WHEN setting.EmployeeId IS NOT NULL AND emp.EmployeeId IS NOT NULL THEN 1 ELSE 0 END AS IsSelected
		,ISNULL(setting.DisplayOrder, 999) AS DisplayOrder
	FROM dbo.JobDescription_Item AS item WITH (NOLOCK)
		LEFT JOIN dbo.JobDescription_EmployeeSetting AS setting WITH (NOLOCK) ON item.JobDescription_ItemId = setting.JobDescription_ItemId
		AND setting.EmployeeId = @EmployeeId
		LEFT JOIN dbo.Employee AS emp WITH (NOLOCK) ON setting.EmployeeId = emp.EmployeeId
		AND setting.JobLevelId = emp.JobLevelId
	WHERE EXISTS (
			SELECT 1
			FROM dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK)
				INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gitem.GroupId = emp.GroupId
			WHERE item.JobDescription_GroupItemId = gitem.JobDescription_GroupItemId
				AND emp.EmployeeId = @EmployeeId
		)
	ORDER BY ISNULL(setting.DisplayOrder, 999) ASC

END