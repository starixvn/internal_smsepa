
CREATE PROCEDURE [dbo].[JobLevelStandard_Assessment_Update] @AssessmentId INT OUTPUT
	,@PeriodId INT
	,@EmployeeId INT
	,@TotalWeight DECIMAL(5, 2) = NULL
	,@CurrentJobLevelId INT = NULL
	,@NewJobLevelId INT = NULL
	,@StatusId TINYINT = NULL
	,@Remark NVARCHAR(200) = NULL
	,@AssessmentKPIXml VARCHAR(MAX) = NULL
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20220816@Tim.Trinh
Task: update assessment
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
		,@modifiedDate DATETIME = GETDATE()
	DECLARE @DocHandler INT

	CREATE TABLE #tempAssessmentKPI (
		KPIId INT
		,[Level] SMALLINT NULL
		,[Weight] DECIMAL(5, 2) NULL
		)

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@AssessmentKPIXml

	INSERT INTO #tempAssessmentKPI (
		KPIId
		,[Level]
		,[Weight]
		)
	SELECT KPIId
		,CASE 
			WHEN ISNUMERIC([Level]) = 1 THEN [Level]
			ELSE NULL
			END
		,CASE 
			WHEN ISNUMERIC([Weight]) = 1 THEN [Weight]
			ELSE NULL
			END
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (KPIId INT, [Level] VARCHAR(MAX), [Weight] VARCHAR(MAX)) AS data

	EXEC sp_xml_removedocument @DocHandler

	IF EXISTS (
			SELECT 1
			FROM dbo.JobLevelStandard_Assessment WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
				AND EmployeeId = @EmployeeId
			)
	BEGIN
		SELECT TOP 1 @AssessmentId = AssessmentId
		FROM dbo.JobLevelStandard_Assessment WITH (NOLOCK)
		WHERE PeriodId = @PeriodId
			AND EmployeeId = @EmployeeId

		-- write log
		SET @xml = (
				SELECT PeriodId AS '@PeriodId'
					,EmployeeId AS '@EmployeeId'
					,TotalWeight AS '@TotalWeight'
					,CurrentJobLevelId AS '@CurrentJobLevelId'
					,NewJobLevelId AS '@NewJobLevelId'
					,StatusId AS '@StatusId'
					,Remark AS '@Remark'
					,ModifiedBy AS '@ModifiedBy'
					,ModifiedDate AS '@ModifiedDate'
				FROM dbo.JobLevelStandard_Assessment WITH (NOLOCK)
				WHERE AssessmentId = @AssessmentId
				FOR XML PATH('r')
					,TYPE
				)
		SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

		-- update
		UPDATE dbo.JobLevelStandard_Assessment WITH (ROWLOCK, UPDLOCK)
		SET TotalWeight = @TotalWeight
			,CurrentJobLevelId = @CurrentJobLevelId
			,NewJobLevelId = @NewJobLevelId
			,StatusId = @StatusId
			,Remark = @Remark
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = @modifiedDate
		WHERE AssessmentId = @AssessmentId

		UPDATE ak
		SET ak.[Level] = tmp.[Level]
			,ak.[Weight] = tmp.[Weight]
			,ak.ModifiedBy = @ModifiedBy
			,ak.ModifiedDate = @modifiedDate
		FROM dbo.JobLevelStandard_AssessmentKPI AS ak WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tempAssessmentKPI AS tmp WITH (NOLOCK) ON ak.KPIId = tmp.KPIId
		WHERE ak.AssessmentId = @AssessmentId

		-- write log
		SET @xml = (
				SELECT PeriodId AS '@PeriodId'
					,EmployeeId AS '@EmployeeId'
					,TotalWeight AS '@TotalWeight'
					,CurrentJobLevelId AS '@CurrentJobLevelId'
					,NewJobLevelId AS '@NewJobLevelId'
					,StatusId AS '@StatusId'
					,Remark AS '@Remark'
					,ModifiedBy AS '@ModifiedBy'
					,ModifiedDate AS '@ModifiedDate'
					,(
						SELECT KPIId AS '@KPIId'
							,[Name] AS '@Name'
							,[Level] AS '@Level'
							,[Weight] AS '@Weight'
							,DisplayOrder AS '@DisplayOrder'
						FROM dbo.JobLevelStandard_AssessmentKPI WITH (NOLOCK)
						WHERE AssessmentId = @AssessmentId
						FOR XML PATH('z')
							,TYPE
						)
				FROM dbo.JobLevelStandard_Assessment WITH (NOLOCK)
				WHERE AssessmentId = @AssessmentId
				FOR XML PATH('r')
					,TYPE
				)
		SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	END
	ELSE
	BEGIN
		INSERT INTO dbo.JobLevelStandard_Assessment (
			EmployeeId
			,PeriodId
			,TotalWeight
			,CurrentJobLevelId
			,NewJobLevelId
			,StatusId
			,Remark
			,CreatedBy
			,CreatedDate
			)
		VALUES (
			@EmployeeId
			,@PeriodId
			,@TotalWeight
			,@CurrentJobLevelId
			,@NewJobLevelId
			,@StatusId
			,@Remark
			,@ModifiedBy
			,@modifiedDate
			)

		SELECT @AssessmentId = SCOPE_IDENTITY()

		INSERT INTO dbo.JobLevelStandard_AssessmentKPI (
			AssessmentId
			,KPIId
			,[Name]
			,[Level]
			,[Weight]
			,DisplayOrder
			,CreatedBy
			,CreatedDate
			)
		SELECT @AssessmentId
			,kpi.KPIId
			,kpi.[Name]
			,tmp.[Level]
			,tmp.[Weight]
			,tkpi.DisplayOrder
			,@ModifiedBy
			,@modifiedDate
		FROM dbo.JobLevelStandard_PeriodEmployeeSetting AS pes WITH (NOLOCK)
			INNER JOIN dbo.JobLevelStandard_TemplateKPI AS tkpi WITH (NOLOCK) ON pes.TemplateId = tkpi.TemplateId
			INNER JOIN dbo.JobLevelStandard_KPI AS kpi WITH (NOLOCK) ON tkpi.KPIId = kpi.KPIId
			LEFT JOIN #tempAssessmentKPI AS tmp WITH (NOLOCK) ON kpi.KPIId = tmp.KPIId
		WHERE pes.PeriodId = @PeriodId
			AND pes.EmployeeId = @EmployeeId

		-- write log
		SET @xml = (
				SELECT PeriodId AS '@PeriodId'
					,EmployeeId AS '@EmployeeId'
					,TotalWeight AS '@TotalWeight'
					,CurrentJobLevelId AS '@CurrentJobLevelId'
					,NewJobLevelId AS '@NewJobLevelId'
					,StatusId AS '@StatusId'
					,Remark AS '@Remark'
					,CreatedBy AS '@CreatedBy'
					,CreatedDate AS '@CreatedDate'
					,(
						SELECT KPIId AS '@KPIId'
							,[Name] AS '@Name'
							,[Level] AS '@Level'
							,[Weight] AS '@Weight'
							,DisplayOrder AS '@DisplayOrder'
						FROM dbo.JobLevelStandard_AssessmentKPI WITH (NOLOCK)
						WHERE AssessmentId = @AssessmentId
						FOR XML PATH('z')
							,TYPE
						)
				FROM dbo.JobLevelStandard_Assessment WITH (NOLOCK)
				WHERE AssessmentId = @AssessmentId
				FOR XML PATH('r')
					,TYPE
				)
		SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	END

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SpName = 'JobLevelStandard_Assessment_Update'
		,@ObjectId = @AssessmentId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@ObjectIds = NULL

	DROP TABLE #tempAssessmentKPI
END