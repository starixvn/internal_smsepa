
CREATE PROCEDURE [dbo].[EPA_GetJobDescriptiontionById] @EPAId INT
AS
/*
Created: 20210428@Tim.Trinh
Task: Get job description by EPAId.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmployeeId INT

	SELECT @EmployeeId = EmployeeId
	FROM dbo.EPA WITH (NOLOCK)
	WHERE EPAId = @EPAId

	IF EXISTS (
			SELECT 1
			FROM dbo.JobDescription_EmployeeSetting AS setting WITH (NOLOCK)
				INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON setting.EmployeeId = emp.EmployeeId AND setting.JobLevelId = emp.JobLevelId
			WHERE emp.EmployeeId = @EmployeeId
			)
	BEGIN
		SELECT DISTINCT gitem.JobDescription_GroupItemId AS GroupItemId
			,gitem.[Name]
		FROM dbo.JobDescription_EmployeeSetting AS setting WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON setting.EmployeeId = emp.EmployeeId AND setting.JobLevelId = emp.JobLevelId
			INNER JOIN dbo.JobDescription_Item AS item WITH (NOLOCK) ON setting.JobDescription_ItemId = item.JobDescription_ItemId
			INNER JOIN dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK) ON item.JobDescription_GroupItemId = gitem.JobDescription_GroupItemId
		WHERE emp.EmployeeId = @EmployeeId
		
		SELECT item.JobDescription_GroupItemId AS GroupItemId
			,item.JobDescription_ItemId AS ItemId
			,item.[Name]
			,setting.DisplayOrder
		FROM dbo.JobDescription_EmployeeSetting AS setting WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON setting.EmployeeId = emp.EmployeeId AND setting.JobLevelId = emp.JobLevelId
			INNER JOIN dbo.JobDescription_Item AS item WITH (NOLOCK) ON setting.JobDescription_ItemId = item.JobDescription_ItemId
		WHERE emp.EmployeeId = @EmployeeId
		ORDER BY setting.DisplayOrder
	END
	ELSE
	BEGIN
		SELECT DISTINCT gitem.JobDescription_GroupItemId AS GroupItemId
			,gitem.[Name]
		FROM dbo.JobDescription_GroupSetting AS setting WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON setting.GroupId = emp.GroupId AND setting.JobLevelId = emp.JobLevelId
			INNER JOIN dbo.JobDescription_Item AS item WITH (NOLOCK) ON setting.JobDescription_ItemId = item.JobDescription_ItemId
			INNER JOIN dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK) ON item.JobDescription_GroupItemId = gitem.JobDescription_GroupItemId
		WHERE emp.EmployeeId = @EmployeeId
		
		SELECT item.JobDescription_GroupItemId AS GroupItemId
			,item.JobDescription_ItemId AS ItemId
			,item.[Name]
			,setting.DisplayOrder
		FROM dbo.JobDescription_GroupSetting AS setting WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON setting.GroupId = emp.GroupId AND setting.JobLevelId = emp.JobLevelId
			INNER JOIN dbo.JobDescription_Item AS item WITH (NOLOCK) ON setting.JobDescription_ItemId = item.JobDescription_ItemId
		WHERE emp.EmployeeId = @EmployeeId
		ORDER BY setting.DisplayOrder
	END
END