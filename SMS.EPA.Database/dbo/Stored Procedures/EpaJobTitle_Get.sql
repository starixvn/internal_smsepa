/****** Object:  Stored Procedure dbo.EpaJobTitle_Get    Script Date: 5/16/2016 2:20:08 PM ******/
/****** Object:  Stored Procedure dbo.EpaJobTitle_Get    Script Date: 5/16/2016 2:15:30 PM ******/
/****** Object:  Stored Procedure dbo.EpaJobTitle_Get    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaJobTitle_Get    Script Date: 4/6/2016 11:27:44 AM ******/

CREATE PROCEDURE [dbo].[EpaJobTitle_Get]
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get users.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;	

	SELECT 
		[Id] AS JobTitleId
		,[Name] AS JobTitleName
	FROM [dbo].[HRMJobTitle] WITH (NOLOCK)
	WHERE [LookupId] = 1		
END