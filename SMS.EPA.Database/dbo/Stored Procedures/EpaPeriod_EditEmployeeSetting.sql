
CREATE PROCEDURE [dbo].[EpaPeriod_EditEmployeeSetting] @PeriodId INT
	,@TargetTemplateId INT = NULL
	,@EmployeeAssignments NVARCHAR(MAX) = NULL
	,@ModifiedBy INT = NULL
	,@DisciplineStartDate DATE = NULL
	,@DisciplineEndDate DATE = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Edit employee setting of period: target template, evaluator, reviewer, management
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
	@EmployeeAssignments = 
		'<ROOT>
			<Assignment EmployeeId='' ProcessId='' EvaluatorId='' StartDate='' EndDate='' Priority=''></Assignment>
		</ROOT>'
*/
BEGIN
	SET NOCOUNT
		,XACT_ABORT ON;

	DECLARE @TargetTypeId INT
		,@EmployeeIds IntId
		,@OverlapEmployeeIds IntId
		,@OverlapEmployees NVARCHAR(MAX)
		,@Today DATE = GETDATE()
		,@ProbationTarget TINYINT = 1
		,@MonthlyTarget TINYINT = 2
		,@SemiTarget TINYINT = 3
		,@PromotionTarget TINYINT = 5

	SELECT @TargetTypeId = TargetTypeId
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	CREATE TABLE #tblEmployeeChangedTarget (
		EPAId INT
		,PeriodId INT
		,EmployeeId INT
		,TargetTemplateId INT
		)

	CREATE TABLE #tblEmployeeAssignments (
		PeriodId INT
		,EmployeeId INT
		,EvaluatorId INT
		,ProcessId INT
		,StartDate DATE NULL
		,EndDate DATE NULL
		,[Priority] INT
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@EmployeeAssignments

		INSERT INTO #tblEmployeeAssignments (
			PeriodId
			,EmployeeId
			,EvaluatorId
			,ProcessId
			,StartDate
			,EndDate
			,[Priority]
			)
		SELECT @PeriodId
			,EmployeeId
			,CASE WHEN ProcessId = 2 THEN EmployeeId ELSE EvaluatorId END
			,ProcessId
			,CASE 
				WHEN StartDate = '01 Jan, 1900' THEN NULL
				ELSE StartDate
				END
			,CASE 
				WHEN EndDate = '01 Jan, 1900' THEN NULL
				ELSE EndDate
				END
			,[Priority]
		FROM OPENXML(@DocHandler, '/ROOT/Assignment', 1) WITH (EmployeeId INT, EvaluatorId INT, ProcessId INT, StartDate DATE, EndDate DATE, [Priority] INT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update employee setting flow unsuccessfully. Paramter invalid.'

		RETURN
	END CATCH

	INSERT INTO @EmployeeIds (Id)
	SELECT DISTINCT EmployeeId
	FROM #tblEmployeeAssignments WITH (NOLOCK)

	-- check overlap monthly, semi
	INSERT INTO @OverlapEmployeeIds (Id)
	EXEC dbo.EpaPeriod_EditEmployeeSetting_CheckEmployeeOverlap @PeriodId
		,@TargetTypeId
		,@EmployeeAssignments

	DELETE empId
	FROM @EmployeeIds AS empId
		INNER JOIN @OverlapEmployeeIds AS overId ON empId.Id = overId.Id

	DELETE empA
	FROM #tblEmployeeAssignments AS empA
		INNER JOIN @OverlapEmployeeIds AS overId ON empA.EmployeeId = overId.Id

	-- write log
	DECLARE @OldData NVARCHAR(MAX)
		,@EmployeeId INT
		,@EmpCursor CURSOR
		,@EmpLogDetail NVARCHAR(MAX)

	SET @OldData = '<r>'
	SET @EmpCursor = CURSOR
	FOR

	SELECT Id
	FROM @EmployeeIds	

	SET @OldData = @OldData + '</r>'

	BEGIN TRY
		BEGIN TRANSACTION

		IF ISNULL(@TargetTemplateId, '') <> ''
		BEGIN
			INSERT INTO #tblEmployeeChangedTarget (
				EPAId
				,PeriodId
				,EmployeeId
				,TargetTemplateId
				)
			SELECT DISTINCT epa.EPAId
				,@PeriodId
				,emp.EmployeeId
				,@TargetTemplateId
			FROM #tblEmployeeAssignments AS emp WITH (NOLOCK)
				INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON emp.PeriodId = epa.PeriodId
				AND emp.EmployeeId = epa.EmployeeId
				INNER JOIN (
				SELECT DISTINCT EmployeeId
				FROM dbo.PeriodTargetSetting WITH (NOLOCK)
				WHERE PeriodId = @PeriodId
					AND TargetTemplateId <> @TargetTemplateId
				) AS pTS ON emp.EmployeeId = pTS.EmployeeId

			DELETE pTS
			FROM dbo.PeriodTargetSetting AS pTS
				INNER JOIN #tblEmployeeAssignments AS empAssign WITH (NOLOCK) ON pTS.EmployeeId = empAssign.EmployeeId
			WHERE pTS.PeriodId = @PeriodId

			INSERT INTO dbo.PeriodTargetSetting (
				PeriodId
				,EmployeeId
				,TargetTemplateId
				,ModifiedBy
				,ModifiedDate
				)
			SELECT DISTINCT PeriodId
				,EmployeeId
				,@TargetTemplateId
				,@ModifiedBy
				,GETDATE()
			FROM #tblEmployeeAssignments WITH (NOLOCK)
		END

		DELETE pFS
		FROM dbo.PeriodFlowSetting AS pFS
			INNER JOIN #tblEmployeeAssignments AS empAssign WITH (NOLOCK) ON pFS.EmployeeId = empAssign.EmployeeId
		WHERE pFS.PeriodId = @PeriodId

		INSERT INTO dbo.PeriodFlowSetting (
			PeriodId
			,EmployeeId
			,ProcessId
			,EvaluatorId
			,StartDate
			,EndDate
			,[Priority]
			,ModifiedBy
			,ModifiedDate
			)
		SELECT DISTINCT PeriodId
			,EmployeeId
			,ProcessId
			,EvaluatorId
			,StartDate
			,EndDate
			,[Priority]
			,@ModifiedBy
			,GETDATE()
		FROM #tblEmployeeAssignments WITH (NOLOCK)

		-- Update EPA when change Period Setting
		CREATE TABLE #tblEmployeeProcess (
			EPAId INT
			,EmployeeId INT
			,ProcessId INT
			,EvaluatorId INT
			,StartDate DATETIME
			,EndDate DATETIME
			,[Priority] INT
			)

		INSERT INTO #tblEmployeeProcess (
			EPAId
			,ProcessId
			,EvaluatorId
			,StartDate
			,EndDate
			,[Priority]
			)
		SELECT DISTINCT epa.EPAId
			,empA.ProcessId
			,empA.EvaluatorId
			,empA.StartDate
			,empA.EndDate
			,empA.[Priority]
		FROM #tblEmployeeAssignments AS empA WITH (NOLOCK)
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON empA.PeriodId = epa.PeriodId
			AND empA.EmployeeId = epa.EmployeeId

		IF EXISTS (
				SELECT 1
				FROM #tblEmployeeProcess WITH (NOLOCK)
				)
		BEGIN
			-- Change Target Template
			IF EXISTS (
					SELECT 1
					FROM #tblEmployeeChangedTarget WITH (NOLOCK)
					)
			BEGIN
				UPDATE epa
				SET epa.ScoreRatingTypeId = tg.ScoreRatingTypeId
					,epa.RatingTypeId = tg.RatingTypeId
				FROM dbo.EPA AS epa
					INNER JOIN #tblEmployeeChangedTarget AS empT WITH (NOLOCK) ON epa.EPAId = empT.EPAId
					INNER JOIN dbo.TargetTemplate AS tg WITH (NOLOCK) ON empT.TargetTemplateId = tg.TargetTemplateId

				DELETE dt
				FROM dbo.EPADetail AS dt
					INNER JOIN #tblEmployeeChangedTarget AS emp WITH (NOLOCK) ON dt.EPAId = emp.EPAId

				UPDATE epa
				SET epa.FinalScore = NULL
					,epa.DisciplineScore = NULL
					,epa.Grade = NULL
					,epa.OldFinalScore = NULL
					,epa.OldDisciplineScore = NULL
					,epa.OldGrade = NULL
				FROM dbo.EPA AS epa
					INNER JOIN #tblEmployeeChangedTarget AS empT WITH (NOLOCK) ON epa.EPAId = empT.EPAId

				UPDATE epaP
				SET epaP.TotalScore = NULL
					,epaP.AdjustScore = NULL
					,epaP.EvaluateDate = NULL
					,epaP.Comment = NULL
					,epaP.ModifiedBy = NULL
					,epaP.ModifiedDate = NULL
				FROM dbo.EPAProcess AS epaP
					INNER JOIN #tblEmployeeChangedTarget AS empT WITH (NOLOCK) ON epaP.EPAId = empT.EPAId

				INSERT INTO dbo.EPADetail (
					EPAId
					,ProcessId
					,EvaluatorId
					,TargetTemplateId
					,KPIId
					,KPIGroupId
					,Score
					,Rate
					,HasComment
					,IsRequiredComment
					,Comment
					,IsRequiredEvaluate
					,KPILogNo
					,KPIGroupLogNo
					,KPIDisplayOrder
					,KPIGroupDisplayOrder
					,IsSpreadKPI
					,KPIGroupWeight
					,KPIWeight
					,ModifiedBy
					,ModifiedDate
					)
				SELECT epa.EPAId
					,peFS.ProcessId
					,CASE peFS.EvaluatorId
						WHEN - 1 THEN epa.EmployeeId
						ELSE peFS.EvaluatorId
						END
					,tarGr.TargetTemplateId
					,kpi.KPIId
					,kpiGr.KPIGroupId
					,NULL
					,NULL
					,CASE 
						WHEN tarKPI.HasComment IS NOT NULL THEN tarKPI.HasComment
						ELSE kpi.HasComment
						END
					,CASE 
						WHEN tarKPI.IsRequiredComment IS NOT NULL THEN tarKPI.IsRequiredComment
						ELSE kpi.IsRequiredComment
						END
					,NULL
					,CASE 
						WHEN tarKPI.IsRequiredEvaluate IS NOT NULL THEN tarKPI.IsRequiredEvaluate
						ELSE kpi.IsRequiredEvaluate
						END
					,kpi.LogNo
					,kpiGr.LogNo
					,tarKPI.DisplayOrder
					,tarGr.DisplayOrder
					,tarGr.IsSpreadKPI
					,tarGr.[Weight]
					,tarKPI.[Weight]
					,NULL
					,GETDATE()
				FROM #tblEmployeeChangedTarget AS peEmp WITH (NOLOCK)
					INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON peEmp.PeriodId = epa.PeriodId
					AND peEmp.EmployeeId = epa.EmployeeId
					INNER JOIN dbo.PeriodFlowSetting AS peFS WITH (NOLOCK) ON peEmp.PeriodId = peFS.PeriodId
					AND peEmp.EmployeeId = peFS.EmployeeId
					INNER JOIN dbo.TargetTemplateKPIGroup AS tarGr WITH (NOLOCK) ON peEmp.TargetTemplateId = tarGr.TargetTemplateId
					INNER JOIN dbo.KPIGroup AS kpiGr WITH (NOLOCK) ON tarGr.KPIGroupId = kpiGr.KPIGroupId
					INNER JOIN dbo.TargetTemplateKPI AS tarKPI WITH (NOLOCK) ON peEmp.TargetTemplateId = tarKPI.TargetTemplateId
					AND tarGr.KPIGroupId = tarKPI.KPIGroupId
					INNER JOIN dbo.KPI AS kpi WITH (NOLOCK) ON tarKPI.KPIId = kpi.KPIId
				WHERE peFS.ProcessId IN (2, 3) -- Self-evaluator and Evaluator process
			END

			-- Delete process
			CREATE TABLE #tblProcessDeleted (
				EPAId INT
				,ProcessId INT
				,EvaluatorId INT
				)

			INSERT INTO #tblProcessDeleted (
				EPAId
				,ProcessId
				,EvaluatorId
				)
			SELECT DISTINCT epaP.EPAId
				,epaP.ProcessId
				,epaP.EvaluatorId
			FROM dbo.EPAProcess AS epaP WITH (NOLOCK)
				INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epaP.EPAId = epa.EPAId
				INNER JOIN #tblEmployeeProcess AS empP WITH (NOLOCK) ON epa.EPAId = empP.EPAId
			WHERE NOT EXISTS (
					SELECT 1
					FROM #tblEmployeeProcess AS empP1 WITH (NOLOCK)
					WHERE empP1.ProcessId = epaP.ProcessId
						AND empP1.EvaluatorId = epaP.EvaluatorId
						AND empP1.EPAId = epaP.EPAId
					)

			DELETE pc
			FROM dbo.EPAProcess AS pc
				INNER JOIN #tblProcessDeleted AS del WITH (NOLOCK) ON pc.EPAId = del.EPAId
				AND pc.ProcessId = del.ProcessId
				AND pc.EvaluatorId = del.EvaluatorId

			DELETE dt
			FROM dbo.EPADetail AS dt
				INNER JOIN #tblProcessDeleted AS del WITH (NOLOCK) ON dt.EPAId = del.EPAId
				AND dt.ProcessId = del.ProcessId
				AND dt.EvaluatorId = del.EvaluatorId
			WHERE NOT EXISTS (
					SELECT 1
					FROM #tblEmployeeChangedTarget WITH (NOLOCK)
					WHERE dt.EPAId = EPAId
					)

			DROP TABLE #tblProcessDeleted

			-- Insert new process and epa detail of new process
			CREATE TABLE #tblProcessNew (
				EPAId INT
				,ProcessId INT
				,EvaluatorId INT
				)

			INSERT INTO #tblProcessNew (
				EPAId
				,ProcessId
				,EvaluatorId
				)
			SELECT empP.EPAId
				,empP.ProcessId
				,empP.EvaluatorId
			FROM #tblEmployeeProcess AS empP WITH (NOLOCK)
				INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON empP.EPAId = epa.EPAId
			WHERE NOT EXISTS (
					SELECT 1
					FROM dbo.EPAProcess AS epaP WITH (NOLOCK)
					WHERE epaP.ProcessId = empP.ProcessId
						AND epaP.EvaluatorId = empP.EvaluatorId
						AND EXISTS (
							SELECT 1
							FROM dbo.EPA WITH (NOLOCK)
							WHERE EPAId = empP.EPAId
								AND EPAId = epaP.EPAId
							)
					)

			INSERT INTO dbo.EPADetail (
				EPAId
				,ProcessId
				,EvaluatorId
				,TargetTemplateId
				,KPIId
				,KPIGroupId
				,Score
				,Rate
				,HasComment
				,IsRequiredComment
				,Comment
				,IsRequiredEvaluate
				,KPILogNo
				,KPIGroupLogNo
				,KPIDisplayOrder
				,KPIGroupDisplayOrder
				,IsSpreadKPI
				,KPIGroupWeight
				,KPIWeight
				,ModifiedBy
				,ModifiedDate
				)
			SELECT epa.EPAId
				,proN.ProcessId
				,CASE proN.EvaluatorId
					WHEN - 1 THEN epa.EmployeeId
					ELSE proN.EvaluatorId
					END
				,epaD.TargetTemplateId
				,epaD.KPIId
				,epaD.KPIGroupId
				,NULL
				,NULL
				,epaD.HasComment
				,epaD.IsRequiredComment
				,NULL
				,epaD.IsRequiredEvaluate
				,epaD.KPILogNo
				,epaD.KPIGroupLogNo
				,epaD.KPIDisplayOrder
				,epaD.KPIGroupDisplayOrder
				,epaD.IsSpreadKPI
				,epaD.KPIGroupWeight
				,epaD.KPIWeight
				,NULL
				,GETDATE()
			FROM #tblProcessNew AS proN WITH (NOLOCK)
				INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON proN.EPAId = epa.EPAId
				INNER JOIN dbo.EPADetail AS epaD WITH (NOLOCK) ON epa.EPAId = epaD.EPAId
				AND epaD.ProcessId = 2 -- Self
			WHERE proN.ProcessId IN (3) -- Self-evaluator and Evaluator process
				AND NOT EXISTS (
					SELECT 1
					FROM #tblEmployeeChangedTarget WITH (NOLOCK)
					WHERE epa.EPAId = EPAId
					)

			INSERT INTO dbo.EPAProcess (
				EPAId
				,ProcessId
				,EvaluatorId
				,[Priority]
				,Comment
				,StartDate
				,EndDate
				,IsEndProcess
				,KPICount
				,TotalScore
				,NoticeAtBegin
				,RemindDate
				,[Status]
				,EvaluateDate
				,ModifiedBy
				,ModifiedDate
				)
			SELECT epa.EPAId
				,empP.ProcessId
				,CASE empP.EvaluatorId
					WHEN - 1 THEN epa.EmployeeId
					ELSE empP.EvaluatorId
					END
				,empP.[Priority]
				,NULL
				,empP.StartDate
				,empP.EndDate
				,CASE 
					WHEN empP.EndDate < @Today THEN 1 -- end progress
					ELSE 0 -- in progress
					END
				,NULL
				,NULL
				,pePro.NoticeAtBegin
				,CASE pePro.RemindDay
					WHEN 0 THEN NULL
					ELSE DATEADD(DAY, - pePro.RemindDay, empP.EndDate)
					END
				,0 -- Open
				,NULL
				,NULL
				,GETDATE()
			FROM #tblProcessNew AS proN WITH (NOLOCK)
				INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON proN.EPAId = epa.EPAId
				INNER JOIN #tblEmployeeProcess AS empP WITH (NOLOCK) ON epa.EPAId = empP.EPAId
				AND proN.ProcessId = empP.ProcessId
				AND proN.EvaluatorId = empP.EvaluatorId
				INNER JOIN dbo.PeriodProcessSetting AS pePro WITH (NOLOCK) ON epa.PeriodId = pePro.PeriodId
				AND proN.ProcessId = pePro.ProcessId

			-- Change start date, end date, remind date, priority
			UPDATE epaP WITH (ROWLOCK, UPDLOCK)
			SET epaP.StartDate = empP.StartDate
				,epaP.RemindDate = DATEADD(DAY, DATEDIFF(DAY, epaP.EndDate, empP.EndDate), epaP.RemindDate)
				,epaP.EndDate = empP.EndDate
				,epaP.[Priority] = empP.[Priority]
				,epaP.IsEndProcess = CASE 
					WHEN @TargetTypeId = 3 -- semi
						AND epaP.ProcessId IN (2, 3) -- self
						AND epa.HasMonthlyEPA = 1 THEN 1 -- end progress
					ELSE CASE 
							WHEN empP.EndDate < @Today THEN 1 -- end progress
							ELSE 0 -- in progress
							END
					END
			FROM dbo.EPAProcess AS epaP
				INNER JOIN #tblEmployeeProcess AS empP WITH (NOLOCK) ON epaP.EPAId = empP.EPAId
				AND epaP.ProcessId = empP.ProcessId
				AND epaP.EvaluatorId = empP.EvaluatorId
				INNER JOIN dbo.EPA AS epa ON epaP.EPAId = epa.EPAId
			WHERE NOT EXISTS (
					SELECT 1
					FROM #tblProcessNew AS pcN WITH (NOLOCK)
					WHERE pcN.EPAId = empP.EPAId
						AND pcN.ProcessId = empP.ProcessId
						AND pcN.EvaluatorId = empP.EvaluatorId
					)

			UPDATE epa WITH (ROWLOCK, UPDLOCK)
			SET [Status] = 1 -- Closed
			FROM dbo.EPA AS epa
				INNER JOIN dbo.Period AS period ON epa.PeriodId = period.PeriodId
				INNER JOIN (
				SELECT DISTINCT pc.EPAId
					,MAX(pc.EndDate) AS EndDate
				FROM dbo.EPAProcess AS pc WITH (NOLOCK)
					INNER JOIN dbo.EPA AS epa1 WITH (NOLOCK) ON pc.EPAId = epa1.EPAId
				WHERE epa1.PeriodId = @PeriodId
				GROUP BY pc.EPAId
				) AS epaP ON epa.EPAId = epaP.EPAId
			WHERE epa.PeriodId = @PeriodId
				AND epa.[Status] <> 2 -- reject
				AND (
					(
						period.TargetTypeId IN (@MonthlyTarget, @SemiTarget)
						AND (
							(
								period.EndDate > epaP.EndDate
								AND period.EndDate < @Today
								)
							OR (
								period.EndDate < epaP.EndDate
								AND epaP.EndDate < @Today
								)
							)
						)
					OR (
						period.TargetTypeId NOT IN (@MonthlyTarget, @SemiTarget)
						AND epaP.EndDate < @Today
						)
					)

			UPDATE epa WITH (ROWLOCK, UPDLOCK)
			SET [Status] = 0 -- Opened
			FROM dbo.EPA AS epa
				INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
				INNER JOIN (
				SELECT DISTINCT pc.EPAId
					,MAX(pc.EndDate) AS EndDate
				FROM dbo.EPAProcess AS pc WITH (NOLOCK)
					INNER JOIN dbo.EPA AS epa1 WITH (NOLOCK) ON pc.EPAId = epa1.EPAId
				WHERE epa1.PeriodId = @PeriodId
				GROUP BY pc.EPAId
				) AS epaP ON epa.EPAId = epaP.EPAId
			WHERE epa.PeriodId = @PeriodId
				AND epa.[Status] <> 2
				AND (
					(
						period.EndDate IS NOT NULL
						AND period.EndDate > @Today
						)
					OR epaP.EndDate >= @Today
					)

			DROP TABLE #tblProcessNew

			DROP TABLE #tblEmployeeProcess
		END

		IF (
				@TargetTypeId = @ProbationTarget
				OR @TargetTypeId = @PromotionTarget
				)
			AND @DisciplineStartDate IS NOT NULL
			AND @DisciplineEndDate IS NOT NULL
		BEGIN
			UPDATE dbo.PeriodDisciplineSetting WITH (ROWLOCK, UPDLOCK)
			SET StartDate = @DisciplineStartDate
				,EndDate = @DisciplineEndDate
			FROM dbo.PeriodDisciplineSetting AS ds WITH (ROWLOCK, UPDLOCK)
				INNER JOIN #tblEmployeeAssignments AS a WITH (NOLOCK) ON ds.PeriodId = a.PeriodId
				AND ds.EmployeeId = a.EmployeeId

			INSERT INTO dbo.PeriodDisciplineSetting (
				PeriodId
				,EmployeeId
				,StartDate
				,EndDate
				)
			SELECT DISTINCT a.PeriodId
				,a.EmployeeId
				,@DisciplineStartDate
				,@DisciplineEndDate
			FROM #tblEmployeeAssignments AS a WITH (NOLOCK)
				LEFT JOIN dbo.PeriodDisciplineSetting AS ds WITH (NOLOCK) ON a.PeriodId = ds.PeriodId
				AND a.EmployeeId = ds.EmployeeId
			WHERE ds.PeriodId IS NULL
		END

		DECLARE @Data NVARCHAR(MAX)

		SET @Data = '<r>'
		SET @EmpCursor = CURSOR
		FOR

		SELECT Id
		FROM @EmployeeIds

		SET @Data = @Data + '</r>'

		EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
			,@SpName = 'EpaPeriod_EditEmployeeSetting'
			,@ObjectId = NULL
			,@ObjectTypeId = 8
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		IF EXISTS (
				SELECT 1
				FROM @OverlapEmployeeIds
				)
		BEGIN
			SELECT @OverlapEmployees = COALESCE(@OverlapEmployees + ', ', '') + NickName
			FROM dbo.Employee WITH (NOLOCK)
			WHERE EmployeeId IN (
					SELECT DISTINCT Id
					FROM @OverlapEmployeeIds
					)

			SELECT @ErrCode = 1

			IF @TargetTypeId = @MonthlyTarget
			BEGIN
				SELECT @ErrMsg = 'Start Date / End Date of <b>' + @OverlapEmployees + '</b> overlap on semi.'
			END
			ELSE IF @TargetTypeId = @SemiTarget
			BEGIN
				SELECT @ErrMsg = 'Start Date / End Date of <b>' + @OverlapEmployees + '</b> overlap on monthly.'
			END

			RETURN
		END

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update employee setting unsuccessfully.'
	END CATCH

	DROP TABLE #tblEmployeeAssignments

	DROP TABLE #tblEmployeeChangedTarget
END