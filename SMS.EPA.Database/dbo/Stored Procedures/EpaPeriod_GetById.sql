/****** Object:  Stored Procedure dbo.EpaPeriod_GetById    Script Date: 5/16/2016 2:20:08 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_GetById    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_GetById    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_GetById    Script Date: 4/6/2016 11:27:45 AM ******/

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
		,tar.[Name]
	FROM [dbo].[Period] AS pe WITH (NOLOCK)
		INNER JOIN [dbo].[TargetType] AS tar WITH (NOLOCK) ON pe.[TargetTypeId] = tar.[TargetTypeId]
	WHERE pe.[PeriodId] = @PeriodId
END