/****** Object:  Stored Procedure dbo.EpaPeriod_GetEvaluationGlobalSettingByTargetTypeId    Script Date: 5/16/2016 2:20:08 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_GetEvaluationGlobalSettingByTargetTypeId    Script Date: 5/16/2016 2:15:30 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_GetEvaluationGlobalSettingByTargetTypeId    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_GetEvaluationGlobalSettingByTargetTypeId    Script Date: 4/6/2016 11:27:45 AM ******/

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