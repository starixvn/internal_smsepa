/****** Object:  Stored Procedure dbo.EpaAuto_CheckLastestEndEvaluationPeriod    Script Date: 5/16/2016 2:20:12 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_CheckLastestEndEvaluationPeriod    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_CheckLastestEndEvaluationPeriod    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_CheckLastestEndEvaluationPeriod    Script Date: 4/6/2016 11:27:43 AM ******/

CREATE PROCEDURE [dbo].[EpaAuto_CheckLastestEndEvaluationPeriod] @PeriodId INT
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Check lastest end process of period.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Yesterday DATETIME

	SET @Yesterday = DATEADD(DAY, - 1, GETDATE())

	IF EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[EPAProcess] AS epaP WITH (NOLOCK)
				INNER JOIN [dbo].[EPA] AS epa WITH (NOLOCK) ON epaP.[EPAId] = epa.[EPAId]
				AND epa.[PeriodId] = @PeriodId
			WHERE CAST(epaP.[EndDate] AS DATE) > CAST(@Yesterday AS DATE)
				AND [ProcessId] IN (
					SELECT MAX([ProcessId])
					FROM [dbo].[EPAProcess] WITH (NOLOCK)
					WHERE [EPAId] = epa.[EPAId]
					)
				AND epa.[Status] <> 2 -- Rejected
			)
	BEGIN
		SELECT 0
	END
	ELSE
	BEGIN
		SELECT 1
	END
END