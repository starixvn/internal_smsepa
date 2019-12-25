CREATE PROCEDURE [dbo].[EPA_GetExtendedQuestion] @EPAId INT
AS
/*
Created: 20191223@Drake.Doi
Task: Get Probation Extended Question.
DB: SPM
Original: 
 
Param's Explanation:

*/
BEGIN
	DECLARE @QuestionRefName varchar(100) = 'ProbationExtendedQuestion',
		@SubQuestionRefName varchar(100) = 'ProbationSubQuestion'

	SELECT TOP(1) ref.Value
	FROM RefResource AS ref
	WHERE ref.RefName = @QuestionRefName

	SELECT
		@EPAId 
		,ref.ResourceId AS SubQuestionId
		,ref.Value  AS SubQuestionContent
		,extendedQuest.IsAgree
	FROM ProbationExtendedQuestion AS extendedQuest
		INNER JOIN RefResource AS ref WITH (NOLOCK) ON extendedQuest.SubQuestionId = ref.ResourceId
			AND ref.RefName = @SubQuestionRefName
	WHERE extendedQuest.EPAId = @EPAId
	ORDER BY ResourceId ASC
END
