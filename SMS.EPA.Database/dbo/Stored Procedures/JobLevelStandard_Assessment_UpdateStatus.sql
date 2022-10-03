
CREATE PROCEDURE [dbo].[JobLevelStandard_Assessment_UpdateStatus] @AssessmentIds VARCHAR(MAX)
	,@StatusId TINYINT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20220816@Tim.Trinh
Task: update assessment status
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	CREATE TABLE #tmpAssessmentId (AssessmentId INT)

	INSERT INTO #tmpAssessmentId (AssessmentId)
	SELECT val
	FROM dbo.fnc_strSplit(@AssessmentIds, ',')

	-- write log
	SET @xml = (
			SELECT a.AssessmentId AS '@AssessmentId'
				,a.PeriodId AS '@PeriodId'
				,a.EmployeeId AS '@EmployeeId'
				,a.StatusId AS '@StatusId'
				,a.ModifiedBy AS '@ModifiedBy'
				,a.ModifiedDate AS '@ModifiedDate'
			FROM dbo.JobLevelStandard_Assessment AS a WITH (NOLOCK)
				INNER JOIN #tmpAssessmentId AS tmp WITH (NOLOCK) ON a.AssessmentId = tmp.AssessmentId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE a
	SET a.StatusId = @StatusId
		,a.ModifiedBy = @ModifiedBy
		,a.ModifiedDate = GETDATE()
	FROM dbo.JobLevelStandard_Assessment AS a WITH (ROWLOCK, UPDLOCK)
	INNER JOIN #tmpAssessmentId AS tmp WITH (NOLOCK) ON a.AssessmentId = tmp.AssessmentId

	-- write log
	SET @xml = (
			SELECT a.AssessmentId AS '@AssessmentId'
				,a.PeriodId AS '@PeriodId'
				,a.EmployeeId AS '@EmployeeId'
				,a.StatusId AS '@StatusId'
				,a.ModifiedBy AS '@ModifiedBy'
				,a.ModifiedDate AS '@ModifiedDate'
			FROM dbo.JobLevelStandard_Assessment AS a WITH (NOLOCK)
				INNER JOIN #tmpAssessmentId AS tmp WITH (NOLOCK) ON a.AssessmentId = tmp.AssessmentId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SpName = 'JobLevelStandard_Assessment_UpdateStatus'
		,@ObjectId = NULL
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@ObjectIds = @AssessmentIds
END