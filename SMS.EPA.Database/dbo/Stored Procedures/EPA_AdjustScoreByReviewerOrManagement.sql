
CREATE PROCEDURE [dbo].[EPA_AdjustScoreByReviewerOrManagement] @EPADetails NVARCHAR(MAX) = ''
	,@RecommendationXml NVARCHAR(MAX) = ''
	,@ProcessId TINYINT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150828@Tim.Trinh
Task: adjust score.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
• @EPADetails: N'
     <ROOT>
        <Row EPAId='' ProcessId='' EmployeeId='' EvaluatorId='' AdjustScore='dsd' TotalScore='xyz' Comment='' />
     </ROOT>

• @RecommendationXml: N'
     <ROOT>
        <Row EPAId='' ProbationStatus='' Promoted='' PayIncreased='' JobLevelIncreased='' JobLevelId='' />
     </ROOT>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE()
		,@Today DATE = GETDATE()
		,@DocHandler INT
		,@NumberAdjustEmployee INT
		,@Data NVARCHAR(MAX)
		-- process
		,@ReviewerProcessId TINYINT = 4
		,@ManagementProcessId TINYINT = 5

	CREATE TABLE #tblDetails (
		EPAId INT
		,ProcessId INT
		,EmployeeId INT
		,EvaluatorId INT
		,AdjustScore NVARCHAR(100)
		,TotalScore NVARCHAR(100) NULL
		,Comment NVARCHAR(MAX) NULL
		)

	CREATE TABLE #tblRecommendations (
		EPAId INT
		,ProbationStatus TINYINT
		,Promoted BIT
		,PayIncreased BIT
		,JobLevelIncreased BIT
		,JobLevelId INT
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@EPADetails

		INSERT INTO #tblDetails (
			EPAId
			,ProcessId
			,EmployeeId
			,EvaluatorId
			,AdjustScore
			,TotalScore
			,Comment
			)
		SELECT EPAId
			,ProcessId
			,EmployeeId
			,EvaluatorId
			,AdjustScore
			,TotalScore
			,Comment
		FROM OPENXML(@DocHandler, '/ROOT/Row', 1) WITH (EPAId INT '@EPAId', ProcessId INT '@ProcessId', EmployeeId INT '@EmployeeId', EvaluatorId INT '@EvaluatorId', AdjustScore NVARCHAR(100) '@AdjustScore', TotalScore NVARCHAR(100) '@TotalScore', Comment NVARCHAR(MAX) '@Comment') AS Data

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@RecommendationXml

		INSERT INTO #tblRecommendations (
			EPAId
			,ProbationStatus
			,Promoted
			,PayIncreased
			,JobLevelIncreased
			,JobLevelId
			)
		SELECT EPAId
			,ProbationStatus
			,Promoted
			,PayIncreased
			,JobLevelIncreased
			,JobLevelId
		FROM OPENXML(@DocHandler, '/ROOT/Row', 1) WITH (EPAId INT, ProbationStatus TINYINT, Promoted BIT, PayIncreased BIT, JobLevelIncreased BIT, JobLevelId INT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Parameter invalid.'

		RETURN
	END CATCH

	IF EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
				INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
				INNER JOIN #tblDetails AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
				AND pc.ProcessId = dt.ProcessId
				AND pc.EvaluatorId = dt.EvaluatorId
				AND @Today NOT BETWEEN pc.StartDate
					AND pc.EndDate
				AND pc.EndDate > @Today
			)
	BEGIN
		DELETE dt
		FROM #tblDetails AS dt
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON dt.EPAId = epa.EPAId
			AND dt.EmployeeId = epa.EmployeeId
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			AND dt.EvaluatorId = pc.EvaluatorId
			AND dt.ProcessId = pc.ProcessId
			AND @Today NOT BETWEEN pc.StartDate
				AND pc.EndDate
			AND pc.EndDate > @Today

		SELECT @NumberAdjustEmployee = COUNT(EPAId)
		FROM #tblDetails WITH (NOLOCK)

		IF @NumberAdjustEmployee = 0
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = N'Adjust EPA of employees unsuccessfully. The employees already end process.'

			RETURN
		END
		ELSE
		BEGIN
			SELECT @ErrCode = 0
				,@ErrMsg = N'Only adjust EPA of ' + CAST(@NumberAdjustEmployee AS NVARCHAR) + ' employees successfully. The other employees already end process.'
		END
	END

	UPDATE pc
	SET pc.AdjustScore = dt.AdjustScore
		,pc.TotalScore = dt.TotalScore
		,pc.Comment = dt.Comment
		,pc.ModifiedBy = @ModifiedBy
		,pc.ModifiedDate = @ModifiedDate
		,pc.EvaluateDate = @ModifiedDate
	FROM dbo.EPAProcess AS pc WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON pc.EPAId = epa.EPAId
		INNER JOIN #tblDetails AS dt WITH (NOLOCK) ON pc.EPAId = dt.EPAId
		AND pc.ProcessId = dt.ProcessId
		AND pc.EvaluatorId = dt.EvaluatorId

	IF @ProcessId = @ManagementProcessId
	BEGIN
		UPDATE pc
		SET pc.AdjustScore = dt.AdjustScore
			,pc.TotalScore = dt.TotalScore
			,pc.ModifiedBy = @ModifiedBy
			,pc.ModifiedDate = @ModifiedDate
			,pc.EvaluateDate = @ModifiedDate
		FROM dbo.EPAProcess AS pc WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON pc.EPAId = epa.EPAId
			INNER JOIN #tblDetails AS dt WITH (NOLOCK) ON pc.EPAId = dt.EPAId
			AND pc.ProcessId = dt.ProcessId
			AND pc.EvaluatorId <> dt.EvaluatorId
	END

	IF @ProcessId = @ReviewerProcessId
	BEGIN
		UPDATE re
		SET re.ProbationStatus = dt.ProbationStatus
			,re.Promoted = dt.Promoted
			,re.PayIncreased = dt.PayIncreased
			,re.JobLevelIncreased = dt.JobLevelIncreased
			,re.JobLevelId = dt.JobLevelId
			,re.ModifiedBy = @ModifiedBy
			,re.ModifiedDate = @ModifiedDate
		FROM dbo.EPARecommendation AS re WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON re.EPAId = epa.EPAId
			INNER JOIN #tblRecommendations AS dt WITH (NOLOCK) ON re.EPAId = dt.EPAId
		WHERE re.ProcessId = @ReviewerProcessId
	END

	EXEC @Data = [dbo].[fncGetDataForWriteLog_EPA_AdjustScore] @EPADetails = @EPADetails

	EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
		,@SpName = 'EPA_AdjustScoreByReviewerOrManagement'
		,@ObjectId = NULL
		,@ObjectTypeId = 10
		,@Data = @Data
		,@OldData = NULL
		,@ObjectIds = NULL

	DROP TABLE #tblDetails

	DROP TABLE #tblRecommendations
END