
CREATE PROCEDURE [dbo].[EpaEmployeeFlowSetting_Get] @FlowTemplateId INT
	,@TeamIds VARCHAR(MAX) = NULL
	,@IsMonthly BIT = NULL
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
	SET NOCOUNT ON;

	CREATE TABLE #tmpTeam (TeamId INT)

	IF ISNULL(@TeamIds, '') != ''
	BEGIN
		INSERT INTO #tmpTeam (TeamId)
		SELECT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
	END

	SELECT e.EmployeeId
		,e.Code
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
	WHERE e.IsDeleted = 0
		AND e.[Enabled] = 1
		AND (
			ISNULL(@TeamIds, '') = ''
			OR EXISTS (
				SELECT 1
				FROM #tmpTeam AS tmp WITH (NOLOCK)
				WHERE e.GroupId = tmp.TeamId
				)
			)
		AND (
			@IsMonthly IS NULL
			OR e.HasMonthlyEPA = @IsMonthly
			)
	ORDER BY Code ASC

	DROP TABLE #tmpTeam
END