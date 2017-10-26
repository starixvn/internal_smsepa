
CREATE PROCEDURE [dbo].[EpaAuto_CheckSendMailAllWhenPeriodStarted]
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Get period has employee that started evaluation.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT CAST(Value AS BIT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'SendMailToAll'
END