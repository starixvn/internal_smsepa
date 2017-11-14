
CREATE FUNCTION [dbo].[fnc_strSplit] (
	@string NVARCHAR(max)
	,@splitter NVARCHAR(max)
	)
RETURNS TABLE
/*          
Created:           
Task:           
DB:       
Original:          
	       
Revisions:          
	      
Param's Explanation:    
	You need to use option: option (maxrecursion 0) at the statement if the lengh of string exceed 100 levels      	        
*/
AS
RETURN WITH cte AS (
		SELECT CAST(0 AS BIGINT) AS idx1
			,CHARINDEX(@splitter, @string) idx2
		
		UNION ALL
		SELECT idx2 + LEN(@splitter)
			,CHARINDEX(@splitter, @string, idx2 + LEN(@splitter))
		FROM cte
		WHERE idx2 > 0
		)

SELECT SUBSTRING(@string, idx1, COALESCE(NULLIF(idx2, 0), LEN(@string) + 1) - idx1) AS val
FROM cte