/****** Object:  Stored Procedure dbo.EpaPeriod_GetProcessSetting    Script Date: 5/16/2016 2:20:08 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_GetProcessSetting    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_GetProcessSetting    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_GetProcessSetting    Script Date: 4/6/2016 11:27:45 AM ******/

CREATE PROCEDURE [dbo].[EpaPeriod_GetProcessSetting] @PeriodId INT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get period process setting.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT [ProcessId]
		,[Duration]
		,[NoticeAtBegin]
		,[RemindDay]
	FROM [dbo].[PeriodProcessSetting] WITH (NOLOCK)
	WHERE [PeriodId] = @PeriodId
END