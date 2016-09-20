
CREATE PROCEDURE [dbo].[EpaPeriod_AddEmployee] @PeriodId INT
	,@Departments VARCHAR(MAX) = NULL
	,@EmployeeId INT = NULL
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Add employee to evaluation period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
	@Departments = <ROOT><Department DepartmentId='1' IsGroup='1'></Department</ROOT>
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.Period WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The Period does not exists.'

		RETURN
	END

	DECLARE @DocHandler INT
		,@TargetTypeId INT
		,@StartDate DATETIME
		,@EmployeeIds IntId
		,@ExistedEmployeeIds IntId
		,@NoneSettingEmployeeIds NVARCHAR(MAX)
		,@OverlapEmployeeIds IntId
		,@NumberEmployeeSuccessfully INT
		,@NoneSettingEmployees NVARCHAR(MAX)
		,@OverlapEmployees NVARCHAR(MAX)

	SELECT @TargetTypeId = TargetTypeId
		,@StartDate = StartDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	CREATE TABLE #Department (
		DepartmentId INT
		,IsGroup BIT
		)

	CREATE TABLE #DeptFlowAndTargetTemplate (
		DepartmentId INT
		,EmployeeId INT
		,FlowTemplateId INT
		,TargetTemplateId INT
		)

	INSERT INTO #DeptFlowAndTargetTemplate
	EXEC GetFlowAndTargetTemplateSetting @TargetTypeId

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@Departments

	IF ISNULL(@Departments, '') <> ''
	BEGIN
		INSERT INTO #Department (
			DepartmentId
			,IsGroup
			)
		SELECT DepartmentId
			,IsGroup
		FROM OPENXML(@DocHandler, '/ROOT/Department', 1) WITH (DepartmentId INT, IsGroup BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler

		INSERT INTO @EmployeeIds (Id)
		SELECT DISTINCT tblEmp.EmployeeId
		FROM (
			SELECT DISTINCT emp.EmployeeId
			FROM #Department AS deptT WITH (NOLOCK)
				INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON deptT.DepartmentId = dept.DepartmentId
				AND deptT.IsGroup = 0
				INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gr.lft >= dept.lft
				AND gr.rgt <= dept.rgt
				AND gr.IsDeleted = 0
				INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gr.GroupId = emp.GroupId
			WHERE emp.IsDeleted = 0
				AND emp.[Enabled] = 1
			
			UNION
			SELECT DISTINCT emp.EmployeeId
			FROM #Department AS deptT WITH (NOLOCK)
				INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON deptT.DepartmentId = gr.GroupId
				AND deptT.IsGroup = 1
				AND gr.IsDeleted = 0
				INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gr.GroupId = emp.GroupId
			WHERE emp.IsDeleted = 0
				AND emp.[Enabled] = 1
			) AS tblEmp
		WHERE ISNULL(@EmployeeId, '') = ''
			OR tblEmp.EmployeeId <> @EmployeeId
	END

	IF ISNULL(@EmployeeId, '') <> ''
	BEGIN
		INSERT INTO @EmployeeIds (Id)
		VALUES (@EmployeeId)
	END

	-- check employees has set monthly EPA
	IF @TargetTypeId = 2
	BEGIN
		DELETE emp
		FROM @EmployeeIds AS emp
			INNER JOIN dbo.Employee AS emp1 WITH (NOLOCK) ON emp.Id = emp1.EmployeeId
		WHERE emp1.HasMonthlyEPA = 0
			OR emp1.HasMonthlyEPA IS NULL
	END

	-- find employee that already existed
	INSERT INTO @ExistedEmployeeIds
	EXEC dbo.EpaPeriod_AddEmployee_CheckEmployeeExisted @PeriodId
		,@EmployeeIds

	DELETE empId
	FROM @EmployeeIds AS empId
		INNER JOIN @ExistedEmployeeIds AS existed ON empId.Id = existed.Id

	-- find employee don't have setting Flow Template / Target Template
	SELECT @NoneSettingEmployeeIds = COALESCE(@NoneSettingEmployeeIds + ',', '') + CAST(empS.Id AS NVARCHAR)
	FROM @EmployeeIds AS empS
	WHERE (
			NOT EXISTS (
				SELECT 1
				FROM (
					SELECT DISTINCT empFS.EmployeeId
					FROM dbo.SettingEmployeeFlow AS empFS WITH (NOLOCK)
						INNER JOIN dbo.FlowTemplate AS fT WITH (NOLOCK) ON empFS.FlowTemplateId = fT.FlowTemplateId
					WHERE fT.TargetTypeId = @TargetTypeId
					
					UNION
					SELECT DISTINCT emp.EmployeeId
					FROM dbo.SettingDepartmentFlow AS deptFS WITH (NOLOCK)
						INNER JOIN dbo.[Group] AS gr ON deptFS.DepartmentId = gr.GroupId
						AND deptFS.IsGroup = 1
						INNER JOIN dbo.Employee AS emp ON emp.GroupId = gr.GroupId
						INNER JOIN dbo.FlowTemplate AS fT WITH (NOLOCK) ON deptFS.FlowTemplateId = fT.FlowTemplateId
					WHERE fT.TargetTypeId = @TargetTypeId
					
					UNION
					SELECT DISTINCT emp.EmployeeId
					FROM #DeptFlowAndTargetTemplate AS emp
					WHERE emp.FlowTemplateId IS NOT NULL
					) AS empF
				WHERE empF.EmployeeId = empS.Id
				)
			)
		OR (
			@TargetTypeId <> 5 -- Not Promotion
			AND (
				NOT EXISTS (
					SELECT 1
					FROM (
						SELECT DISTINCT tarTemp.EmployeeId
						FROM dbo.TargetTemplateEmployee AS tarTemp WITH (NOLOCK)
							INNER JOIN dbo.TargetTemplate AS tarT WITH (NOLOCK) ON tarTemp.TargetTemplateId = tarT.TargetTemplateId
						WHERE tarT.TargetTypeId = @TargetTypeId
						
						UNION
						SELECT DISTINCT emp.EmployeeId
						FROM dbo.TargetTemplateDepartment AS deptTar WITH (NOLOCK)
							INNER JOIN dbo.[Group] AS gr ON deptTar.DepartmentId = gr.GroupId
							AND deptTar.IsGroup = 1
							INNER JOIN dbo.Employee AS emp ON emp.GroupId = gr.GroupId
							INNER JOIN dbo.TargetTemplate AS tarT WITH (NOLOCK) ON deptTar.TargetTemplateId = tarT.TargetTemplateId
						WHERE tarT.TargetTypeId = @TargetTypeId
						
						UNION
						SELECT DISTINCT emp.EmployeeId
						FROM #DeptFlowAndTargetTemplate AS emp
						WHERE emp.TargetTemplateId IS NOT NULL
						) AS empTS
					WHERE empTS.EmployeeId = empS.Id
					)
				)
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

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = dbo.fncGetDataForWriteLog_Period_AddEmployee @PeriodId = @PeriodId

	BEGIN TRY
		BEGIN TRANSACTION

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
			LEFT JOIN dbo.Employee AS eva WITH (NOLOCK) ON empFS.EvaluatorId = eva.EmployeeId
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
		IF @TargetTypeId <> 5 -- Not Promotion
		BEGIN
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
				,@ModifiedBy
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
				,@ModifiedBy
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
				,@ModifiedBy
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
		END

		-- Only update start date and end date for employee of period that has target type is monthly, semi annual
		IF @TargetTypeId IN (2, 3, 5) -- Monthly, Semi-Annual and Promotion
		BEGIN
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
					INNER JOIN @EmployeeIds AS emp ON pe.EmployeeId = emp.Id
				WHERE pe.PeriodId = @PeriodId
			END
		END

		EXEC @Data = dbo.fncGetDataForWriteLog_Period_AddEmployee @PeriodId = @PeriodId

		EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
			,@SpName = 'EpaPeriod_AddEmployee'
			,@ObjectId = @PeriodId
			,@ObjectTypeId = 8
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		SELECT @NumberEmployeeSuccessfully = COUNT(Id)
		FROM @EmployeeIds

		IF ISNULL(@NoneSettingEmployeeIds, '') <> ''
		BEGIN
			SELECT @NoneSettingEmployees = COALESCE(@NoneSettingEmployees + ', ', '') + NickName
			FROM dbo.Employee WITH (NOLOCK)
			WHERE EmployeeId IN (
					SELECT DISTINCT val
					FROM dbo.fnc_strSplit(@NoneSettingEmployeeIds, ',')
					)
		END

		SELECT @OverlapEmployees = COALESCE(@OverlapEmployees + ', ', '') + emp.NickName
		FROM dbo.Employee AS emp WITH (NOLOCK)
			INNER JOIN @OverlapEmployeeIds AS overlap ON emp.EmployeeId = overlap.Id

		IF EXISTS (
				SELECT 1
				FROM @ExistedEmployeeIds
				)
			AND @NumberEmployeeSuccessfully > 0
		BEGIN
			SET @ErrCode = 1
			SET @ErrMsg = N'Only insert ' + CAST(@NumberEmployeeSuccessfully AS NVARCHAR) + ' employees successfully.'

			IF ISNULL(@NoneSettingEmployeeIds, '') <> ''
			BEGIN
				SET @ErrMsg = @ErrMsg + '<br/><b>' + @NoneSettingEmployees + '</b> do not have target setting / flow setting.'
			END

			IF EXISTS (
					SELECT 1
					FROM @OverlapEmployeeIds
					)
			BEGIN
				IF @TargetTypeId = 2 -- monthly
				BEGIN
					SET @ErrMsg = @ErrMsg + '<br/>Start Date / End Date of <b>' + @OverlapEmployees + '</b> overlap on semi.'
				END
				ELSE IF @TargetTypeId = 3 -- monthly
				BEGIN
					SET @ErrMsg = @ErrMsg + '<br/>Start Date / End Date of <b>' + @OverlapEmployees + '</b> overlap on monthly.'
				END
			END
		END
		ELSE IF ISNULL(@NoneSettingEmployeeIds, '') <> ''
		BEGIN
			SET @ErrCode = 1
			SET @ErrMsg = '<b>' + @NoneSettingEmployees + '</b> do not have target setting / flow setting.'

			IF EXISTS (
					SELECT 1
					FROM @OverlapEmployeeIds
					)
			BEGIN
				IF @TargetTypeId = 2 -- monthly
				BEGIN
					SET @ErrMsg = @ErrMsg + '<br/>Start Date / End Date of <b>' + @OverlapEmployees + '</b> overlap on semi.'
				END
				ELSE IF @TargetTypeId = 3 -- monthly
				BEGIN
					SET @ErrMsg = @ErrMsg + '<br/>Start Date / End Date of <b>' + @OverlapEmployees + '</b> overlap on monthly.'
				END
			END
		END
		ELSE IF EXISTS (
				SELECT 1
				FROM @OverlapEmployeeIds
				)
		BEGIN
			SET @ErrCode = 1

			IF @TargetTypeId = 2 -- monthly
			BEGIN
				SET @ErrMsg = 'Start Date / End Date of <b>' + @OverlapEmployees + '</b> overlap on semi.'
			END
			ELSE IF @TargetTypeId = 3 -- monthly
			BEGIN
				SET @ErrMsg = 'Start Date / End Date of <b>' + @OverlapEmployees + '</b> overlap on monthly.'
			END
		END

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'All employee into period unsuccessfully.'
	END CATCH

	DROP TABLE #Department

	DROP TABLE #DeptFlowAndTargetTemplate
END