/****** Object:  Stored Procedure dbo.EPA_RatingByKPI_UpdateKPIScore    Script Date: 5/16/2016 2:20:11 PM ******/
/****** Object:  Stored Procedure dbo.EPA_RatingByKPI_UpdateKPIScore    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.EPA_RatingByKPI_UpdateKPIScore    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EPA_RatingByKPI_UpdateKPIScore    Script Date: 4/6/2016 11:27:42 AM ******/

CREATE PROCEDURE [dbo].[EPA_RatingByKPI_UpdateKPIScore] @EvaluatorId INT
	,@KPIDetails NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150910@Tim.Trinh
Task: Update KPI Score for a list employees.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
• @KPIDetails=N'
     <ROOT>
		 <EPA EPAId="1" EmployeeId="125" KPIGroupId="1" TotalScore="abc">
		     <EPADetail KPIId ="1" Rate="1" Score="xyz" IsMaxScore="0" Comment="Test" />
		  </EPA>
	</ROOT>'
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ModifiedDate DATETIME
	DECLARE @TblKPIDetails TABLE (
		EPAId INT
		,KPIGroupId INT
		,KPIId INT
		,EmployeeId INT
		,Rate NVARCHAR(100) NULL
		,Score NVARCHAR(100) NULL
		,IsMaxScore BIT
		,Comment NVARCHAR(MAX) NULL
		,TotalScore NVARCHAR(100) NULL
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@KPIDetails

		INSERT INTO @TblKPIDetails (
			EPAId
			,EmployeeId
			,TotalScore
			,KPIGroupId
			,KPIId
			,Rate
			,Score
			,IsMaxScore
			,Comment
			)
		SELECT EPAId
			,EmployeeId
			,TotalScore
			,KPIGroupId
			,KPIId
			,Rate
			,Score
			,IsMaxScore
			,Comment
		FROM OPENXML(@DocHandler, '/ROOT/EPA/EPADetail', 1) WITH (EPAId INT '../@EPAId', EmployeeId INT '../@EmployeeId', TotalScore NVARCHAR(100) '../@TotalScore', KPIGroupId INT '../@KPIGroupId', KPIId INT '@KPIId', Rate NVARCHAR(100) '@Rate', Score NVARCHAR(100) '@Score', IsMaxScore BIT '@IsMaxScore', Comment NVARCHAR(MAX) '@Comment') AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Employee Score unsuccessfully. Parameter invalid.'

		RETURN
	END CATCH

	BEGIN TRY
		BEGIN TRANSACTION

		SET @ModifiedDate = GETDATE()

		UPDATE dt
		SET dt.Rate = sdt.Rate
			,dt.Score = sdt.Score
			,dt.IsMaxScore = sdt.IsMaxScore
			,dt.Comment = sdt.Comment
			,dt.ModifiedBy = @EvaluatorId
			,dt.ModifiedDate = @ModifiedDate
		FROM dbo.EPADetail AS dt WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON dt.EPAId = epa.EPAId
			INNER JOIN @TblKPIDetails AS sdt ON epa.EPAId = sdt.EPAId
			AND epa.EmployeeId = sdt.EmployeeId
			AND dt.ProcessId = 3
			AND dt.EvaluatorId = @EvaluatorId
			AND dt.KPIGroupId = sdt.KPIGroupId
			AND dt.KPIId = sdt.KPIId

		UPDATE pc
		SET pc.TotalScore = sdt.TotalScore
			,pc.ModifiedBy = @EvaluatorId
			,pc.ModifiedDate = @ModifiedDate
			,pc.EvaluateDate = @ModifiedDate
		FROM dbo.EPAProcess AS pc WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON pc.EPAId = epa.EPAId
			INNER JOIN @TblKPIDetails AS sdt ON epa.EPAId = sdt.EPAId
			AND epa.EmployeeId = sdt.EmployeeId
			AND pc.ProcessId = 3
			AND pc.EvaluatorId = @EvaluatorId

		COMMIT TRANSACTION

		DECLARE @Data NVARCHAR(MAX)

		EXEC @Data = [dbo].[fncGetDataForWriteLog_EPA_UpdateScoreByKPI] @EvaluatorId = @EvaluatorId, @KPIDetails = @KPIDetails

		EXEC [dbo].[LogDetail_Insert] @ActorId = @EvaluatorId, @SpName = 'EPA_RatingByKPI_UpdateKPIScore', @ObjectId = NULL, @ObjectTypeId = 10, @Data = @Data, @OldData = NULL, @ObjectIds = NULL

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Employee Score unsuccessfully.'

		RETURN
	END CATCH
END