
CREATE PROCEDURE [dbo].[EPA_UpdateProbationExtendedQuestion] @EPAId INT
	,@EmployeeId INT
	,@SubQuestions NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20191225@Drake.Doi
Task: Set probation extended question's answer.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
• @KPIDetails=
    N'<ROOT>
		<ExtendedQuestion SubQuestionId='1' IsAgree='0'></ExtendedQuestion>
    </ROOT>'
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE()
		,@Today DATE = GETDATE()
	DECLARE @tblSubQuestions TABLE (
		SubQuestionId TINYINT
		,IsAgree BIT
		)

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
			WHERE epa.EPAId = @EPAId
				AND epa.EmployeeId = @EmployeeId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'You are not authorized.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
				INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			WHERE epa.EPAId = @EPAId
				AND epa.EmployeeId = @EmployeeId
				AND pc.EndDate < @Today
				AND pc.[ProcessId] = 2 -- Self Evaluatation
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Your phase is closed. You can not update.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EPA AS epa WITH (NOLOCK)
				INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			WHERE epa.EPAId = @EPAId
				AND epa.EmployeeId = @EmployeeId
				AND epa.[Status] = 1
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'This EPA is closed. You can not update.'

		RETURN
	END

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@SubQuestions

		INSERT INTO @tblSubQuestions (
			SubQuestionId
			,IsAgree
			)
		SELECT SubQuestionId
			,IsAgree
		FROM OPENXML(@DocHandler, '/ROOT/KPI', 1) WITH (SubQuestionId INT, IsAgree TINYINT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Parameter invalid.'

		RETURN
	END CATCH

	BEGIN TRY
		BEGIN TRANSACTION

		-- update table ProbationExtendedQuestion
		UPDATE extendedQ
			SET extendedQ.IsAgree = data.IsAgree
			FROM dbo.ProbationExtendedQuestion AS extendedQ WITH (ROWLOCK, UPDLOCK)
				INNER JOIN @tblSubQuestions AS data ON data.SubQuestionId = extendedQ.SubQuestionId
			WHERE extendedQ.EPAId = @EPAId

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully.'
	END CATCH;
END