
CREATE PROCEDURE [dbo].[EPA_UpdateStatus] @EPAId INT
	,@ProcessId TINYINT
	,@EmployeeId INT
	,@EvaluatorId INT
	,@Status INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150819@Tim.Trinh
Task: Update status of EPA.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
				INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			WHERE epa.EPAId = @EPAId
				AND pc.ProcessId = @ProcessId
				AND pc.EvaluatorId = @EvaluatorId
				AND epa.EmployeeId = @EmployeeId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'EPA does not exist.'

		RETURN
	END

	IF @Status = 0
		AND EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
				INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			WHERE epa.EPAId = @EPAId
				AND pc.ProcessId = @ProcessId
				AND pc.EvaluatorId = @EvaluatorId
				AND epa.EmployeeId = @EmployeeId
				AND pc.[IsEndProcess] = 1
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'EPA has closed. You can not open this EPA.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE pc
		SET [Status] = @Status
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		FROM dbo.EPAProcess AS pc WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON pc.EPAId = epa.EPAId
		WHERE epa.EPAId = @EPAId
			AND pc.ProcessId = @ProcessId
			AND epa.EmployeeId = @EmployeeId
			AND pc.EvaluatorId = @EvaluatorId

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Evaluate unsuccessfully.'
	END CATCH;
END