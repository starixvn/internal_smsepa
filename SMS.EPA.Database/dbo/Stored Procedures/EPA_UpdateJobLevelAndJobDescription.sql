
CREATE PROCEDURE [dbo].[EPA_UpdateJobLevelAndJobDescription] @EPAIds VARCHAR(MAX) = NULL
AS
/*
Created: 2021-09-15@Tim.Trinh
Task: update job level and job description
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblEPAId (EPAId INT)

	IF ISNULL(@EPAIds, '') <> ''
	BEGIN
		INSERT INTO #tblEPAId (EPAId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@EPAIds, ',')
	END

	-- job level
	UPDATE epa
	SET epa.JobLevel = jl.[Name]
	FROM dbo.EPA AS epa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblEPAId AS tbl WITH (NOLOCK) ON epa.EPAId = tbl.EPAId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.JobLevel AS jl WITH (NOLOCK) ON emp.JobLevelId = jl.JobLevelId

	-- job description
	DELETE jd
	FROM dbo.EPAJobDescription AS jd WITH (ROWLOCK, UPDLOCK)
	WHERE EXISTS (
			SELECT tbl.EPAId
			FROM #tblEPAId AS tbl WITH (NOLOCK)
			WHERE jd.EPAId = tbl.EPAId
			)

	INSERT INTO dbo.EPAJobDescription (
		EPAId
		,GroupItemId
		,GroupItemName
		,ItemId
		,[Name]
		,DisplayOrder
		)
	SELECT epa.EPAId
		,gitem.JobDescription_GroupItemId AS GroupItemId
		,gitem.[Name] AS GroupItemName
		,item.JobDescription_ItemId AS ItemId
		,item.[Name]
		,setting.DisplayOrder
	FROM dbo.JobDescription_EmployeeSetting AS setting WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON setting.EmployeeId = emp.EmployeeId
		AND setting.JobLevelId = emp.JobLevelId
		INNER JOIN dbo.JobDescription_Item AS item WITH (NOLOCK) ON setting.JobDescription_ItemId = item.JobDescription_ItemId
		INNER JOIN dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK) ON item.JobDescription_GroupItemId = gitem.JobDescription_GroupItemId
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON emp.EmployeeId = epa.EmployeeId
		INNER JOIN #tblEPAId AS tbl WITH (NOLOCK) ON epa.EPAId = tbl.EPAId

	INSERT INTO dbo.EPAJobDescription (
		EPAId
		,GroupItemId
		,GroupItemName
		,ItemId
		,[Name]
		,DisplayOrder
		)
	SELECT epa.EPAId
		,gitem.JobDescription_GroupItemId AS GroupItemId
		,gitem.[Name] AS GroupItemName
		,item.JobDescription_ItemId AS ItemId
		,item.[Name]
		,setting.DisplayOrder
	FROM dbo.JobDescription_GroupSetting AS setting WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON setting.GroupId = emp.GroupId
		AND setting.JobLevelId = emp.JobLevelId
		INNER JOIN dbo.JobDescription_Item AS item WITH (NOLOCK) ON setting.JobDescription_ItemId = item.JobDescription_ItemId
		INNER JOIN dbo.JobDescription_GroupItem AS gitem WITH (NOLOCK) ON item.JobDescription_GroupItemId = gitem.JobDescription_GroupItemId
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON emp.EmployeeId = epa.EmployeeId
		INNER JOIN #tblEPAId AS tbl WITH (NOLOCK) ON epa.EPAId = tbl.EPAId
	WHERE NOT EXISTS (
			SELECT esetting.EmployeeId
			FROM dbo.JobDescription_EmployeeSetting AS esetting WITH (NOLOCK)
			WHERE esetting.EmployeeId = emp.EmployeeId
			)

	DROP TABLE #tblEPAId
END