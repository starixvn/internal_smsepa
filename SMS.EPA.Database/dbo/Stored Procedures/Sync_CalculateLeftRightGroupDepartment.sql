
CREATE PROCEDURE [dbo].[Sync_CalculateLeftRightGroupDepartment]
AS
/*
Created: 20151016@Josh.Phan
Task: Calculate left, right of group and department after sync data from HRM.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	-- Get parent department
	DECLARE @ParentDepartmentId INT
		,@index INT

	SET @ParentDepartmentId = (
			SELECT [DepartmentId]
			FROM [dbo].[Department] WITH (NOLOCK)
			WHERE ISNULL([ParentId], '') = ''
				AND [IsActive] = 1
			)
	SET @index = 1

	-- Set left for this parent deparment
	UPDATE [dbo].[Department] WITH (ROWLOCK, UPDLOCK)
	SET [lft] = @index
	WHERE [DepartmentId] = @ParentDepartmentId

	-- Get all child department
	DECLARE @department_id INT

	DECLARE department_cursor CURSOR
	FOR
	SELECT [DepartmentId]
	FROM [dbo].[Department] WITH (NOLOCK)
	WHERE [ParentId] = @ParentDepartmentId
		AND [IsActive] = 1
	ORDER BY [DepartmentId] ASC

	OPEN department_cursor

	FETCH NEXT
	FROM department_cursor
	INTO @department_id

	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE [dbo].[Department] WITH (ROWLOCK, UPDLOCK)
		SET [lft] = @index + 1
		WHERE [DepartmentId] = @department_id

		SET @index = @index + 1

		DECLARE @group_id INT

		DECLARE group_cursor CURSOR
		FOR
		SELECT [GroupId]
		FROM [dbo].[Group] WITH (NOLOCK)
		WHERE [DepartmentId] = @department_id
			AND [IsDeleted] = 0
		ORDER BY [GroupId] ASC

		OPEN group_cursor

		FETCH NEXT
		FROM group_cursor
		INTO @group_id

		WHILE @@FETCH_STATUS = 0
		BEGIN
			UPDATE [dbo].[Group] WITH (ROWLOCK, UPDLOCK)
			SET [lft] = @index + 1
				,[rgt] = @index + 2
				,[ModifiedDate] = GETDATE()
			WHERE [GroupId] = @group_id

			SET @index = @index + 2

			FETCH NEXT
			FROM group_cursor
			INTO @group_id
		END

		CLOSE group_cursor

		DEALLOCATE group_cursor

		UPDATE [dbo].[Department] WITH (ROWLOCK, UPDLOCK)
		SET [rgt] = @index + 1
			,[ModifiedDate] = GETDATE()
		WHERE [DepartmentId] = @department_id

		SET @index = @index + 1

		FETCH NEXT
		FROM department_cursor
		INTO @department_id
	END

	CLOSE department_cursor;

	DEALLOCATE department_cursor;

	UPDATE [dbo].[Department] WITH (ROWLOCK, UPDLOCK)
	SET [rgt] = @index + 1
		,[ModifiedDate] = GETDATE()
	WHERE [DepartmentId] = @ParentDepartmentId
END