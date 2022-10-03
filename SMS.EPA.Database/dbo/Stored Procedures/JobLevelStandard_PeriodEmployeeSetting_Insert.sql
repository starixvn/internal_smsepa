
CREATE PROCEDURE [dbo].[JobLevelStandard_PeriodEmployeeSetting_Insert] @PeriodId INT
	,@TeamIds VARCHAR(MAX) = NULL
	,@EmployeeId INT = NULL
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20220816@Tim.Trinh
Task: insert employee to period.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SemiTargetTypeId TINYINT = 3
		,@TemplateId INT = 1
		,@ModifiedDate DATETIME = GETDATE()

	CREATE TABLE #tmpTeam (TeamId INT)

	CREATE TABLE #tmpEmployee (
		EmployeeId INT
		,TeamId INT
		)

	CREATE TABLE #tmpEmployeeSetting (
		ProcessId TINYINT
		,EmployeeId INT
		,EvaluatorId INT
		)

	CREATE TABLE #tmpInsertedEmployeeSetting (EmployeeId INT, TemplateId INT, IsExcluded BIT)
	CREATE TABLE #tmpDeletedEmployeeSetting (EmployeeId INT, TemplateId INT, IsExcluded BIT)

	CREATE TABLE #tmpDeletedAssessment (AssessmentId INT)

	IF ISNULL(@TeamIds, '') != ''
	BEGIN
		INSERT INTO #tmpTeam (TeamId)
		SELECT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
	END

	INSERT INTO #tmpEmployee (
		EmployeeId
		,TeamId
		)
	SELECT emp.EmployeeId
		,emp.GroupId
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND (
			emp.EmployeeId = @EmployeeId
			OR EXISTS (
				SELECT 1
				FROM #tmpTeam AS tmp WITH (NOLOCK)
				WHERE emp.GroupId = tmp.TeamId
				)
			)

	INSERT INTO #tmpEmployeeSetting (
		ProcessId
		,EmployeeId
		,EvaluatorId
		)
	SELECT DISTINCT CASE 
			WHEN efs.ProcessId = 3
				OR efs.ProcessId = 4 THEN 1
			ELSE 3
			END
		,efs.EmployeeId
		,efs.EvaluatorId
	FROM dbo.SettingEmployeeFlow AS efs WITH (NOLOCK)
		INNER JOIN dbo.FlowTemplate AS flow WITH (NOLOCK) ON efs.FlowTemplateId = flow.FlowTemplateId
		AND flow.TargetTypeId = @SemiTargetTypeId
		AND flow.IsActive = 1
		INNER JOIN #tmpEmployee AS emp WITH (NOLOCK) ON efs.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON emp.EmployeeId != eva.EmployeeId
		AND efs.EvaluatorId = eva.EmployeeId
		AND eva.[Enabled] = 1
		AND eva.IsDeleted = 0
	WHERE efs.ProcessId > 2

	INSERT INTO #tmpEmployeeSetting (
		ProcessId
		,EmployeeId
		,EvaluatorId
		)
	SELECT DISTINCT CASE 
			WHEN dfs.ProcessId = 3
				OR dfs.ProcessId = 4 THEN 1
			ELSE 3
			END
		,emp.EmployeeId
		,dfs.EvaluatorId
	FROM dbo.SettingDepartmentFlow AS dfs WITH (NOLOCK)
		INNER JOIN dbo.FlowTemplate AS flow WITH (NOLOCK) ON dfs.FlowTemplateId = flow.FlowTemplateId
		AND flow.TargetTypeId = @SemiTargetTypeId
		AND flow.IsActive = 1
		INNER JOIN #tmpEmployee AS emp WITH (NOLOCK) ON dfs.DepartmentId = emp.TeamId
		INNER JOIN dbo.Employee AS eva WITH (NOLOCK) ON emp.EmployeeId != eva.EmployeeId
		AND dfs.EvaluatorId = eva.EmployeeId
		AND eva.[Enabled] = 1
		AND eva.IsDeleted = 0
	WHERE dfs.IsGroup = 1
		AND dfs.ProcessId > 2
		AND NOT EXISTS (
			SELECT 1
			FROM #tmpEmployeeSetting AS tmp WITH (NOLOCK)
			WHERE emp.EmployeeId = tmp.EmployeeId
			)

	INSERT INTO #tmpEmployeeSetting (
		ProcessId
		,EmployeeId
		,EvaluatorId
		)
	SELECT DISTINCT 2
		,EmployeeId
		,EmployeeId
	FROM #tmpEmployeeSetting WITH (NOLOCK)

	DELETE pes
	OUTPUT DELETED.EmployeeId, DELETED.TemplateId, DELETED.IsExcluded
	INTO #tmpDeletedEmployeeSetting(EmployeeId, TemplateId, IsExcluded)
	FROM dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (ROWLOCK, UPDLOCK)
	WHERE pes.PeriodId = @PeriodId
		AND EXISTS (
			SELECT 1
			FROM #tmpEmployee AS tmp WITH (NOLOCK)
			WHERE pes.EmployeeId = tmp.EmployeeId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM #tmpEmployeeSetting AS tmp WITH (NOLOCK)
			WHERE pes.EmployeeId = tmp.EmployeeId
			)

	INSERT INTO dbo.JobLevelStandard_PeriodEmployeeSetting (
		PeriodId
		,EmployeeId
		,TemplateId
		,IsExcluded
		,CreatedBy
		,CreatedDate
		)
	OUTPUT INSERTED.EmployeeId, INSERTED.TemplateId, INSERTED.IsExcluded
	INTO #tmpInsertedEmployeeSetting(EmployeeId, TemplateId, IsExcluded)
	SELECT DISTINCT @PeriodId
		,EmployeeId
		,@TemplateId
		,0
		,@ModifiedBy
		,@ModifiedDate
	FROM #tmpEmployeeSetting AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK)
			WHERE pes.PeriodId = @PeriodId
				AND pes.EmployeeId = tmp.EmployeeId
			)

	DELETE pps
	FROM dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (ROWLOCK, UPDLOCK)
	WHERE pps.PeriodId = @PeriodId
		AND EXISTS (
			SELECT 1
			FROM #tmpEmployee AS tmp WITH (NOLOCK)
			WHERE pps.EmployeeId = tmp.EmployeeId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM #tmpEmployeeSetting AS tmp WITH (NOLOCK)
			WHERE pps.ProcessId = tmp.ProcessId
				AND pps.EmployeeId = tmp.EmployeeId
				AND pps.EvaluatorId = tmp.EvaluatorId
			)

	INSERT INTO dbo.JobLevelStandard_PeriodProcessSetting (
		PeriodId
		,ProcessId
		,EmployeeId
		,EvaluatorId
		,FromDate
		,ToDate
		,CreatedBy
		,CreatedDate
		)
	SELECT @PeriodId
		,ps.ProcessId
		,tmp.EmployeeId
		,tmp.EvaluatorId
		,ps.FromDate
		,ps.ToDate
		,@ModifiedBy
		,@ModifiedDate
	FROM #tmpEmployeeSetting AS tmp WITH (NOLOCK)
		INNER JOIN dbo.JobLevelStandard_PeriodSetting AS ps WITH (NOLOCK) ON ps.PeriodId = @PeriodId
		AND tmp.ProcessId = ps.ProcessId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_PeriodProcessSetting AS pps WITH (NOLOCK)
			WHERE pps.PeriodId = @PeriodId
				AND pps.ProcessId = tmp.ProcessId
				AND pps.EmployeeId = tmp.EmployeeId
				AND pps.EvaluatorId = tmp.EvaluatorId
			)

	DELETE a
	OUTPUT DELETED.AssessmentId
	INTO #tmpDeletedAssessment(AssessmentId)
	FROM dbo.JobLevelStandard_Assessment AS a WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpDeletedEmployeeSetting AS tmp WITH (NOLOCK) ON a.PeriodId = @PeriodId
		AND a.EmployeeId = tmp.EmployeeId

	DELETE a
	FROM dbo.JobLevelStandard_AssessmentKPI AS a WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpDeletedAssessment AS tmp WITH (NOLOCK) ON a.AssessmentId = tmp.AssessmentId

	-- write log
	DECLARE @logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	SET @xml = (
			SELECT EmployeeId AS '@EmployeeId'
				,TemplateId AS '@TemplateId'
				,IsExcluded AS '@IsExcluded'
			FROM #tmpDeletedEmployeeSetting WITH (NOLOCK)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	SET @xml = (
			SELECT EmployeeId AS '@EmployeeId'
				,TemplateId AS '@TemplateId'
				,IsExcluded AS '@IsExcluded'
			FROM #tmpInsertedEmployeeSetting WITH (NOLOCK)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SpName = 'JobLevelStandard_PeriodEmployeeSetting_Insert'
		,@ObjectId = @PeriodId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@ObjectIds = NULL

	DROP TABLE #tmpTeam

	DROP TABLE #tmpEmployee

	DROP TABLE #tmpEmployeeSetting

	DROP TABLE #tmpInsertedEmployeeSetting

	DROP TABLE #tmpDeletedEmployeeSetting

	DROP TABLE #tmpDeletedAssessment
END