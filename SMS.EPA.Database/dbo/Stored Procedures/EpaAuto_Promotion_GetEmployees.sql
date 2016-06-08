/****** Object:  Stored Procedure dbo.EpaAuto_Promotion_GetEmployees    Script Date: 5/16/2016 2:20:07 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_Promotion_GetEmployees    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_Promotion_GetEmployees    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  Stored Procedure dbo.EpaAuto_Promotion_GetEmployees    Script Date: 4/6/2016 11:27:43 AM ******/

CREATE PROCEDURE [dbo].[EpaAuto_Promotion_GetEmployees]
AS
/*
Created: 2015-09-01@Josh.Phan
Task: Get employee that has promotion started today.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;	

	SELECT DISTINCT pe.[PeriodId]
		,peFS.[EmployeeId] 
	FROM [dbo].[Period] AS pe WITH (NOLOCK)
		INNER JOIN [dbo].[PeriodFlowSetting] AS peFS WITH (NOLOCK) ON pe.[PeriodId] = peFS.[PeriodId]
	WHERE pe.[TargetTypeId] = 5 -- Promotion
		AND peFS.[ProcessId] = 1 -- Human Resource
		AND CAST(peFS.[StartDate] AS DATE) = CAST(GETDATE() AS DATE)
END