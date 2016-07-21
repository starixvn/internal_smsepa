CREATE PROCEDURE [dbo].[EpaAttendanceSummary_GetViolationType] @IncludeOtherViolate BIT
	,@IsActive BIT = NULL
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get violation type.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DisciplineTypeId AS ViolationId
		,[Name]
	FROM dbo.DisciplineType WITH (NOLOCK)
	WHERE (
			ISNULL(@IsActive, '') = ''
			OR IsActive = @IsActive
			)
		AND (
			@IncludeOtherViolate = 1
			OR (
				@IncludeOtherViolate = 0
				AND DisciplineTypeId > 0
				)
			)
END
