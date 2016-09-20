
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