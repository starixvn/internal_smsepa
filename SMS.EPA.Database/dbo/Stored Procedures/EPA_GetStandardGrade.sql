CREATE PROCEDURE [dbo].[EPA_GetStandardGrade]
AS
/*
Created: 20151002@Tim.Trinh
Task: Get list standard grade.
DB: SPM
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT GradeId
		,[Type]
		,[Name]
		,RTRIM(Code) AS Code
		,LeftScore
		,RightScore
	FROM dbo.StandardGrade WITH (NOLOCK)
END