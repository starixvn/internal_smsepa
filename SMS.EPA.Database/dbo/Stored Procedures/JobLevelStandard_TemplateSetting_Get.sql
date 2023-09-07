
CREATE PROCEDURE [dbo].[JobLevelStandard_TemplateSetting_Get] @ObjectIds NVARCHAR(MAX) = NULL
	,@ObjectTypeId TINYINT = NULL
	,@ShowAll BIT = 0
AS
/*
Created: 2023-05-23@Frankie.Nguyen
Task: Get Template Setting JLS.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SectionTypeId INT = 3
		,@DepartmentTypeId INT = 2
		,@UnitTypeId INT = 1

	CREATE TABLE #tmpObjectIds (ObjectId INT PRIMARY KEY)

	IF ISNULL(@ObjectIds, '') != ''
	BEGIN
		INSERT INTO #tmpObjectIds (ObjectId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@ObjectIds, ',')
		WHERE ISNUMERIC(val) = 1
	END

	SELECT @ObjectTypeId AS ObjectTypeId
		,section.SectionId AS ObjectId
		,section.[Name] AS SectionName
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,unit.UnitId
		,unit.[Name] AS UnitName
	INTO #tmpTemplateSetting
	FROM dbo.[Group] AS gr WITH (NOLOCK)
		INNER JOIN dbo.Section AS section WITH (NOLOCK) ON gr.DepartmentId = section.SectionId
		AND section.IsActive = 1
		LEFT JOIN dbo.Department_New AS dept WITH (NOLOCK) ON section.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
		LEFT JOIN dbo.Unit AS unit WITH (NOLOCK) ON dept.UnitId = unit.UnitId
		AND unit.IsActive = 1
	WHERE gr.IsDeleted = 0

	SELECT DISTINCT tmp.ObjectId
		,tmp.ObjectTypeId
		,tmp.UnitName
		,tmp.DepartmentName
		,tmp.SectionName
		,t.[Name] AS Template
	FROM #tmpTemplateSetting AS tmp WITH (NOLOCK)
		LEFT JOIN dbo.JobLevelStandard_TemplateSetting AS ts WITH (NOLOCK) ON tmp.ObjectTypeId = ts.ObjectTypeId
		AND ts.ObjectId = tmp.ObjectId
		LEFT JOIN dbo.JobLevelStandard_Template AS t WITH (NOLOCK) ON t.TemplateId = ts.TemplateId
	WHERE (
			@ShowAll = 1
			OR (
				@ShowAll = 0
				AND t.TemplateId IS NOT NULL
				)
			)
		AND (
			ISNULL(@ObjectIds, '') = ''
			OR EXISTS (
				SELECT 1
				FROM #tmpObjectIds AS tmpobj WITH (NOLOCK)
				WHERE tmp.ObjectId = tmpobj.ObjectId
				)
			)

	DROP TABLE #tmpObjectIds

	DROP TABLE #tmpTemplateSetting
END