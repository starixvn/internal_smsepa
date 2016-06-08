/****** Object:  Stored Procedure dbo.EPASummaryImport_Get    Script Date: 5/16/2016 2:20:12 PM ******/
/****** Object:  Stored Procedure dbo.EPASummaryImport_Get    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.EPASummaryImport_Get    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EPASummaryImport_Get    Script Date: 4/6/2016 11:27:46 AM ******/

CREATE PROCEDURE [dbo].[EPASummaryImport_Get]
AS
/*
Created: 20151002@Quinn.Vo
Task: Get EPA summary import to encode it.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT 
		[Id]
		,[OriginFinalScore]
		FROM [dbo].[EPASummaryImport] WITH (NOLOCK)
END