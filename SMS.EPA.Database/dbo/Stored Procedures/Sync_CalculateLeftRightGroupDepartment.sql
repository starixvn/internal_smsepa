
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

	DECLARE @RootDepartmentId INT
		,@index INT

	SET @RootDepartmentId = (
			SELECT DepartmentId
			FROM dbo.Department WITH (NOLOCK)
			WHERE ParentId IS NULL
				AND IsActive = 1
			)
	SET @index = 1

	UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
	SET lft = @index
	WHERE DepartmentId = @RootDepartmentId

	-- unit
	DECLARE @unit_id INT
	DECLARE unit_cursor CURSOR
	FOR
	SELECT DepartmentId
	FROM dbo.Department WITH (NOLOCK)
	WHERE ParentId = @RootDepartmentId
	ORDER BY DepartmentId ASC

	OPEN unit_cursor
	FETCH NEXT
	FROM unit_cursor
	INTO @unit_id

	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
		SET lft = @index + 1
		WHERE DepartmentId = @unit_id

		SET @index = @index + 1

		-- department
		DECLARE @department_id INT
		DECLARE department_cursor CURSOR
		FOR
		SELECT DepartmentId
		FROM dbo.Department WITH (NOLOCK)
		WHERE ParentId = @unit_id
		ORDER BY DepartmentId ASC

		OPEN department_cursor
		FETCH NEXT
		FROM department_cursor
		INTO @department_id

		WHILE @@FETCH_STATUS = 0
		BEGIN
			UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
			SET lft = @index + 1
			WHERE DepartmentId = @department_id

			SET @index = @index + 1

			-- section
			DECLARE @section_id INT
			DECLARE section_cursor CURSOR
			FOR
			SELECT DepartmentId
			FROM dbo.Department WITH (NOLOCK)
			WHERE ParentId = @department_id
			ORDER BY DepartmentId ASC

			OPEN section_cursor
			FETCH NEXT
			FROM section_cursor
			INTO @section_id

			WHILE @@FETCH_STATUS = 0
			BEGIN
				UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
				SET lft = @index + 1
				WHERE DepartmentId = @section_id

				SET @index = @index + 1

				DECLARE @group_id INT
				DECLARE group_cursor CURSOR
				FOR
				SELECT GroupId
				FROM dbo.[Group] WITH (NOLOCK)
				WHERE DepartmentId = @section_id
				ORDER BY GroupId ASC

				OPEN group_cursor
				FETCH NEXT
				FROM group_cursor
				INTO @group_id

				WHILE @@FETCH_STATUS = 0
				BEGIN
					UPDATE dbo.[Group] WITH (ROWLOCK, UPDLOCK)
					SET lft = @index + 1
						,rgt = @index + 2
					WHERE GroupId = @group_id

					SET @index = @index + 2

					FETCH NEXT
					FROM group_cursor
					INTO @group_id
				END

				CLOSE group_cursor
				DEALLOCATE group_cursor

				UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
				SET rgt = @index + 1
				WHERE DepartmentId = @section_id

				SET @index = @index + 1
				
				FETCH NEXT
				FROM section_cursor
				INTO @section_id
			END

			CLOSE section_cursor
			DEALLOCATE section_cursor

			UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
			SET rgt = @index + 1
			WHERE DepartmentId = @department_id

			SET @index = @index + 1

			FETCH NEXT
			FROM department_cursor
			INTO @department_id
		END

		CLOSE department_cursor
		DEALLOCATE department_cursor

		UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
		SET rgt = @index + 1
		WHERE DepartmentId = @unit_id

		SET @index = @index + 1
		
		FETCH NEXT
		FROM unit_cursor
		INTO @unit_id
	END

	CLOSE unit_cursor
	DEALLOCATE unit_cursor

	UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
	SET rgt = @index + 1
	WHERE DepartmentId = @RootDepartmentId
END
