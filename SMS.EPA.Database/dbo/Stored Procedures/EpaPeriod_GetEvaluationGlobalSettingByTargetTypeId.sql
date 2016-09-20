
CREATE PROCEDURE [dbo].[EpaPeriod_GetEvaluationGlobalSettingByTargetTypeId] @TargetTypeId INT
AS
/*
Created: 20160223@Tim.Trinh
Task: Get global process setting of monthly period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT ProcessId
		,Duration
		,NoticeAtBegin
		,RemindDay
	FROM dbo.EvaluationGlobalSetting WITH (NOLOCK)
	WHERE TargetTypeId = @TargetTypeId
END