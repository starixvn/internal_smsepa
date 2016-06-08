/****** Object:  Stored Procedure dbo.EPA_ResetScore    Script Date: 5/16/2016 2:20:11 PM ******/
/****** Object:  Stored Procedure dbo.EPA_ResetScore    Script Date: 5/16/2016 2:15:33 PM ******/
/****** Object:  Stored Procedure dbo.EPA_ResetScore    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EPA_ResetScore    Script Date: 4/6/2016 11:27:42 AM ******/

CREATE PROCEDURE [dbo].[EPA_ResetScore] @EPAId INT
	,@Details NVARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20160311@Tim.Trinh
Task: reset score
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
     @Details: N'
     <ROOT>
        <EPA ProcessId='' TotalScore='' AdjustScore='' />
     </ROOT>
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #Details (
		EPAId INT
		,ProcessId INT
		,TotalScore NVARCHAR(100) NULL
		,AdjustScore NVARCHAR(100) NULL
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@Details

		INSERT INTO #Details
		SELECT @EPAId
			,ProcessId
			,TotalScore
			,AdjustScore
		FROM OPENXML(@DocHandler, '/ROOT/EPA', 1) WITH (ProcessId INT '@ProcessId', TotalScore NVARCHAR(100) '@TotalScore', AdjustScore NVARCHAR(100) '@AdjustScore') AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Reset Score unsuccessfully. Parameter invalid.'

		RETURN
	END CATCH

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE dbo.EPA WITH (ROWLOCK, UPDLOCK)
		SET [Status] = 0
			,FinalScore = NULL
			,Grade = NULL
		WHERE EPAId = @EPAId

		UPDATE pc WITH (ROWLOCK, UPDLOCK)
		SET pc.TotalScore = dt.TotalScore
			,pc.AdjustScore = dt.AdjustScore
			,pc.Comment = (CASE 
				WHEN pc.ProcessId IN (4, 5) THEN NULL
				ELSE pc.Comment END
				)
			,pc.IsEndProcess = (CASE 
				WHEN pc.ProcessId IN (4, 5) THEN 0
				ELSE pc.IsEndProcess END
				)
			,pc.EvaluateDate = (CASE 
				WHEN pc.ProcessId IN (4, 5) THEN NULL
				ELSE pc.EvaluateDate END
				)
			,pc.ModifiedBy = @ModifiedBy
			,pc.ModifiedDate = GETDATE()
		FROM dbo.EPAProcess AS pc WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON pc.EPAId = epa.EPAId
			INNER JOIN #Details AS dt ON pc.EPAId = dt.EPAId
			AND pc.ProcessId = dt.ProcessId
		WHERE pc.EPAId = @EPAId

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Reset Score unsuccessfully.'
	END CATCH;
END