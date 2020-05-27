
CREATE PROCEDURE [dbo].[Feedback_Update] @FeedbackId BIGINT
	,@FinalScore NVARCHAR(100) = ''
	,@Comment NVARCHAR(MAX) = ''
	,@DetailXml NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200527@Tim.Trinh
Task: update feedback
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
• @DetailXml=
    N'<ROOT>
	   <Row KPIId="" KPIGroupId="" Score="" Rate="" Comment=""></KPI>
    </ROOT>'
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE()
		,@DocHandler INT

	CREATE TABLE #tblKPI (
		KPIGroupId INT
		,KPIId INT
		,Score NVARCHAR(100)
		,Rate NVARCHAR(100)
		,Comment NVARCHAR(MAX)
		)

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@DetailXml

	INSERT INTO #tblKPI (
		KPIGroupId
		,KPIId
		,Score
		,Rate
		,Comment
		)
	SELECT KPIGroupId
		,KPIId
		,Score
		,Rate
		,Comment
	FROM OPENXML(@DocHandler, '/ROOT/Row', 1) WITH (KPIGroupId INT, KPIId INT, Score NVARCHAR(100), Rate NVARCHAR(100), Comment NVARCHAR(MAX)) AS Data

	EXEC sp_xml_removedocument @DocHandler

	UPDATE fb
	SET fb.Score = kpi.Score
		,fb.Rate = kpi.Rate
		,fb.Comment = kpi.Comment
	FROM dbo.FeedbackKPI AS fb WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblKPI AS kpi WITH (NOLOCK) ON fb.KPIId = kpi.KPIId
		AND fb.KPIGroupId = kpi.KPIGroupId
	WHERE fb.FeedbackId = @FeedbackId

	UPDATE dbo.Feedback WITH (ROWLOCK, UPDLOCK)
	SET FinalScore = @FinalScore
		,Comment = @Comment
		,EvaluatedDate = @ModifiedDate
	WHERE FeedbackId = @FeedbackId

	DROP TABLE #tblKPI
END