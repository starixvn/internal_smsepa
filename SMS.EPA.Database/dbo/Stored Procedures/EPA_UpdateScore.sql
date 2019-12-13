
CREATE PROCEDURE [dbo].[EPA_UpdateScore] @EPAId INT
	,@ProcessId TINYINT
	,@EmployeeId INT
	,@EvaluatorId INT
	,@Comment NVARCHAR(MAX) = ''
	,@Score NVARCHAR(100) = ''
	,@KPIDetails NVARCHAR(MAX) = ''
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
	   <KPI KPIId="1" KPIGroupId="1" Score="3" Rate="5" IsMaxScore="0" Comment="Comment"></KPI>
    </ROOT>'
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE()
	DECLARE @tblEvaluator TABLE (EvaluatorId INT)
	DECLARE @tblKPIScores TABLE (
		KPIId INT
		,KPIGroupId INT
		,Score NVARCHAR(100)
		,Rate NVARCHAR(100)
		,IsMaxScore BIT
		,Comment NVARCHAR(MAX)
		)

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
			,@ErrMsg = N'You are not authorized.'

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
			,@ErrMsg = N'Your phase is closed. You can not update.'

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
			,@ErrMsg = N'This EPA is closed. You can not update.'

		RETURN
	END

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@KPIDetails

		INSERT INTO @tblKPIScores (
			KPIId
			,KPIGroupId
			,Score
			,Rate
			,IsMaxScore
			,Comment
			)
		SELECT KPIId
			,KPIGroupId
			,Score
			,Rate
			,IsMaxScore
			,Comment
		FROM OPENXML(@DocHandler, '/ROOT/KPI', 1) WITH (KPIId INT, KPIGroupId INT, Score NVARCHAR(100), Rate NVARCHAR(100), IsMaxScore BIT, Comment NVARCHAR(MAX)) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Parameter invalid.'

		RETURN
	END CATCH

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
