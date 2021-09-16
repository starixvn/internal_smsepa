
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

	CREATE TABLE #tblEPA (
		EPAId INT
		,FinalScore NVARCHAR(100)
		,DisciplineScore FLOAT
		,Grade NVARCHAR(100)
		)

	CREATE TABLE #tblEPAScore (
		EPAId INT
		,FinalScore NVARCHAR(100)
		,DisciplineScore FLOAT
		,Grade NVARCHAR(100)
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@EPAScores

		INSERT INTO #tblEPAScore (
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

	INSERT INTO #tblEPA (
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
		INNER JOIN #tblEPAScore AS tbl WITH (NOLOCK) ON epa.EPAId = tbl.EPAId

	DECLARE @EPAIds NVARCHAR(MAX)
		,@Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	SELECT @EPAIds = COALESCE(@EPAIds + ',', '') + CAST(EPAId AS NVARCHAR(20))
	FROM #tblEPAScore WITH (NOLOCK)

	EXEC @OldData = dbo.fncGetDataForWriteLog_EPA_UpdateFinalScore @EPAIds

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
	FROM dbo.EPA AS epa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblEPAScore AS new WITH (NOLOCK) ON epa.EPAId = new.EPAId
		INNER JOIN #tblEPA AS old WITH (NOLOCK) ON epa.EPAId = old.EPAId

	EXEC dbo.EPA_UpdateJobLevelAndJobDescription @EPAIds

	EXEC @Data = dbo.fncGetDataForWriteLog_EPA_UpdateFinalScore @EPAIds

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SpName = 'EPA_UpdateFinalScoreEndEvaluation'
		,@ObjectId = NULL
		,@ObjectTypeId = 10
		,@Data = @Data
		,@OldData = @OldData
		,@ObjectIds = NULL

	DROP TABLE #tblEPA

	DROP TABLE #tblEPAScore
END