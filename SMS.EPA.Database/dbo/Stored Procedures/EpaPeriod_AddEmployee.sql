

CREATE PROCEDURE [dbo].[EpaPeriod_AddEmployee] @PeriodId INT
	,@TeamIds VARCHAR(MAX) = NULL
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
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TargetTypeId INT
		,@StartDate DATE
		,@EmployeeIds IntId
		,@ExistedEmployeeIds IntId
		,@NoneSettingEmployeeIds NVARCHAR(MAX)
		,@OverlapEmployeeIds IntId
		,@NumberEmployeeSuccessfully INT
		,@NoneSettingEmployees NVARCHAR(MAX)
		,@OverlapEmployees NVARCHAR(MAX)
		,@AttendanceStartDate DATE
		,@ProbationTarget TINYINT = 1
		,@MonthlyTarget TINYINT = 2
		,@SemiTarget TINYINT = 3
		,@PromotionTarget TINYINT = 5
		,@EnableDisciplineRule BIT = 0
		,@CurrentYear INT = YEAR(GETDATE())

	SELECT @EnableDisciplineRule = CAST(Value AS BIT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'EnableDisciplineRuleWhenAddEmployeeToSemi'

	SET @EnableDisciplineRule = ISNULL(@EnableDisciplineRule, 0)

	SELECT @TargetTypeId = TargetTypeId
		,@StartDate = StartDate
		,@AttendanceStartDate = AttendanceStartDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	CREATE TABLE #tmpTeam (TeamId INT)

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

	IF ISNULL(@TeamIds, '') != ''
	BEGIN
		INSERT INTO #tmpTeam (TeamId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
	END

	INSERT INTO @EmployeeIds (Id)
	SELECT emp.EmployeeId
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE emp.IsDeleted = 0
		AND emp.[Enabled] = 1
		AND emp.ResignedDate IS NULL
		AND (
			(
				@EnableDisciplineRule = 1
				AND (
					(
						@TargetTypeId = @SemiTarget
						AND (
							(
								MONTH(@StartDate) <= 6
								AND CAST(emp.JoinedDate AS DATE) <= CAST(CAST(@CurrentYear AS NVARCHAR(5)) + '-02-15' AS DATE)
								)
							OR (
								MONTH(@StartDate) > 6
								AND CAST(emp.JoinedDate AS DATE) <= CAST(CAST(@CurrentYear AS NVARCHAR(5)) + '-08-15' AS DATE)
								)
							)
						)
					OR @TargetTypeId != @SemiTarget
					)
				)
			OR @EnableDisciplineRule = 0
			)
		AND ISNULL(@TeamIds, '') != ''
		AND EXISTS (
			SELECT 1
			FROM #tmpTeam AS tmp WITH (NOLOCK)
			WHERE emp.GroupId = tmp.TeamId
			)
		AND (
			@EmployeeId IS NULL
			OR emp.EmployeeId != @EmployeeId
			)

	IF @EmployeeId IS NOT NULL
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM dbo.Employee WITH (NOLOCK)
				WHERE [Enabled] = 1
					AND IsDeleted = 0
					AND EmployeeId = @EmployeeId
					AND ResignedDate IS NOT NULL
				)
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = N'Employee resigned'

			RETURN
		END

		IF EXISTS (
				SELECT 1
				FROM dbo.Employee WITH (NOLOCK)
				WHERE [Enabled] = 1
					AND IsDeleted = 0
					AND EmployeeId = @EmployeeId
					AND ResignedDate IS NULL
					AND @TargetTypeId = @SemiTarget
					AND (
						(
							MONTH(@StartDate) <= 6
							AND CAST(JoinedDate AS DATE) > CAST(CAST(@CurrentYear AS NVARCHAR(5)) + '-02-15' AS DATE)
							)
						OR (
							MONTH(@StartDate) > 6
							AND CAST(JoinedDate AS DATE) > CAST(CAST(@CurrentYear AS NVARCHAR(5)) + '-08-15' AS DATE)
							)
						)
				)
			AND @EnableDisciplineRule = 1
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = N'Employee Group Date must be <= 15-Feb(H1) or <= 15-Aug(H2)'

			RETURN
		END

		INSERT INTO @EmployeeIds (Id)
		VALUES (@EmployeeId)
	END

	IF @TargetTypeId = @MonthlyTarget
	BEGIN
		DELETE emp
		FROM @EmployeeIds AS emp
			INNER JOIN dbo.Employee AS emp1 WITH (NOLOCK) ON emp.Id = emp1.EmployeeId
		WHERE emp1.HasMonthlyEPA = 0
			OR emp1.HasMonthlyEPA IS NULL
	END

	-- find employee that already existed
	INSERT INTO @ExistedEmployeeIds (Id)
	EXEC dbo.EpaPeriod_AddEmployee_CheckEmployeeExisted @PeriodId
		,@EmployeeIds

	DELETE empId
	FROM @EmployeeIds AS empId
		INNER JOIN @ExistedEmployeeIds AS existed ON empId.Id = existed.Id

	-- find employee don't have setting Flow Template / Target Template
	SELECT @NoneSettingEmployeeIds = COALESCE(@NoneSettingEmployeeIds + ',', '') + CAST(empS.Id AS NVARCHAR(20))
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
						INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON deptFS.DepartmentId = gr.GroupId
						AND deptFS.IsGroup = 1
						INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.GroupId = gr.GroupId
						INNER JOIN dbo.FlowTemplate AS fT WITH (NOLOCK) ON deptFS.FlowTemplateId = fT.FlowTemplateId
					WHERE fT.TargetTypeId = @TargetTypeId
					
					UNION
					SELECT DISTINCT emp.EmployeeId
					FROM #DeptFlowAndTargetTemplate AS emp WITH (NOLOCK)
					WHERE emp.FlowTemplateId IS NOT NULL
					) AS empF
				WHERE empF.EmployeeId = empS.Id
				)
			)
		OR (
			@TargetTypeId <> @PromotionTarget
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
							INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON deptTar.DepartmentId = gr.GroupId
							AND deptTar.IsGroup = 1
							INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.GroupId = gr.GroupId
							INNER JOIN dbo.TargetTemplate AS tarT WITH (NOLOCK) ON deptTar.TargetTemplateId = tarT.TargetTemplateId
						WHERE tarT.TargetTypeId = @TargetTypeId
						
						UNION
						SELECT DISTINCT emp.EmployeeId
						FROM #DeptFlowAndTargetTemplate AS emp WITH (NOLOCK)
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
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON deptFS.DepartmentId = gr.GroupId
			AND deptFS.IsGroup = 1
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.GroupId = gr.GroupId
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
			INNER JOIN #DeptFlowAndTargetTemplate AS deptFS1 WITH (NOLOCK) ON deptFS.DepartmentId = deptFS1.DepartmentId
			AND deptFS.FlowTemplateId = deptFS1.FlowTemplateId
			AND deptFS.IsGroup = 0
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON deptFS1.EmployeeId = emp.EmployeeId
			INNER JOIN @EmployeeIds AS empt ON emp.EmployeeId = empt.Id
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.PeriodFlowSetting AS pFS WITH (NOLOCK)
				WHERE pFS.EmployeeId = emp.EmployeeId
					AND pFS.PeriodId = @PeriodId
				)

		-- insert employee target setting
		IF @TargetTypeId <> @PromotionTarget
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
				INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON deptTS.DepartmentId = gr.GroupId
				AND deptTS.IsGroup = 1
				INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.GroupId = gr.GroupId
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
				INNER JOIN #DeptFlowAndTargetTemplate AS deptTS1 WITH (NOLOCK) ON deptTS.DepartmentId = deptTS1.DepartmentId
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

		IF @TargetTypeId IN (@MonthlyTarget, @SemiTarget, @PromotionTarget)
		BEGIN
			IF ISNULL(@StartDate, '') <> ''
			BEGIN
				UPDATE pe
				SET pe.StartDate = DATEADD(DAY, ISNULL(aa.Duration - peP.Duration, 0), @StartDate)
					,pe.EndDate = DATEADD(DAY, (CASE 
							WHEN ISNULL(aa.Duration, 0) - 1 < 0 THEN 0
							ELSE ISNULL(aa.Duration, 0) - 1 END
							), @StartDate)
				FROM dbo.PeriodFlowSetting AS pe WITH (NOLOCK)
					INNER JOIN dbo.PeriodProcessSetting AS peP WITH (NOLOCK) ON pe.PeriodId = peP.PeriodId
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
				IF @TargetTypeId = @MonthlyTarget
				BEGIN
					SET @ErrMsg = @ErrMsg + '<br/>Start Date / End Date of <b>' + @OverlapEmployees + '</b> overlap on semi.'
				END
				ELSE IF @TargetTypeId = @SemiTarget
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
				IF @TargetTypeId = @MonthlyTarget
				BEGIN
					SET @ErrMsg = @ErrMsg + '<br/>Start Date / End Date of <b>' + @OverlapEmployees + '</b> overlap on semi.'
				END
				ELSE IF @TargetTypeId = @SemiTarget
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

			IF @TargetTypeId = @MonthlyTarget
			BEGIN
				SET @ErrMsg = 'Start Date / End Date of <b>' + @OverlapEmployees + '</b> overlap on semi.'
			END
			ELSE IF @TargetTypeId = @SemiTarget
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

	DROP TABLE #tmpTeam

	DROP TABLE #DeptFlowAndTargetTemplate
END