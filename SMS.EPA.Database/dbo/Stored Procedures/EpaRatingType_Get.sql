/****** Object:  Stored Procedure dbo.EpaRatingType_Get    Script Date: 5/16/2016 2:20:10 PM ******/
/****** Object:  Stored Procedure dbo.EpaRatingType_Get    Script Date: 5/16/2016 2:15:33 PM ******/
/****** Object:  Stored Procedure dbo.EpaRatingType_Get    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaRatingType_Get    Script Date: 4/6/2016 11:27:45 AM ******/

CREATE PROCEDURE [dbo].[EpaRatingType_Get] 
AS
/*
Created: 20151014@Josh.Phan
Task: Get rating type.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT CAST([Value] AS INT) AS RatingTypeId
		,[Name]
	FROM [dbo].[LookupData] WITH (NOLOCK)
	WHERE [LookupId] = 8 -- Rating Type
END