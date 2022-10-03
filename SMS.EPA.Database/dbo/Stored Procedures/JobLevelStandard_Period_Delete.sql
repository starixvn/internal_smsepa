
CREATE PROCEDURE [dbo].[JobLevelStandard_Period_Delete] @PeriodId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2022-08-11@Tim.Trinh
Task: delete period.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_Period AS p WITH (NOLOCK)
				INNER JOIN dbo.JobLevelStandard_Assessment AS a WITH (NOLOCK) ON p.PeriodId = a.PeriodId
			WHERE p.PeriodId = @PeriodId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete period unsuccessfully.'

		RETURN
	END

	DECLARE @logDataOld NVARCHAR(MAX)
		,@xml XML

	-- write log
	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,[Name] AS '@Name'
				,IsActive AS '@IsActive'
				,CreatedBy AS '@CreatedBy'
				,CreatedDate AS '@CreatedDate'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.JobLevelStandard_Period WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	DELETE
	FROM dbo.JobLevelStandard_Period WITH (ROWLOCK, UPDLOCK)
	WHERE PeriodId = @PeriodId

	DELETE
	FROM dbo.JobLevelStandard_PeriodSetting WITH (ROWLOCK, UPDLOCK)
	WHERE PeriodId = @PeriodId

	DELETE
	FROM dbo.JobLevelStandard_PeriodJobLevelSetting WITH (ROWLOCK, UPDLOCK)
	WHERE PeriodId = @PeriodId

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SpName = 'JobLevelStandard_Period_Delete'
		,@ObjectId = @PeriodId
		,@ObjectTypeId = NULL
		,@Data = NULL
		,@OldData = @logDataOld
		,@ObjectIds = NULL
END