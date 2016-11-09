
CREATE PROCEDURE [dbo].[EpaAuto_GenerateEPA] @PeriodId INT
	,@EmployeeIds VARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Generate EPA.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @TargetTypeId INT
		,@NumberEmployeeHasEPA INT
		,@Today DATE

	SET @Today = CAST(GETDATE() AS DATE)

	SELECT @TargetTypeId = TargetTypeId
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	CREATE TABLE #PeriodEmployee (
		PeriodId INT
		,EmployeeId INT
		,HasMonthlyEPA INT
		,TargetTemplateId INT
		)

	IF ISNULL(@EmployeeIds, '') = ''
	BEGIN
		INSERT INTO #PeriodEmployee (
			PeriodId
			,EmployeeId
			,HasMonthlyEPA
			,TargetTemplateId
			)
		SELECT DISTINCT peFS.PeriodId
			,peFS.EmployeeId
			,(CASE 
				WHEN pe.TargetTypeId = 3 -- semi
					AND emp.HasMonthlyEPA = 1 THEN 1
				ELSE 0 END
				) AS HasMonthlyEPA
			,peTS.TargetTemplateId
		FROM dbo.Period AS pe WITH (NOLOCK)
			INNER JOIN dbo.PeriodFlowSetting AS peFS WITH (NOLOCK) ON pe.PeriodId = peFS.PeriodId
			INNER JOIN dbo.PeriodTargetSetting AS peTS WITH (NOLOCK) ON peFS.PeriodId = peTS.PeriodId
			AND peFS.EmployeeId = peTS.EmployeeId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON peFS.EmployeeId = emp.EmployeeId
			AND ISNULL(emp.IsDeleted, 0) = 0
			AND emp.[Enabled] = 1
		WHERE CAST(pe.StartDate AS DATE) = @Today
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.EPA AS epa WITH (NOLOCK)
				WHERE pe.PeriodId = epa.PeriodId
					AND emp.EmployeeId = epa.EmployeeId
				)
	END
	ELSE
	BEGIN
		CREATE TABLE #EmployeeTemp (
			PeriodId INT
			,EmployeeId INT
			)

		INSERT INTO #EmployeeTemp (
			PeriodId
			,EmployeeId
			)
		SELECT DISTINCT @PeriodId
			,val
		FROM dbo.fnc_strSplit(@EmployeeIds, ',')

		SELECT @NumberEmployeeHasEPA = COUNT(epa.EPAId)
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN #EmployeeTemp AS emp ON emp.PeriodId = epa.PeriodId
			AND emp.EmployeeId = epa.EmployeeId
		GROUP BY epa.PeriodId

		DELETE emp
		FROM #EmployeeTemp AS emp
			INNER JOIN dbo.EPA AS epa ON emp.EmployeeId = epa.EmployeeId
			AND epa.PeriodId = @PeriodId

		INSERT INTO #PeriodEmployee (
			PeriodId
			,EmployeeId
			,HasMonthlyEPA
			,TargetTemplateId
			)
		SELECT DISTINCT empT.PeriodId
			,peTS.EmployeeId
			,(CASE 
				WHEN emp.HasMonthlyEPA = 1 THEN 1
				ELSE 0 END
				) AS HasMonthlyEPA
			,peTS.TargetTemplateId
		FROM #EmployeeTemp AS empT WITH (NOLOCK)
			INNER JOIN dbo.PeriodTargetSetting AS peTS WITH (NOLOCK) ON empT.PeriodId = peTS.PeriodId
			AND empT.EmployeeId = peTS.EmployeeId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON empT.EmployeeId = emp.EmployeeId
			AND ISNULL(emp.IsDeleted, 0) = 0
			AND emp.[Enabled] = 1
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.EPA AS epa WITH (NOLOCK)
				WHERE empT.EmployeeId = epa.EmployeeId
					AND peTS.PeriodId = epa.PeriodId
				)

		DROP TABLE #EmployeeTemp
	END

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = dbo.fncGetDataForWriteLog_Period_GenerateEPA @PeriodId = @PeriodId
		,@EmployeeIds = @EmployeeIds

	BEGIN TRY
		BEGIN TRANSACTION

		INSERT INTO dbo.EPA (
			EmployeeId
			,PeriodId
			,[Status]
			,ScoreRatingTypeId
			,RatingTypeId
			,HasMonthlyEPA
			,FinalScore
			,Grade
			,DisciplineScore
			,OldFinalScore
			,OldGrade
			,OldDisciplineScore
			,ModifiedBy
			,ModifiedDate
			)
		SELECT peEmp.EmployeeId
			,peEmp.PeriodId
			,(CASE 
				WHEN pfs.EndDate < @Today THEN 1
				ELSE 0 END
				) AS [Status] -- 0: Opened; 1: Closed
			,tg.ScoreRatingTypeId
			,tg.RatingTypeId
			,(CASE 
				WHEN @TargetTypeId <> 3 THEN 0
				ELSE peEmp.HasMonthlyEPA END
				) AS HasMonthlyEPA
			,NULL
			,NULL
			,NULL
			,NULL
			,NULL
			,NULL
			,NULL
			,GETDATE()
		FROM #PeriodEmployee AS peEmp
			INNER JOIN dbo.TargetTemplate AS tg WITH (NOLOCK) ON peEmp.TargetTemplateId = tg.TargetTemplateId
			INNER JOIN (
			SELECT DISTINCT pfs.PeriodId
				,pfs.EmployeeId
				,CAST(MAX(pfs.EndDate) AS DATE) AS EndDate
			FROM dbo.PeriodFlowSetting AS pfs WITH (NOLOCK)
			GROUP BY pfs.PeriodId
				,pfs.EmployeeId
			) AS pfs ON pfs.PeriodId = peEmp.PeriodId
			AND pfs.EmployeeId = peEmp.EmployeeId

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
			,(CASE peFS.EvaluatorId
				WHEN - 1 THEN epa.EmployeeId
				ELSE peFS.EvaluatorId END
				) AS EvaluatorId
			,tarGr.TargetTemplateId
			,kpi.KPIId
			,kpiGr.KPIGroupId
			,NULL
			,NULL
			,(CASE 
				WHEN tarKPI.HasComment IS NOT NULL THEN tarKPI.HasComment
				ELSE kpi.HasComment END
				) AS HasComment
			,(CASE 
				WHEN tarKPI.IsRequiredComment IS NOT NULL THEN tarKPI.IsRequiredComment
				ELSE kpi.IsRequiredComment END
				) AS IsRequiredComment
			,NULL
			,(CASE 
				WHEN tarKPI.IsRequiredEvaluate IS NOT NULL THEN tarKPI.IsRequiredEvaluate
				ELSE kpi.IsRequiredEvaluate END
				) AS IsRequiredEvaluate
			,kpi.LogNo
			,kpiGr.LogNo
			,tarKPI.DisplayOrder
			,tarGr.DisplayOrder
			,tarGr.IsSpreadKPI
			,tarGr.[Weight]
			,tarKPI.[Weight]
			,NULL
			,GETDATE()
		FROM #PeriodEmployee AS peEmp
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON peEmp.PeriodId = epa.PeriodId
			AND peEmp.EmployeeId = epa.EmployeeId
			INNER JOIN dbo.PeriodFlowSetting AS peFS WITH (NOLOCK) ON peEmp.PeriodId = peFS.PeriodId
			AND peEmp.EmployeeId = peFS.EmployeeId
			INNER JOIN dbo.TargetTemplateKPIGroup AS tarGr WITH (NOLOCK) ON peEmp.TargetTemplateId = tarGr.TargetTemplateId
			INNER JOIN dbo.KPIGroup AS kpiGr WITH (NOLOCK) ON tarGr.KPIGroupId = kpiGr.KPIGroupId
			INNER JOIN dbo.TargetTemplateKPI AS tarKPI WITH (NOLOCK) ON peEmp.TargetTemplateId = tarKPI.TargetTemplateId
			AND tarGr.KPIGroupId = tarKPI.KPIGroupId
			INNER JOIN dbo.KPI AS kpi WITH (NOLOCK) ON tarKPI.KPIId = kpi.KPIId
		WHERE peFS.ProcessId IN (2, 3) -- Self and Evaluator process

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
			,peFS.ProcessId
			,CASE peFS.EvaluatorId
				WHEN - 1 THEN epa.EmployeeId
				ELSE peFS.EvaluatorId
				END
			,peFS.[Priority]
			,NULL
			,peFS.StartDate
			,peFS.EndDate
			,(CASE 
				WHEN (
						@TargetTypeId = 3 -- semi
						AND peFS.ProcessId IN (2, 3) -- self
						AND peEmp.HasMonthlyEPA = 1
						)
					OR (CAST(peFS.EndDate AS DATE) < @Today) THEN 1
				ELSE 0 END
				) AS IsEndProcess -- 0: in progress; 1: end progress
			,NULL
			,NULL
			,pePro.NoticeAtBegin
			,DATEADD(DAY, - pePro.RemindDay, peFS.EndDate)
			,0 -- Open
			,NULL
			,NULL
			,GETDATE()
		FROM #PeriodEmployee AS peEmp
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON peEmp.PeriodId = epa.PeriodId
			AND peEmp.EmployeeId = epa.EmployeeId
			INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON peEmp.PeriodId = pe.PeriodId
			INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON pe.TargetTypeId = tt.TargetTypeId
			INNER JOIN dbo.PeriodFlowSetting AS peFS WITH (NOLOCK) ON peEmp.PeriodId = peFS.PeriodId
			AND peEmp.EmployeeId = peFS.EmployeeId
			INNER JOIN dbo.PeriodProcessSetting AS pePro WITH (NOLOCK) ON peEmp.PeriodId = pePro.PeriodId
			AND peFS.ProcessId = pePro.ProcessId

		-- semi has Monthly
		INSERT INTO dbo.SemiEPADetail (
			EPAId
			,[Month]
			,[Year]
			)
		SELECT epa.EPAId
			,semiSD.[Month]
			,(CASE 
				WHEN semiSD.IsLastYear = 1 THEN YEAR(period.StartDate) - 1
				ELSE YEAR(period.StartDate) END
				) AS [Year]
		FROM #PeriodEmployee AS peEmp
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON peEmp.PeriodId = epa.PeriodId
			AND peEmp.EmployeeId = epa.EmployeeId
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.TargetTypeId = 3 -- semi
			INNER JOIN dbo.SemiPeriodSetting AS semiS WITH (NOLOCK) ON CAST(period.StartDate AS DATE) BETWEEN CAST(CAST(YEAR(period.StartDate) AS VARCHAR) + '-' + semiS.FromDate AS DATE)
				AND CAST(CAST(YEAR(period.StartDate) AS VARCHAR) + '-' + semiS.ToDate AS DATE)
			INNER JOIN dbo.SemiPeriodSettingDetail AS semiSD WITH (NOLOCK) ON semiS.SemiPeriodSettingId = semiSD.SemiPeriodSettingId
			AND semiSD.IsActive = 1
		WHERE epa.HasMonthlyEPA = 1

		-- recommendation
		INSERT INTO dbo.EPARecommendation (
			EPAId
			,ProbationStatus
			,Promoted
			,PayIncreased
			,JobGradeIncreased
			,JobGradeId
			)
		SELECT epa.EPAId
			,NULL
			,NULL
			,NULL
			,NULL
			,NULL
		FROM #PeriodEmployee AS peEmp
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON peEmp.PeriodId = epa.PeriodId
			AND peEmp.EmployeeId = epa.EmployeeId
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.TargetTypeId <> 2 -- monthly

		EXEC @Data = dbo.fncGetDataForWriteLog_Period_GenerateEPA @PeriodId = @PeriodId
			,@EmployeeIds = @EmployeeIds

		EXEC dbo.LogDetail_Insert @ActorId = NULL
			,@SpName = 'EpaAuto_GenerateEPA'
			,@ObjectId = @PeriodId
			,@ObjectTypeId = 10
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		IF @NumberEmployeeHasEPA > 0
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = N'Only generate EPA of ' + CAST((
						SELECT COUNT(*)
						FROM #PeriodEmployee
						) AS NVARCHAR) + ' employees successfully. The other employees already exist EPA.'

			RETURN
		END

		COMMIT TRANSACTION

		DROP TABLE #PeriodEmployee
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Generate EPA unsuccessfully.'
	END CATCH
END