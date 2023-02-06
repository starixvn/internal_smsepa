

CREATE PROCEDURE [dbo].[JobLevelStandard_Period_UpdateJobLevel] @AssessmentId INT
	,@EmployeeId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 2023-01-05@Frankie.Nguyen
Task: update job level according user management
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @JobLevelId AS INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
	
	SELECT @JobLevelId = JobLevelId 
	FROM dbo.Employee WITH (NOLOCK)
	WHERE EmployeeId = @EmployeeId

	-- write log
	SET @xml = (
			SELECT AssessmentId AS '@AssessmentId'
				,CurrentJobLevelId AS '@CurrentJobLevelId'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.JobLevelStandard_Assessment WITH (NOLOCK)
			WHERE AssessmentId = @AssessmentId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	UPDATE JobLevelStandard_Assessment
	SET CurrentJobLevelId = @JobLevelId 
		,ModifiedDate = GETDATE()
		,ModifiedBy = @ModifiedBy
	WHERE AssessmentId = @AssessmentId 

	-- write log
	SET @xml = (
			SELECT AssessmentId AS '@AssessmentId'
				,CurrentJobLevelId AS '@CurrentJobLevelId'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.JobLevelStandard_Assessment WITH (NOLOCK)
			WHERE AssessmentId = @AssessmentId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SpName = 'JobLevelStandard_Period_UpdateJobLevel'
		,@ObjectId = @AssessmentId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@ObjectIds = NULL
END