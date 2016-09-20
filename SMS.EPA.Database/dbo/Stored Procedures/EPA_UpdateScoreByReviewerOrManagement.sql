
CREATE PROCEDURE [dbo].[EPA_UpdateScoreByReviewerOrManagement] @EPAId INT
	,@ProcessId TINYINT
	,@EmployeeId INT
	,@EvaluatorId INT
	,@Comment NVARCHAR(MAX) = ''
	,@Score NVARCHAR(50) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150828@Tim.Trinh
Task: Set KPI score.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ModifiedDate DATETIME
		,@Evaluators IntId

	INSERT INTO @Evaluators (Id)
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
					SELECT Id
					FROM @Evaluators
					)
				AND pc.ProcessId = @ProcessId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'You are not authorized to evaluate.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
				INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			WHERE epa.EPAId = @EPAId
				AND epa.EmployeeId = @EmployeeId
				AND pc.EvaluatorId IN (
					SELECT Id
					FROM @Evaluators
					)
				AND epa.[Status] = 1
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'This EPA is closed. You can not update.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION

		SET @ModifiedDate = GETDATE()

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

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Evaluate unsuccessfully.'
	END CATCH;
END