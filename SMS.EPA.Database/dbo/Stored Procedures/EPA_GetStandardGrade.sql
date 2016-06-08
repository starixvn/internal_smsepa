/****** Object:  Stored Procedure dbo.EPA_GetStandardGrade    Script Date: 5/16/2016 2:20:12 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetStandardGrade    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetStandardGrade    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EPA_GetStandardGrade    Script Date: 4/6/2016 11:27:42 AM ******/

CREATE PROCEDURE [dbo].[EPA_GetStandardGrade]
AS
/*
Created: 20151002@Tim.Trinh
Task: Get list standard grade.
DB: SPM
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT GradeId
		,[Name]
		,RTRIM(Code) AS Code
		,LeftScore
		,RightScore
	FROM dbo.StandardGrade
END