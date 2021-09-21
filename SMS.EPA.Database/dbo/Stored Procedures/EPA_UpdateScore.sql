

CREATE PROCEDURE [dbo].[EPA_UpdateScore] @EPAId INT
	,@ProcessId TINYINT
	,@EmployeeId INT
	,@EvaluatorId INT
	,@Comment NVARCHAR(MAX) = ''
	,@Score NVARCHAR(100) = ''
	,@KPIDetails NVARCHAR(MAX) = ''
	,@ExtendedQuestions NVARCHAR(MAX) = ''
	,@RecommendationXml NVARCHAR(MAX) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150813@Tim.Trinh
Task: Set KPI score.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
• @KPIDetails=
    N'<ROOT>
	   <Row KPIId="1" KPIGroupId="1" Score="3" Rate="5" IsMaxScore="0" Comment="Comment" />
    </ROOT>'
• @KPIDetails=
	N'<ROOT>
		<Row SubQuestionId='1' IsAgree='0' />
	</ROOT>'
• @RecommendationXml: N'
     <ROOT>
        <Row EPAId='' ProbationStatus='' Promoted='' PayIncreased='' JobLevelIncreased='' JobLevelId='' />
     </ROOT>

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE()
	DECLARE @tblEvaluator TABLE (EvaluatorId INT)
	DECLARE @tblKPIScores TABLE (KPIId INT, KPIGroupId INT, Score NVARCHAR(100), Rate NVARCHAR(100), IsMaxScore BIT, Comment NVARCHAR(MAX))
	DECLARE @tblExtendedQuestions TABLE (SubQuestionId TINYINT, IsAgree BIT)
	DECLARE @tblRecommendation TABLE (EPAId INT, ProbationStatus TINYINT, Promoted BIT, PayIncreased BIT, JobLevelIncreased BIT, JobLevelId INT)

	INSERT INTO @tblEvaluator (EvaluatorId)
	SELECT DISTINCT EvaluatorId
	FROM dbo.EPAProcess WITH (NOLOCK)
	WHERE EPAId = @EPAId
		AND ProcessId = @ProcessId

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
				INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			WHERE epa.EPAId = @EPAId
				AND epa.EmployeeId = @EmployeeId
				AND pc.EvaluatorId IN (
					SELECT EvaluatorId
					FROM @tblEvaluator
					)
				AND pc.ProcessId = @ProcessId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'You do not have permission.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
				INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			WHERE epa.EPAId = @EPAId
				AND epa.EmployeeId = @EmployeeId
				AND pc.EvaluatorId IN (
					SELECT EvaluatorId
					FROM @tblEvaluator
					)
				AND pc.IsEndProcess = 1
				AND pc.[ProcessId] = @ProcessId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Your phase is closed.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
				INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			WHERE epa.EPAId = @EPAId
				AND epa.EmployeeId = @EmployeeId
				AND pc.EvaluatorId IN (
					SELECT EvaluatorId
					FROM @tblEvaluator
					)
				AND epa.[Status] = 1
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'EPA is closed.'

		RETURN
	END

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@KPIDetails

		INSERT INTO @tblKPIScores (KPIId, KPIGroupId, Score, Rate, IsMaxScore, Comment)
		SELECT KPIId, KPIGroupId, Score, Rate, IsMaxScore, Comment
		FROM OPENXML(@DocHandler, '/ROOT/Row', 1) WITH (KPIId INT, KPIGroupId INT, Score NVARCHAR(100), Rate NVARCHAR(100), IsMaxScore BIT, Comment NVARCHAR(MAX)) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'KPIDetails param is invalid.'

		RETURN
	END CATCH

	IF ISNULL(@ExtendedQuestions, '') != ''
	BEGIN
		BEGIN TRY
			EXEC sp_xml_preparedocument @DocHandler OUTPUT
				,@ExtendedQuestions

			INSERT INTO @tblExtendedQuestions (SubQuestionId, IsAgree)
			SELECT SubQuestionId, IsAgree
			FROM OPENXML(@DocHandler, '/ROOT/Row', 1) WITH (SubQuestionId TINYINT, IsAgree BIT) AS Data

			EXEC sp_xml_removedocument @DocHandler
		END TRY

		BEGIN CATCH
			SELECT @ErrCode = 1
				,@ErrMsg = N'ExtendedQuestions param is invalid.'

			RETURN
		END CATCH
	END

	IF ISNULL(@RecommendationXml, '') != ''
	BEGIN
		BEGIN TRY
			EXEC sp_xml_preparedocument @DocHandler OUTPUT
				,@RecommendationXml

			INSERT INTO @tblRecommendation (EPAId, ProbationStatus, Promoted, PayIncreased, JobLevelIncreased, JobLevelId)
			SELECT EPAId, ProbationStatus, Promoted, PayIncreased, JobLevelIncreased, JobLevelId
			FROM OPENXML(@DocHandler, '/ROOT/Row', 1) WITH (EPAId INT, ProbationStatus TINYINT, Promoted BIT, PayIncreased BIT, JobLevelIncreased BIT, JobLevelId INT) AS Data

			EXEC sp_xml_removedocument @DocHandler
		END TRY

		BEGIN CATCH
			SELECT @ErrCode = 1
				,@ErrMsg = N'Recommendation param is invalid.'

			RETURN
		END CATCH
	END

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_EPA_UpdateScore] @EPAId = @EPAId
		,@ProcessId = @ProcessId
		,@EvaluatorId = @EvaluatorId

	BEGIN TRY
		BEGIN TRANSACTION

		-- update table EPADetail
		UPDATE dt
		SET dt.Score = k.Score
			,dt.Rate = k.Rate
			,dt.IsMaxScore = k.IsMaxScore
			,dt.Comment = k.Comment
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = @ModifiedDate
		FROM dbo.EPADetail AS dt WITH (ROWLOCK, UPDLOCK)
			INNER JOIN @tblKPIScores AS k ON dt.KPIId = k.KPIId
			AND dt.KPIGroupId = k.KPIGroupId
		WHERE dt.EPAId = @EPAId
			AND dt.ProcessId = @ProcessId
			AND dt.EvaluatorId = @EvaluatorId

		-- update table EPAProcess
		UPDATE dbo.EPAProcess WITH (ROWLOCK, UPDLOCK)
		SET TotalScore = @Score
			,Comment = @Comment
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = @ModifiedDate
			,EvaluateDate = @ModifiedDate
		WHERE EPAId = @EPAId
			AND ProcessId = @ProcessId
			AND EvaluatorId = @EvaluatorId

		-- update table EPA
		UPDATE dbo.EPA WITH (ROWLOCK, UPDLOCK)
		SET ModifiedBy = @ModifiedBy
			,ModifiedDate = @ModifiedDate
		WHERE EPAId = @EPAId

		-- update table ProbationExtendedQuestion
		UPDATE extendedQ
			SET extendedQ.IsAgree = data.IsAgree
			FROM dbo.ProbationExtendedQuestion AS extendedQ WITH (ROWLOCK, UPDLOCK)
				INNER JOIN @tblExtendedQuestions AS data ON data.SubQuestionId = extendedQ.SubQuestionId
			WHERE extendedQ.EPAId = @EPAId

		-- update recommendation
		IF @ProcessId = 3 -- evaluator
		BEGIN
			UPDATE re
			SET re.ProbationStatus = dt.ProbationStatus
				,re.ModifiedBy = CASE WHEN re.ProbationStatus != dt.ProbationStatus THEN @ModifiedBy ELSE re.ModifiedBy END
				,re.Promoted = dt.Promoted
				,re.PromotedBy = CASE WHEN re.Promoted != dt.Promoted THEN @ModifiedBy ELSE re.PromotedBy END
				,re.PayIncreased = dt.PayIncreased
				,re.PayIncreasedBy = CASE WHEN re.PayIncreased != dt.PayIncreased THEN @ModifiedBy ELSE re.PayIncreasedBy END
				,re.JobLevelIncreased = dt.JobLevelIncreased
				,re.JobLevelIncreasedBy = CASE WHEN re.JobLevelIncreased != dt.JobLevelIncreased OR re.JobLevelId != dt.JobLevelId THEN @ModifiedBy ELSE re.JobLevelIncreasedBy END
				,re.JobLevelId = dt.JobLevelId
				,re.ModifiedDate = @ModifiedDate
			FROM dbo.EPARecommendation AS re WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON re.EPAId = epa.EPAId
				INNER JOIN @tblRecommendation AS dt ON re.EPAId = dt.EPAId
			WHERE re.ProcessId = 3
		END

		EXEC @Data = [dbo].[fncGetDataForWriteLog_EPA_UpdateScore] @EPAId = @EPAId
			,@ProcessId = @ProcessId
			,@EvaluatorId = @EvaluatorId

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'EPA_UpdateScore'
			,@ObjectId = @EPAId
			,@ObjectTypeId = 10
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully.'
	END CATCH;
END