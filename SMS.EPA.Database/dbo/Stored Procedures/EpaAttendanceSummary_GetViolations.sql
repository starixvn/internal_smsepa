
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_GetViolations    Script Date: 5/16/2016 2:20:11 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_GetViolations    Script Date: 5/16/2016 2:15:33 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_GetViolations    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaAttendanceSummary_GetViolations    Script Date: 4/6/2016 11:27:42 AM ******/

CREATE PROCEDURE [dbo].[EpaAttendanceSummary_GetViolations]
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get violations.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;	

	SELECT [Id] AS ViolationId
		,[Name]
	FROM [dbo].[AttendanceViolation] WITH (NOLOCK)
	WHERE [Score] > 0
END