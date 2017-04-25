CREATE PROCEDURE [dbo].[JobGrade_Get] @IsActive BIT = NULL
AS
/*
Created: 20161010@Tim.Trinh
Task: Get job grade.
DB: SMSEPA
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	SELECT JobGradeId
		,[Name]
	FROM dbo.JobGrade WITH (NOLOCK)
	WHERE @IsActive IS NULL
		OR IsActive = @IsActive
END