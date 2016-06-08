/****** Object:  Stored Procedure dbo.EPAPeriod_CheckSelfProcessStarted    Script Date: 5/16/2016 2:20:08 PM ******/
/****** Object:  Stored Procedure dbo.EPAPeriod_CheckSelfProcessStarted    Script Date: 5/16/2016 2:15:30 PM ******/
/****** Object:  Stored Procedure dbo.EPAPeriod_CheckSelfProcessStarted    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EPAPeriod_CheckSelfProcessStarted    Script Date: 4/6/2016 11:27:45 AM ******/

CREATE PROCEDURE [dbo].[EPAPeriod_CheckSelfProcessStarted] @PeriodId INT
	,@EmployeeIds VARCHAR(MAX)
AS
/*
Created: 20150924@Josh.Phan
Task: Check that self process started or not.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #Employee ([EmployeeId] INT)

	IF ISNULL(@EmployeeIds, '') <> ''
	BEGIN
		INSERT INTO #Employee ([EmployeeId])
		SELECT DISTINCT val
		FROM fnc_strSplit(@EmployeeIds, ',')

		IF EXISTS (
				SELECT TOP 1 1
				FROM [dbo].[PeriodFlowSetting] AS pe WITH (NOLOCK)
					INNER JOIN #Employee AS emp ON pe.[EmployeeId] = emp.[EmployeeId]
					AND pe.[PeriodId] = @PeriodId
					INNER JOIN [dbo].[EPA] AS epa WITH (NOLOCK) ON pe.[PeriodId] = epa.[PeriodId]
					AND pe.[EmployeeId] = epa.[EmployeeId]
				WHERE pe.[ProcessId] = 2
					AND ISNULL(pe.[StartDate], '') <> ''
					AND CAST(pe.[StartDate] AS DATE) <= CAST(GETDATE() AS DATE)
				)
		BEGIN
			SELECT CAST(1 AS BIT)
		END
		ELSE
		BEGIN
			SELECT CAST(0 AS BIT)
		END
	END
END