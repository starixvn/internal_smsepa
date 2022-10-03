
CREATE PROCEDURE [dbo].[Employee_Search] @Query NVARCHAR(100) = NULL
	,@TeamIds VARCHAR(MAX) = NULL
	,@IsSpecialUser BIT = NULL
	,@IsDisabledUser BIT = NULL
	,@ApproverId INT = NULL
	,@IncludeVerifier BIT = NULL
AS
/*
Created: 2022-09-14@Tim.Trinh
Task: search employee
DB: SMS EPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET @query = '%' + ISNULL(@query, '') + '%'

	CREATE TABLE #tmpTeam (TeamId INT)

	CREATE TABLE #tblEmployee (EmployeeId INT)

	IF @TeamIds IS NOT NULL
	BEGIN
		INSERT INTO #tmpTeam (TeamId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
	END

	INSERT INTO #tblEmployee (EmployeeId)
	SELECT DISTINCT emp.EmployeeId
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE emp.IsDeleted = 0
		AND (
			@TeamIds IS NULL
			OR EXISTS (
				SELECT 1
				FROM #tmpTeam AS tmp WITH (NOLOCK)
				WHERE emp.GroupId = tmp.TeamId
				)
			)
		AND (
			emp.FullName LIKE @query
			OR emp.NickName LIKE @query
			)
		AND (
			@IsSpecialUser IS NULL
			OR emp.IsSpecialUser = @IsSpecialUser
			)
		AND (
			(
				@IsDisabledUser IS NULL
				AND emp.[Enabled] = 1
				)
			OR (
				@IsDisabledUser IS NOT NULL
				AND emp.[Enabled] = ~ @IsDisabledUser
				)
			)
		AND (
			@ApproverId IS NULL
			OR (
				@IncludeVerifier IS NULL
				AND EXISTS (
					SELECT 1
					FROM dbo.HRMGroupSupervisor AS grS WITH (NOLOCK)
					WHERE emp.GroupId = grS.GroupId
						AND @ApproverId = grS.OwnerId
						AND grS.ActionType = 3 -- Approver
					)
				)
			OR (
				@IncludeVerifier IS NOT NULL
				AND (
					EXISTS (
						SELECT 1
						FROM dbo.EmployeeRole AS empR
						WHERE empR.EmployeeId = @ApproverId
							AND RoleId IN (1, 2, 3)
						)
					OR EXISTS (
						SELECT 1
						FROM dbo.HRMGroupSupervisor AS grS WITH (NOLOCK)
						WHERE emp.GroupId = grS.GroupId
							AND @ApproverId = grS.OwnerId
							AND grS.ActionType IN (2, 3) -- Approver and Verfier
						)
					)
				)
			)

	SELECT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.HasMonthlyEPA
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM #tblEmployee AS tbl WITH (NOLOCK)
			WHERE emp.EmployeeId = tbl.EmployeeId
			)

	DROP TABLE #tmpTeam

	DROP TABLE #tblEmployee
END