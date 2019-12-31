
CREATE PROCEDURE [dbo].[EpaPeriod_GetById] @PeriodId INT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get evaluation period by Id.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT pe.[PeriodId]
		,pe.[Name]
		,pe.[StartDate]
		,pe.[EndDate]
		,pe.[AttendanceStartDate]
		,pe.[AttendanceEndDate]
		,pe.[Description]
		,pe.[IsActive]
		,tar.[TargetTypeId]
		,tar.[Name] AS TargetName
	FROM [dbo].[Period] AS pe WITH (NOLOCK)
		INNER JOIN [dbo].[TargetType] AS tar WITH (NOLOCK) ON pe.[TargetTypeId] = tar.[TargetTypeId]
	WHERE pe.[PeriodId] = @PeriodId
END