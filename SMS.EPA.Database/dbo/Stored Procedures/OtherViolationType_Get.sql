/****** Object:  Stored Procedure dbo.OtherViolationType_Get    Script Date: 5/16/2016 2:20:07 PM ******/
/****** Object:  Stored Procedure dbo.OtherViolationType_Get    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  Stored Procedure dbo.OtherViolationType_Get    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  Stored Procedure dbo.OtherViolationType_Get    Script Date: 4/6/2016 11:27:47 AM ******/

CREATE PROCEDURE [dbo].[OtherViolationType_Get] @IsActive BIT = NULL
AS
/*
Created: 20151023@Josh.Phan
Task: Get other violation type.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT [Id]
		,[Name]
		,[Value]
		,[IsActive]
	FROM [dbo].[OtherViolationType] WITH (NOLOCK)
	WHERE ISNULL(@IsActive, '') = ''
		OR [IsActive] = @IsActive
END