
CREATE PROCEDURE [dbo].[EpaAuto_Probation_AddEmployeeToPeriod] @PeriodId INT
	,@EmployeeIds VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 2015-09-01@Josh.Phan
Task: Add employee to probation period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TargetTypeId INT
		,@StartDate DATETIME
		,@Employees IntId
		,@ExistedEmployeeIds IntId
		,@NoneSettingEmployeeIds NVARCHAR(MAX)
		,@NoneSettingEmployees NVARCHAR(MAX)

	SET @TargetTypeId = 1 -- Probation	
	SET @StartDate = GETDATE()

	INSERT INTO @Employees (Id)
	SELECT DISTINCT val
	FROM fnc_strSplit(@EmployeeIds, ',')

	CREATE TABLE #DeptFlowAndTargetTemplate (
		DepartmentId INT
		,EmployeeId INT
		,FlowTemplateId INT
		,TargetTemplateId INT
		)

	INSERT INTO #DeptFlowAndTargetTemplate
	EXEC GetFlowAndTargetTemplateSetting @TargetTypeId

	-- find employee that already existed
	INSERT INTO @ExistedEmployeeIds
	EXEC dbo.EpaPeriod_AddEmployee_CheckEmployeeExisted @PeriodId
		,@Employees

	DELETE empId
	FROM @Employees AS empId
		INNER JOIN @ExistedEmployeeIds AS existed ON empId.Id = existed.Id

	-- find employee don't have setting Flow Template / Target Template
	SELECT @NoneSettingEmployeeIds = COALESCE(@NoneSettingEmployeeIds + ',', '') + CAST(empS.Id AS NVARCHAR)
	FROM @Employees AS empS
	WHERE NOT EXISTS (
			SELECT 1
			FROM (
				SELECT DISTINCT empFS.EmployeeId
				FROM dbo.SettingEmployeeFlow AS empFS WITH (NOLOCK)
					INNER JOIN dbo.FlowTemplate AS fT WITH (NOLOCK) ON empFS.FlowTemplateId = fT.FlowTemplateId
				WHERE @TargetTypeId = fT.TargetTypeId
				
				UNION
				SELECT DISTINCT emp.EmployeeId
				FROM dbo.SettingDepartmentFlow AS deptFlow WITH (NOLOCK)
					INNER JOIN dbo.[Group] AS gr ON deptFlow.DepartmentId = gr.GroupId
					AND deptFlow.IsGroup = 1
					INNER JOIN dbo.Employee AS emp ON emp.GroupId = gr.GroupId
					INNER JOIN dbo.FlowTemplate AS fT WITH (NOLOCK) ON deptFlow.FlowTemplateId = fT.FlowTemplateId
				WHERE @TargetTypeId = fT.TargetTypeId
				
				UNION
				SELECT DISTINCT emp.EmployeeId
				FROM #DeptFlowAndTargetTemplate AS emp
				WHERE emp.FlowTemplateId IS NOT NULL
				) AS empF
			WHERE empF.EmployeeId = empS.Id
			)
		OR NOT EXISTS (
			SELECT 1
			FROM (
				SELECT DISTINCT tarTemp.EmployeeId
				FROM dbo.TargetTemplateEmployee AS tarTemp WITH (NOLOCK)
					INNER JOIN dbo.TargetTemplate AS tarT WITH (NOLOCK) ON tarTemp.TargetTemplateId = tarT.TargetTemplateId
				WHERE @TargetTypeId = tarT.TargetTypeId
				
				UNION
				SELECT DISTINCT emp.EmployeeId
				FROM dbo.TargetTemplateDepartment AS deptTar WITH (NOLOCK)
					INNER JOIN dbo.[Group] AS gr ON deptTar.DepartmentId = gr.GroupId
					AND deptTar.IsGroup = 1
					INNER JOIN dbo.Employee AS emp ON emp.GroupId = gr.GroupId
					INNER JOIN dbo.TargetTemplate AS tarT WITH (NOLOCK) ON deptTar.TargetTemplateId = tarT.TargetTemplateId
				WHERE @TargetTypeId = tarT.TargetTypeId
				
				UNION
				SELECT DISTINCT emp.EmployeeId
				FROM #DeptFlowAndTargetTemplate AS emp
				WHERE emp.TargetTemplateId IS NOT NULL
				) AS empTS
			WHERE empTS.EmployeeId = empS.Id
			)

	IF ISNULL(@NoneSettingEmployeeIds, '') <> ''
	BEGIN
		DELETE
		FROM @Employees
		WHERE Id IN (
				SELECT DISTINCT val
				FROM fnc_strSplit(@NoneSettingEmployeeIds, ',')
				)
	END

	-- insert employee flow setting
	INSERT INTO dbo.PeriodFlowSetting (
		PeriodId
		,EmployeeId
		,ProcessId
		,EvaluatorId
		,[Priority]
		)
	SELECT @PeriodId
		,empFS.EmployeeId
		,empFS.ProcessId
		,CASE 
			WHEN empFS.ProcessId = 2 THEN empFS.EmployeeId
			ELSE empFS.EvaluatorId
			END
		,empFS.[Priority]
	FROM dbo.SettingEmployeeFlow AS empFS WITH (NOLOCK)
		INNER JOIN @Employees AS empt ON empFS.EmployeeId = empt.Id
		LEFT JOIN dbo.Employee AS emp WITH (NOLOCK) ON empFS.EvaluatorId = emp.EmployeeId
		INNER JOIN dbo.FlowTemplate AS flowT WITH (NOLOCK) ON empFS.FlowTemplateId = flowT.FlowTemplateId
		AND flowT.TargetTypeId = @TargetTypeId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodFlowSetting AS pFS WITH (NOLOCK)
			WHERE pFS.EmployeeId = empFS.EmployeeId
				AND pFS.PeriodId = @PeriodId
			)

	INSERT INTO dbo.PeriodFlowSetting (
		PeriodId
		,EmployeeId
		,ProcessId
		,EvaluatorId
		,[Priority]
		)
	SELECT @PeriodId
		,emp.EmployeeId
		,deptFS.ProcessId
		,CASE 
			WHEN deptFS.ProcessId = 2 THEN emp.EmployeeId
			ELSE deptFS.EvaluatorId
			END
		,deptFS.[Priority]
	FROM dbo.SettingDepartmentFlow AS deptFS WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr ON deptFS.DepartmentId = gr.GroupId
		AND deptFS.IsGroup = 1
		INNER JOIN dbo.Employee AS emp ON emp.GroupId = gr.GroupId
		INNER JOIN @Employees AS empt ON emp.EmployeeId = empt.Id
		LEFT JOIN dbo.Employee AS eva ON deptFS.EvaluatorId = eva.EmployeeId
		INNER JOIN dbo.FlowTemplate AS flowT WITH (NOLOCK) ON deptFS.FlowTemplateId = flowT.FlowTemplateId
		AND flowT.TargetTypeId = @TargetTypeId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodFlowSetting AS pFS WITH (NOLOCK)
			WHERE pFS.EmployeeId = emp.EmployeeId
				AND pFS.PeriodId = @PeriodId
			)

	INSERT INTO dbo.PeriodFlowSetting (
		PeriodId
		,EmployeeId
		,ProcessId
		,EvaluatorId
		,[Priority]
		)
	SELECT DISTINCT @PeriodId
		,emp.EmployeeId
		,deptFS.ProcessId
		,CASE 
			WHEN deptFS.ProcessId = 2 THEN emp.EmployeeId
			ELSE deptFS.EvaluatorId
			END
		,deptFS.[Priority]
	FROM dbo.SettingDepartmentFlow AS deptFS WITH (NOLOCK)
		INNER JOIN #DeptFlowAndTargetTemplate AS deptFS1 ON deptFS.DepartmentId = deptFS1.DepartmentId
		AND deptFS.FlowTemplateId = deptFS1.FlowTemplateId
		AND deptFS.IsGroup = 0
		INNER JOIN dbo.Employee AS emp ON deptFS1.EmployeeId = emp.EmployeeId
		INNER JOIN @Employees AS empt ON emp.EmployeeId = empt.Id
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodFlowSetting AS pFS WITH (NOLOCK)
			WHERE pFS.EmployeeId = emp.EmployeeId
				AND pFS.PeriodId = @PeriodId
			)

	-- insert employee target setting
	INSERT INTO dbo.PeriodTargetSetting (
		PeriodId
		,EmployeeId
		,TargetTemplateId
		,ModifiedBy
		,ModifiedDate
		)
	SELECT DISTINCT @PeriodId
		,empTS.EmployeeId
		,empTS.TargetTemplateId
		,NULL
		,GETDATE()
	FROM dbo.TargetTemplateEmployee AS empTS WITH (NOLOCK)
		INNER JOIN @Employees AS emp ON empTS.EmployeeId = emp.Id
		INNER JOIN dbo.TargetTemplate AS tarT WITH (NOLOCK) ON empTS.TargetTemplateId = tarT.TargetTemplateId
		AND tarT.TargetTypeId = @TargetTypeId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodTargetSetting AS pTS WITH (NOLOCK)
			WHERE empTS.EmployeeId = pTS.EmployeeId
				AND pTS.PeriodId = @PeriodId
			)

	INSERT INTO dbo.PeriodTargetSetting (
		PeriodId
		,EmployeeId
		,TargetTemplateId
		,ModifiedBy
		,ModifiedDate
		)
	SELECT DISTINCT @PeriodId
		,emp.EmployeeId
		,deptTS.TargetTemplateId
		,NULL
		,GETDATE()
	FROM dbo.TargetTemplateDepartment AS deptTS WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr ON deptTS.DepartmentId = gr.GroupId
		AND deptTS.IsGroup = 1
		INNER JOIN dbo.Employee AS emp ON emp.GroupId = gr.GroupId
		INNER JOIN @Employees AS empt ON emp.EmployeeId = empt.Id
		INNER JOIN dbo.TargetTemplate AS tarT WITH (NOLOCK) ON deptTS.TargetTemplateId = tarT.TargetTemplateId
		AND tarT.TargetTypeId = @TargetTypeId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodTargetSetting AS pTS WITH (NOLOCK)
			WHERE emp.EmployeeId = pTS.EmployeeId
				AND pTS.PeriodId = @PeriodId
			)

	INSERT INTO dbo.PeriodTargetSetting (
		PeriodId
		,EmployeeId
		,TargetTemplateId
		,ModifiedBy
		,ModifiedDate
		)
	SELECT DISTINCT @PeriodId
		,emp.EmployeeId
		,deptTS.TargetTemplateId
		,NULL
		,GETDATE()
	FROM dbo.TargetTemplateDepartment AS deptTS WITH (NOLOCK)
		INNER JOIN #DeptFlowAndTargetTemplate AS deptTS1 ON deptTS.DepartmentId = deptTS1.DepartmentId
		AND deptTS.TargetTemplateId = deptTS1.TargetTemplateId
		AND deptTS.IsGroup = 0
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON deptTS1.EmployeeId = emp.EmployeeId
		INNER JOIN @Employees AS empt ON emp.EmployeeId = empt.Id
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodTargetSetting AS pTS WITH (NOLOCK)
			WHERE pTS.EmployeeId = emp.EmployeeId
				AND pTS.PeriodId = @PeriodId
			)

	UPDATE pe
	SET pe.StartDate = DATEADD(DAY, ISNULL(aa.Duration - peP.Duration, 0), @StartDate)
		,pe.EndDate = DATEADD(DAY, (CASE 
				WHEN ISNULL(aa.Duration, 0) - 1 < 0 THEN 0
				ELSE ISNULL(aa.Duration, 0) - 1 END
				), @StartDate)
	FROM dbo.PeriodFlowSetting AS pe
		INNER JOIN dbo.PeriodProcessSetting AS peP ON pe.PeriodId = peP.PeriodId
		AND pe.ProcessId = peP.ProcessId
		INNER JOIN (
		SELECT set1.PeriodId
			,set1.ProcessId
			,SUM(CASE 
					WHEN set2.ProcessId <= set1.ProcessId THEN set2.Duration
					ELSE 0
					END) AS Duration
		FROM dbo.PeriodProcessSetting AS set1 WITH (NOLOCK)
			INNER JOIN dbo.PeriodProcessSetting AS set2 WITH (NOLOCK) ON set1.PeriodId = set2.PeriodId
		WHERE set1.PeriodId = @PeriodId
		GROUP BY set1.PeriodId
			,set1.ProcessId
		) AS aa ON pe.PeriodId = aa.PeriodId
		AND pe.ProcessId = aa.ProcessId
		INNER JOIN @Employees AS empId ON pe.EmployeeId = empId.Id
	WHERE pe.PeriodId = @PeriodId

	IF ISNULL(@NoneSettingEmployeeIds, '') <> ''
	BEGIN
		SELECT @NoneSettingEmployees = COALESCE(@NoneSettingEmployees + ', ', '') + NickName
		FROM dbo.Employee WITH (NOLOCK)
		WHERE EmployeeId IN (
				SELECT val
				FROM fnc_strSplit(@NoneSettingEmployeeIds, ',')
				)

		SELECT @ErrCode = 1
			,@ErrMsg = @NoneSettingEmployees + ' do not have target setting / flow setting.'

		RETURN
	END

	DROP TABLE #DeptFlowAndTargetTemplate
END