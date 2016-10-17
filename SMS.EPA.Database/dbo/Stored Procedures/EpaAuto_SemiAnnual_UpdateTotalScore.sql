
CREATE PROCEDURE [dbo].[EpaAuto_SemiAnnual_UpdateTotalScore] @EPAId INT
	,@Details NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20160308@Tim.Trinh
Task: Auto update total score for Semi has Monthly
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
     @Details: N'
     <ROOT>
        <EPA ProcessId='' TotalScore='' />
     </ROOT>
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #Details (
		EPAId INT
		,ProcessId INT
		,TotalScore NVARCHAR(100) NULL
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@Details

		INSERT INTO #Details
		SELECT @EPAId
			,ProcessId
			,TotalScore
		FROM OPENXML(@DocHandler, '/ROOT/EPA', 1) WITH (ProcessId INT '@ProcessId', TotalScore NVARCHAR(100) '@TotalScore') AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Parameter invalid.'

		RETURN
	END CATCH

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE pc
		SET pc.TotalScore = dt.TotalScore
		FROM dbo.EPAProcess AS pc WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON pc.EPAId = epa.EPAId
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			INNER JOIN #Details AS dt ON pc.EPAId = dt.EPAId
			AND pc.ProcessId = dt.ProcessId
		WHERE epa.EPAId = @EPAId
			AND epa.HasMonthlyEPA = 1
			AND period.TargetTypeId = 3 -- semi

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully.'
	END CATCH;
END