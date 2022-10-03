
CREATE PROCEDURE [dbo].[JobLevelStandard_PeriodEmployeeSetting_UpdateStatus] @PeriodId INT
	,@EmployeeId INT
	,@IsExcluded BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20220816@Tim.Trinh
Task: update status period employee setting.
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

	-- write log
	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,IsExcluded AS '@IsExcluded'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.JobLevelStandard_PeriodEmployeeSetting WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
				AND EmployeeId = @EmployeeId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE dbo.JobLevelStandard_PeriodEmployeeSetting WITH (ROWLOCK, UPDLOCK)
	SET IsExcluded = @IsExcluded
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE PeriodId = @PeriodId
		AND EmployeeId = @EmployeeId

	-- write log
	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,IsExcluded AS '@IsExcluded'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.JobLevelStandard_PeriodEmployeeSetting WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
				AND EmployeeId = @EmployeeId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SpName = 'JobLevelStandard_PeriodEmployeeSetting_UpdateStatus'
		,@ObjectId = @EmployeeId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@ObjectIds = NULL
END