/****** Object:  User Defined Function dbo.RelatedDepartment    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.RelatedDepartment    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.RelatedDepartment    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.RelatedDepartment    Script Date: 4/6/2016 11:27:50 AM ******/
CREATE FUNCTION [dbo].[RelatedDepartment] (
	@DepartmentId INT
	)
RETURNS @Departments TABLE (
	DepartmentId INT,
	DepartmentLevel INT
	)
AS
/*
Created: 20150722@Tim.Trinh
Task: Split string by character.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN

	INSERT INTO @Departments (DepartmentId, DepartmentLevel)
	SELECT DepartmentId, DepartmentLevel
	FROM dbo.Department WITH (NOLOCK)
	WHERE ParentId = @DepartmentId

	-- Add new child records until exhausted.
	WHILE @@ROWCOUNT > 0
	INSERT INTO @Departments (DepartmentId, DepartmentLevel)
	SELECT dp.DepartmentId, dp.DepartmentLevel
	FROM dbo.Department AS dp
	INNER JOIN @Departments AS d ON dp.ParentID = d.DepartmentId
	WHERE NOT EXISTS(
		SELECT 1 FROM @Departments AS CurrentRecords 
		WHERE CurrentRecords.DepartmentId= dp.DepartmentId
		)

	RETURN
END