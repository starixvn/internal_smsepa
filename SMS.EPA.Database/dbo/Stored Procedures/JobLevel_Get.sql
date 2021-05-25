CREATE PROCEDURE [dbo].[JobLevel_Get] @IsDeleted BIT = NULL
AS
/*
Created: 20210428@Tim.Trinh
Task: Get job level.
DB: SMSEPA
Original:

Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	SELECT JobLevelId
		,[Name]
	FROM dbo.JobLevel WITH (NOLOCK)
	WHERE @IsDeleted IS NULL
		OR IsDeleted = @IsDeleted
END
