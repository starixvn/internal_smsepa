

CREATE PROCEDURE [dbo].[EPASummaryImport_Update] @EPADetailXml NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20151002@Quinn.Vo
Task: Update EPA summary which were encoded.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
 - @EPADetailXml: "<EPA Id='{0}' FinalScore='{1}'></EPA>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Xml XML
		,@DocHandler INT

	IF OBJECT_ID('tempdb..#EPASummaryImport') IS NOT NULL
	BEGIN
		DROP TABLE #EPASummaryImport
	END

	CREATE TABLE #EPASummaryImport (
		[Id] INT NOT NULL
		,[FinalScore] NVARCHAR(50) NULL
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@EPADetailXml

		INSERT INTO #EPASummaryImport (
			[Id]
			,[FinalScore]
			)
		SELECT [Id]
			,[FinalScore]
		FROM OPENXML(@DocHandler, '/ROOT/EPA', 1) WITH ([Id] INT, [FinalScore] NVARCHAR(50)) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	UPDATE epa WITH (ROWLOCK, UPDLOCK)
	SET FinalScore = tempEPA.FinalScore
	FROM dbo.EPASummaryImport AS epa
		INNER JOIN #EPASummaryImport AS tempEPA ON epa.Id = tempEPA.Id

	IF OBJECT_ID('tempdb..#EPASummaryImport') IS NOT NULL
	BEGIN
		DROP TABLE #EPASummaryImport
	END
END