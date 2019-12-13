
CREATE PROCEDURE [dbo].[EpaAuto_Monthly_AddEmployeeToPeriod] @PeriodId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20160223@Tim.Trinh
Task: Auto add employee to period that has start date is today.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

Edited: 2017-04-14: Hunter.Tran
Do not add Employees that have [Enable] = 0 or ResignDate not null
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TargetTypeId INT
		,@StartDate DATETIME
		,@EmployeeIds IntId
		,@ExistedEmployeeIds IntId
		,@NoneSettingEmployeeIds NVARCHAR(MAX)
		,@OverlapEmployeeIds IntId
		,@OverlapEmployees NVARCHAR(MAX)
		,@NoneSettingEmployees NVARCHAR(MAX)

	SELECT @TargetTypeId = TargetTypeId
		,@StartDate = StartDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	INSERT INTO @EmployeeIds (Id)
	SELECT DISTINCT emp.EmployeeId
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND emp.HasMonthlyEPA = 1
		AND emp.ResignedDate is null

	CREATE TABLE #DeptFlowAndTargetTemplate (
		DepartmentId INT
		,EmployeeId INT
		,FlowTemplateId INT
		,TargetTemplateId INT
		)

	INSERT INTO #DeptFlowAndTargetTemplate (
		DepartmentId
		,EmployeeId
		,FlowTemplateId
		,TargetTemplateId
		)
	EXEC dbo.GetFlowAndTargetTemplateSetting @TargetTypeId

	-- find employee that already existed
	INSERT INTO @ExistedEmployeeIds (Id)
	EXEC dbo.EpaPeriod_AddEmployee_CheckEmployeeExisted @PeriodId
		,@EmployeeIds

	DELETE empId
	FROM @EmployeeIds AS empId
		INNER JOIN @ExistedEmployeeIds AS existed ON empId.Id = existed.Id

	-- find employee don't have setting Flow Template / Target Template
	SELECT @NoneSettingEmployeeIds = COALESCE(@NoneSettingEmployeeIds + ',', '') + CAST(emp.EmployeeId AS NVARCHAR(20))
	FROM @EmployeeIds AS empS
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON empS.Id = emp.EmployeeId
	WHERE NOT EXISTS (
			SELECT 1
			FROM (
				SELECT DISTINCT empFS.EmployeeId
				FROM dbo.SettingEmployeeFlow AS empFS WITH (NOLOCK)
					INNER JOIN dbo.FlowTemplate AS fT WITH (NOLOCK) ON empFS.FlowTemplateId = fT.FlowTemplateId
				WHERE fT.TargetTypeId = @TargetTypeId
				
				UNION
				SELECT DISTINCT emp1.EmployeeId
				FROM dbo.SettingDepartmentFlow AS deptFlow WITH (NOLOCK)
					INNER JOIN dbo.[Group] AS gr ON deptFlow.DepartmentId = gr.GroupId
					AND deptFlow.IsGroup = 1
					INNER JOIN dbo.Employee AS emp1 ON emp1.GroupId = gr.GroupId
					INNER JOIN dbo.FlowTemplate AS fT WITH (NOLOCK) ON deptFlow.FlowTemplateId = fT.FlowTemplateId
				WHERE fT.TargetTypeId = @TargetTypeId
				
				UNION
				SELECT DISTINCT emp1.EmployeeId
				FROM #DeptFlowAndTargetTemplate AS emp1
				WHERE emp1.FlowTemplateId IS NOT NULL
				) AS empF
			WHERE empF.EmployeeId = empS.Id
			)
		OR NOT EXISTS (
			SELECT 1
			FROM (
				SELECT DISTINCT tarTemp.EmployeeId
				FROM dbo.TargetTemplateEmployee AS tarTemp WITH (NOLOCK)
					INNER JOIN dbo.TargetTemplate AS tarT WITH (NOLOCK) ON tarTemp.TargetTemplateId = tarT.TargetTemplateId
				WHERE tarT.TargetTypeId = @TargetTypeId
				
				UNION
				SELECT DISTINCT emp1.EmployeeId
				FROM dbo.TargetTemplateDepartment AS deptTar WITH (NOLOCK)
					INNER JOIN dbo.[Group] AS gr ON deptTar.DepartmentId = gr.GroupId
					AND deptTar.IsGroup = 1
					INNER JOIN dbo.Employee AS emp1 ON emp1.GroupId = gr.GroupId
					INNER JOIN dbo.TargetTemplate AS tarT WITH (NOLOCK) ON deptTar.TargetTemplateId = tarT.TargetTemplateId
				WHERE tarT.TargetTypeId = @TargetTypeId
				
				UNION
				SELECT DISTINCT emp1.EmployeeId
				FROM #DeptFlowAndTargetTemplate AS emp1
				WHERE emp1.TargetTemplateId IS NOT NULL
				) AS empTS
			WHERE empTS.EmployeeId = empS.Id
			)

	IF ISNULL(@NoneSettingEmployeeIds, '') <> ''
	BEGIN
		DELETE
		FROM @EmployeeIds
		WHERE Id IN (
				SELECT DISTINCT val
				FROM fnc_strSplit(@NoneSettingEmployeeIds, ',')
				)
	END

	-- check overlap date between monthly and semi
	INSERT INTO @OverlapEmployeeIds (Id)
	EXEC dbo.EpaPeriod_AddEmployee_CheckEmployeeOverlap @PeriodId
		,@TargetTypeId
		,@EmployeeIds

	DELETE empId
	FROM @EmployeeIds AS empId
		INNER JOIN @OverlapEmployeeIds AS overlap ON empId.Id = overlap.Id

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
		INNER JOIN @EmployeeIds AS empt ON empFS.EmployeeId = empt.Id
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
		INNER JOIN @EmployeeIds AS empt ON emp.EmployeeId = empt.Id
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
		INNER JOIN @EmployeeIds AS empt ON emp.EmployeeId = empt.Id
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
		INNER JOIN @EmployeeIds AS emp ON empTS.EmployeeId = emp.Id
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
		INNER JOIN @EmployeeIds AS empt ON emp.EmployeeId = empt.Id
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
		INNER JOIN @EmployeeIds AS empt ON emp.EmployeeId = empt.Id
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodTargetSetting AS pTS WITH (NOLOCK)
			WHERE pTS.EmployeeId = emp.EmployeeId
				AND pTS.PeriodId = @PeriodId
			)

	IF ISNULL(@StartDate, '') <> ''
	BEGIN
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
			SELECT DISTINCT set1.PeriodId
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
			INNER JOIN @EmployeeIds AS empId ON pe.EmployeeId = empId.Id
		WHERE pe.PeriodId = @PeriodId
	END

	SELECT @OverlapEmployees = COALESCE(@OverlapEmployees + ', ', '') + emp.NickName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN @OverlapEmployeeIds AS overlap ON emp.EmployeeId = overlap.Id

	IF ISNULL(@NoneSettingEmployeeIds, '') <> ''
	BEGIN
		SELECT @NoneSettingEmployees = COALESCE(@NoneSettingEmployees + ', ', '') + NickName
		FROM dbo.Employee WITH (NOLOCK)
		WHERE EmployeeId IN (
				SELECT DISTINCT val
				FROM fnc_strSplit(@NoneSettingEmployeeIds, ',')
				)

		SELECT @ErrCode = 1
			,@ErrMsg = '<b>' + @NoneSettingEmployees + '</b> do not have target setting / flow setting.'

		IF EXISTS (
				SELECT 1
				FROM @OverlapEmployeeIds
				)
		BEGIN
			SET @ErrMsg = @ErrMsg + '<br/>Start Date / End Date of <b>' + @OverlapEmployees + '</b> overlap on semi.'
		END

		RETURN
	END
	ELSE IF EXISTS (
			SELECT 1
			FROM @OverlapEmployeeIds
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Start Date / End Date of <b>' + @OverlapEmployees + '</b> overlap on semi.'

		RETURN
	END

	DROP TABLE #DeptFlowAndTargetTemplate
END