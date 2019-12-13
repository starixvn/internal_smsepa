
CREATE PROCEDURE [dbo].[EPA_UpdateFinalScoreEndEvaluation] @ModifiedBy INT
	,@EPAScores VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160523@Tim.Trinh
Task: Update final score.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
	@EPAScores = '<ROOT>
					<EPA EPAId='' FinalScore='' DisciplineScore='' Grade=''></EPA>
				</ROOT>'
*/
BEGIN
	SET NOCOUNT ON

	CREATE TABLE #EPA (
		EPAId INT
		,FinalScore NVARCHAR(100)
		,DisciplineScore FLOAT
		,Grade NVARCHAR(100)
		)

	CREATE TABLE #EPAScore (
		EPAId INT
		,FinalScore NVARCHAR(100)
		,DisciplineScore FLOAT
		,Grade NVARCHAR(100)
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@EPAScores

		INSERT INTO #EPAScore (
			EPAId
			,FinalScore
			,DisciplineScore
			,Grade
			)
		SELECT EPAId
			,FinalScore
			,DisciplineScore
			,Grade
		FROM OPENXML(@DocHandler, '/ROOT/EPA', 1) WITH (EPAId INT, FinalScore NVARCHAR(100), DisciplineScore FLOAT, Grade NVARCHAR(100)) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Parameter invalid.'

		RETURN
	END CATCH

	INSERT INTO #EPA (
		EPAId
		,FinalScore
		,DisciplineScore
		,Grade
		)
	SELECT epa.EPAId
		,epa.FinalScore
		,epa.DisciplineScore
		,epa.Grade
	FROM dbo.EPA AS epa
		INNER JOIN #EPAScore AS tbl ON epa.EPAId = tbl.EPAId

	DECLARE @EPAIds NVARCHAR(MAX)
		,@Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	SELECT @EPAIds = COALESCE(@EPAIds + ',', '') + CAST(EPAId AS NVARCHAR(20))
	FROM #EPAScore

	EXEC @OldData = dbo.fncGetDataForWriteLog_EPA_UpdateFinalScore @EPAIds

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE epa
		SET FinalScore = new.FinalScore
			,DisciplineScore = new.DisciplineScore
			,Grade = new.Grade
			,OldFinalScore = old.FinalScore
			,OldDisciplineScore = old.DisciplineScore
			,OldGrade = old.Grade
			,[Status] = 1 -- closed
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN #EPAScore AS new ON epa.EPAId = new.EPAId
			INNER JOIN #EPA AS old ON epa.EPAId = old.EPAId

		EXEC @Data = dbo.fncGetDataForWriteLog_EPA_UpdateFinalScore @EPAIds
		
		EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
			,@SpName = 'EPA_UpdateFinalScoreEndEvaluation'
			,@ObjectId = NULL
			,@ObjectTypeId = 10
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update final score unsuccessfully.'
	END CATCH

	DROP TABLE #EPA

	DROP TABLE #EPAScore
END