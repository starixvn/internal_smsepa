
CREATE PROCEDURE [dbo].[EpaAuto_GenerateFeedback] @PeriodId INT
	,@EmployeeIds VARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20200525@Tim.Trinh
Task: generate feedback
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FeedbackTargetTypeId TINYINT = 4
		,@SemiTargetTypeId TINYINT = 3
		,@SelfProcessId TINYINT = 2

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.Period WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
				AND TargetTypeId = @SemiTargetTypeId
			)
	BEGIN
		RETURN
	END

	CREATE TABLE #tblFeedback (
		EmployeeId INT
		,EvaluatorId INT
		,FromDate DATE
		,ToDate DATE
		)

	CREATE TABLE #tblInsertedFeedback (
		FeedbackId BIGINT
		,EmployeeId INT
		,EvaluatorId INT
		,TargetTemplateId INT
		)

	CREATE TABLE #tblDeletedFeedback (FeedbackId BIGINT)

	CREATE TABLE #tblDeptFlowAndTargetTemplate (
		DepartmentId INT
		,EmployeeId INT
		,FlowTemplateId INT
		,TargetTemplateId INT
		)

	IF ISNULL(@EmployeeIds, '') = ''
	BEGIN
		INSERT INTO #tblFeedback (
			EmployeeId
			,EvaluatorId
			,FromDate
			,ToDate
			)
		SELECT DISTINCT process.EvaluatorId
			,epa.EmployeeId
			,selfprocess.StartDate
			,selfprocess.EndDate
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS selfprocess WITH (NOLOCK) ON epa.EPAId = selfprocess.EPAId
			AND selfprocess.ProcessId = @SelfProcessId
			INNER JOIN dbo.EPAProcess AS process WITH (NOLOCK) ON epa.EPAId = process.EPAId
			AND process.ProcessId > @SelfProcessId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON process.EvaluatorId = emp.EmployeeId
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
		WHERE epa.PeriodId = @PeriodId
			AND epa.[Status] <> 2 -- rejected
			AND epa.EmployeeId <> process.EvaluatorId
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.Feedback AS fb WITH (NOLOCK)
				WHERE fb.PeriodId = @PeriodId
					AND fb.EmployeeId = process.EvaluatorId
					AND fb.EvaluatorId = epa.EmployeeId
				)
	END
	ELSE
	BEGIN
		INSERT INTO #tblFeedback (
			EmployeeId
			,EvaluatorId
			,FromDate
			,ToDate
			)
		SELECT DISTINCT process.EvaluatorId
			,epa.EmployeeId
			,selfprocess.StartDate
			,selfprocess.EndDate
		FROM dbo.fnc_strSplit(@EmployeeIds, ',') AS emp
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON emp.val = epa.EmployeeId
			INNER JOIN dbo.EPAProcess AS selfprocess WITH (NOLOCK) ON epa.EPAId = selfprocess.EPAId
			AND selfprocess.ProcessId = @SelfProcessId
			INNER JOIN dbo.EPAProcess AS process WITH (NOLOCK) ON epa.EPAId = process.EPAId
			AND process.ProcessId > @SelfProcessId
		WHERE epa.PeriodId = @PeriodId
			AND epa.[Status] <> 2 -- rejected
			AND epa.EmployeeId <> process.EvaluatorId
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.Feedback AS fb WITH (NOLOCK)
				WHERE fb.PeriodId = @PeriodId
					AND fb.EmployeeId = process.EvaluatorId
					AND fb.EvaluatorId = epa.EmployeeId
				)
	END

	INSERT INTO #tblDeletedFeedback (FeedbackId)
	SELECT DISTINCT fb.FeedbackId
	FROM dbo.Feedback AS fb WITH (NOLOCK)
		--INNER JOIN #tblFeedback AS tblfb WITH (NOLOCK) ON fb.EmployeeId = tblfb.EmployeeId
	WHERE fb.PeriodId = @PeriodId
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
				INNER JOIN dbo.EPAProcess AS process WITH (NOLOCK) ON epa.EPAId = process.EPAId
				AND process.ProcessId > @SelfProcessId
				AND fb.EmployeeId = process.EvaluatorId
			WHERE fb.PeriodId = epa.PeriodId
				AND fb.EvaluatorId = epa.EmployeeId
			)

	INSERT INTO #tblDeptFlowAndTargetTemplate (
		DepartmentId
		,EmployeeId
		,FlowTemplateId
		,TargetTemplateId
		)
	EXEC dbo.GetFlowAndTargetTemplateSetting @TargetTypeId = @FeedbackTargetTypeId

	INSERT INTO dbo.Feedback (
		EmployeeId
		,EvaluatorId
		,PeriodId
		,TargetTemplateId
		,FromDate
		,ToDate
		)
	OUTPUT INSERTED.FeedbackId
		,INSERTED.EmployeeId
		,INSERTED.EvaluatorId
		,INSERTED.TargetTemplateId
	INTO #tblInsertedFeedback
	SELECT DISTINCT tblfb.EmployeeId
		,tblfb.EvaluatorId
		,@PeriodId
		,emptt.TargetTemplateId
		,tblfb.FromDate
		,tblfb.ToDate
	FROM dbo.TargetTemplateEmployee AS emptt WITH (NOLOCK)
		INNER JOIN dbo.TargetTemplate AS tt WITH (NOLOCK) ON emptt.TargetTemplateId = tt.TargetTemplateId
		AND tt.TargetTypeId = @FeedbackTargetTypeId
		INNER JOIN #tblFeedback AS tblfb WITH (NOLOCK) ON emptt.EmployeeId = tblfb.EmployeeId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.Feedback AS fb WITH (NOLOCK)
			WHERE tblfb.EmployeeId = fb.EmployeeId
				AND tblfb.EvaluatorId = fb.EvaluatorId
				AND fb.PeriodId = @PeriodId
			)

	INSERT INTO dbo.Feedback (
		EmployeeId
		,EvaluatorId
		,PeriodId
		,TargetTemplateId
		,FromDate
		,ToDate
		)
	OUTPUT INSERTED.FeedbackId
		,INSERTED.EmployeeId
		,INSERTED.EvaluatorId
		,INSERTED.TargetTemplateId
	INTO #tblInsertedFeedback
	SELECT DISTINCT tblfb.EmployeeId
		,tblfb.EvaluatorId
		,@PeriodId
		,depttt.TargetTemplateId
		,tblfb.FromDate
		,tblfb.ToDate
	FROM dbo.TargetTemplateDepartment AS depttt WITH (NOLOCK)
		INNER JOIN dbo.TargetTemplate AS tt WITH (NOLOCK) ON depttt.TargetTemplateId = tt.TargetTemplateId
		AND tt.TargetTypeId = @FeedbackTargetTypeId
		INNER JOIN dbo.[Group] AS gr ON depttt.DepartmentId = gr.GroupId
		AND depttt.IsGroup = 1
		INNER JOIN dbo.Employee AS emp ON emp.GroupId = gr.GroupId
		INNER JOIN #tblFeedback AS tblfb WITH (NOLOCK) ON emp.EmployeeId = tblfb.EmployeeId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.Feedback AS fb WITH (NOLOCK)
			WHERE tblfb.EmployeeId = fb.EmployeeId
				AND tblfb.EvaluatorId = fb.EvaluatorId
				AND fb.PeriodId = @PeriodId
			)

	INSERT INTO dbo.Feedback (
		EmployeeId
		,EvaluatorId
		,PeriodId
		,TargetTemplateId
		,FromDate
		,ToDate
		)
	OUTPUT INSERTED.FeedbackId
		,INSERTED.EmployeeId
		,INSERTED.EvaluatorId
		,INSERTED.TargetTemplateId
	INTO #tblInsertedFeedback
	SELECT DISTINCT tblfb.EmployeeId
		,tblfb.EvaluatorId
		,@PeriodId
		,depttt.TargetTemplateId
		,tblfb.FromDate
		,tblfb.ToDate
	FROM dbo.TargetTemplateDepartment AS depttt WITH (NOLOCK)
		INNER JOIN #tblDeptFlowAndTargetTemplate AS depttt1 ON depttt.DepartmentId = depttt1.DepartmentId
		AND depttt.TargetTemplateId = depttt1.TargetTemplateId
		AND depttt.IsGroup = 0
		INNER JOIN #tblFeedback AS tblfb WITH (NOLOCK) ON depttt1.EmployeeId = tblfb.EmployeeId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.Feedback AS fb WITH (NOLOCK)
			WHERE tblfb.EmployeeId = fb.EmployeeId
				AND tblfb.EvaluatorId = fb.EvaluatorId
				AND fb.PeriodId = @PeriodId
			)

	INSERT INTO dbo.FeedbackKPIGroup (
		FeedbackId
		,KPIGroupId
		,LogNo
		,IsSpreadKPI
		,[Weight]
		,DisplayOrder
		)
	SELECT fb.FeedbackId
		,gr.KPIGroupId
		,gr.LogNo
		,ttgr.IsSpreadKPI
		,ttgr.[Weight]
		,ttgr.DisplayOrder
	FROM #tblInsertedFeedback AS fb WITH (NOLOCK)
		INNER JOIN dbo.TargetTemplateKPIGroup AS ttgr WITH (NOLOCK) ON fb.TargetTemplateId = ttgr.TargetTemplateId
		INNER JOIN dbo.KPIGroup AS gr WITH (NOLOCK) ON ttgr.KPIGroupId = gr.KPIGroupId

	INSERT INTO dbo.FeedbackKPI (
		FeedbackId
		,KPIGroupId
		,KPIId
		,LogNo
		,HasComment
		,IsRequiredComment
		,IsRequiredEvaluate
		,[Weight]
		,DisplayOrder
		)
	SELECT fb.FeedbackId
		,ttkpi.KPIGroupId
		,kpi.KPIId
		,kpi.LogNo
		,CASE 
			WHEN ttkpi.HasComment IS NOT NULL THEN ttkpi.HasComment
			ELSE kpi.HasComment
			END
		,CASE 
			WHEN ttkpi.IsRequiredComment IS NOT NULL THEN ttkpi.IsRequiredComment
			ELSE kpi.IsRequiredComment
			END
		,CASE 
			WHEN ttkpi.IsRequiredEvaluate IS NOT NULL THEN ttkpi.IsRequiredEvaluate
			ELSE kpi.IsRequiredEvaluate
			END
		,ttkpi.[Weight]
		,ttkpi.DisplayOrder
	FROM #tblInsertedFeedback AS fb WITH (NOLOCK)
		INNER JOIN dbo.TargetTemplateKPI AS ttkpi WITH (NOLOCK) ON fb.TargetTemplateId = ttkpi.TargetTemplateId
		INNER JOIN dbo.KPI AS kpi WITH (NOLOCK) ON ttkpi.KPIId = kpi.KPIId

	DELETE fb
	FROM dbo.Feedback AS fb
	WHERE EXISTS (
			SELECT 1
			FROM #tblDeletedFeedback AS tbl WITH (NOLOCK)
			WHERE fb.FeedbackId = tbl.FeedbackId
			)

	DELETE fb
	FROM dbo.FeedbackKPIGroup AS fb
	WHERE EXISTS (
			SELECT 1
			FROM #tblDeletedFeedback AS tbl WITH (NOLOCK)
			WHERE fb.FeedbackId = tbl.FeedbackId
			)

	DELETE fb
	FROM dbo.FeedbackKPI AS fb
	WHERE EXISTS (
			SELECT 1
			FROM #tblDeletedFeedback AS tbl WITH (NOLOCK)
			WHERE fb.FeedbackId = tbl.FeedbackId
			)

	DROP TABLE #tblFeedback
	DROP TABLE #tblInsertedFeedback
	DROP TABLE #tblDeletedFeedback
	DROP TABLE #tblDeptFlowAndTargetTemplate
END
