/****** Object:  Stored Procedure dbo.EPA_AdjustScoreByReviewerOrManagement    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.EPA_AdjustScoreByReviewerOrManagement    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.EPA_AdjustScoreByReviewerOrManagement    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EPA_AdjustScoreByReviewerOrManagement    Script Date: 4/6/2016 11:27:40 AM ******/

CREATE PROCEDURE [dbo].[EPA_AdjustScoreByReviewerOrManagement] @EPADetails NVARCHAR(MAX) = ''
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
• @EPADetails: N'
     <ROOT>
        <EPA EPAId='1' ProcessId='3' EmployeeId='1' EvaluatorId='1' AdjustScore='dsd' TotalScore='xyz' Comment='' />
     </ROOT>

• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ModifiedDate DATETIME

	CREATE TABLE #Details (
		EPAId INT
		,ProcessId INT
		,EmployeeId INT
		,EvaluatorId INT
		,AdjustScore NVARCHAR(100)
		,TotalScore NVARCHAR(100) NULL
		,Comment NVARCHAR(MAX) NULL
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@EPADetails

		INSERT INTO #Details
		SELECT EPAId
			,ProcessId
			,EmployeeId
			,EvaluatorId
			,AdjustScore
			,TotalScore
			,Comment
		FROM OPENXML(@DocHandler, '/ROOT/EPA', 1) WITH (EPAId INT '@EPAId', ProcessId INT '@ProcessId', EmployeeId INT '@EmployeeId', EvaluatorId INT '@EvaluatorId', AdjustScore NVARCHAR(100) '@AdjustScore', TotalScore NVARCHAR(100) '@TotalScore', Comment NVARCHAR(MAX) '@Comment') AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Employee Score unsuccessfully. Parameter invalid.'

		RETURN
	END CATCH

	IF EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
				INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
				INNER JOIN #Details AS dt ON epa.EPAId = dt.EPAId
				AND pc.ProcessId = dt.ProcessId
				AND pc.EvaluatorId = dt.EvaluatorId
				AND pc.IsEndProcess = 1
			)
	BEGIN
		DELETE
		FROM #Details
		WHERE EXISTS (
				SELECT 1
				FROM dbo.EPAProcess AS pc WITH (NOLOCK)
				WHERE pc.[EPAId] = #Details.[EPAId]
					AND pc.ProcessId = #Details.ProcessId
					AND pc.EvaluatorId = #Details.EvaluatorId
					AND pc.IsEndProcess = 1
				)

		DECLARE @NumberAdjustEmployee INT

		SET @NumberAdjustEmployee = (
				SELECT COUNT([EPAId])
				FROM #Details
				)

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

	BEGIN TRY
		BEGIN TRANSACTION

		SET @ModifiedDate = GETDATE()

		UPDATE pc
		SET pc.AdjustScore = dt.AdjustScore
			,pc.TotalScore = dt.TotalScore
			,pc.Comment = dt.Comment
			,pc.ModifiedBy = @ModifiedBy
			,pc.ModifiedDate = @ModifiedDate
			,pc.EvaluateDate = @ModifiedDate
		FROM dbo.EPAProcess AS pc WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON pc.EPAId = epa.EPAId
			INNER JOIN #Details AS dt ON pc.EPAId = dt.EPAId
			AND pc.ProcessId = dt.ProcessId
			AND pc.EvaluatorId = dt.EvaluatorId

		COMMIT TRANSACTION

		DECLARE @Data NVARCHAR(MAX)

		EXEC @Data = [dbo].[fncGetDataForWriteLog_EPA_AdjustScore] @EPADetails = @EPADetails

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'EPA_AdjustScoreByReviewerOrManagement'
			,@ObjectId = NULL
			,@ObjectTypeId = 10
			,@Data = @Data
			,@OldData = NULL
			,@ObjectIds = NULL
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Evaluate unsuccessfully.'
	END CATCH;
END