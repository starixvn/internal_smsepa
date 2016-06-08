/****** Object:  User Defined Function dbo.fnc_strSplit    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fnc_strSplit    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.fnc_strSplit    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fnc_strSplit    Script Date: 4/6/2016 11:27:49 AM ******/
CREATE FUNCTION [dbo].[fnc_strSplit] (
    @string NVARCHAR(4000)
    ,@splitter CHAR(1)
)
RETURNS @res TABLE (
    id INT PRIMARY KEY
    ,RANK INT
    ,val NVARCHAR(4000)
)
AS
/*
Created: 20150708@Tim.Trinh
Task: Split string by character.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    IF SUBSTRING(@string, LEN(@string), 1) <> @splitter
    BEGIN
	   SET @string = @string + @splitter
    END

    DECLARE @start INT
        ,@word NVARCHAR(4000)
        ,@charindex INT
        ,@i INT

    SET @i = 1
    SET @start = 1
    SET @charindex = CHARINDEX(@splitter, @string, @start)

    WHILE (@charindex <> 0)
    BEGIN
	   SET @word = SUBSTRING(@string, @start, @charindex - @start)
	   SET @start = @charindex + 1
	   SET @charindex = CHARINDEX(@splitter, @string, @start)

	   INSERT INTO @res
	   VALUES (
		  @start
		  ,@i
		  ,LTRIM(RTRIM(@word))
	   )

	   SET @i = @i + 1
    END

    RETURN
END