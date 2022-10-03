
CREATE PROCEDURE [dbo].[EPA_GetEmployeeToUpdateFinalScore] @PeriodId INT
	,@TeamIds VARCHAR(MAX) = NULL
	,@IsLocal BIT = NULL
	,@Query NVARCHAR(100) = NULL
AS
/*
Created: 20160531@Tim.Trinh
Task: Get employee.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EPAIds IntId
		,@Today DATE = GETDATE()
		,@Yesterday DATE = DATEADD(DAY, - 1, GETDATE())
		,@TargetTypeId INT

	SELECT @TargetTypeId = TargetTypeId
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId
		AND IsActive = 1

	SET @query = '%' + ISNULL(@query, '') + '%'

	CREATE TABLE #tmpTeam (TeamId INT)

	IF @TeamIds IS NOT NULL
	BEGIN
		INSERT INTO #tmpTeam (TeamId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
	END

	IF @TargetTypeId IN (2, 3)
	BEGIN
		INSERT INTO @EPAIds (Id)
		SELECT DISTINCT epa.EPAId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN (
			SELECT DISTINCT EPAId
				,MAX(EndDate) AS EndDate
			FROM dbo.EPAProcess WITH (NOLOCK)
			GROUP BY EPAId
			) AS pc ON epa.EPAId = pc.EPAId
			INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON epa.PeriodId = pe.PeriodId
			AND pe.EndDate <= @Today
			AND pe.TargetTypeId IN (2, 3) -- monthly, semi annual
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		WHERE pc.EndDate < pe.EndDate
			AND epa.[Status] IN (0, 1) -- open/closed
			AND (
				@TeamIds IS NULL
				OR EXISTS (
					SELECT 1
					FROM #tmpTeam AS tmp WITH (NOLOCK)
					WHERE gr.GroupId = tmp.TeamId
					)
				)
			AND (
				@IsLocal IS NULL
				OR emp.IsLocal = emp.IsLocal
				)
			AND (
				emp.FullName LIKE @query
				OR emp.NickName LIKE @query
				)
		
		UNION
		SELECT DISTINCT epa.EPAId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN (
			SELECT DISTINCT EPAId
				,MAX(EndDate) AS EndDate
			FROM dbo.EPAProcess WITH (NOLOCK)
			GROUP BY EPAId
			) AS pc ON epa.EPAId = pc.EPAId
			INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
			AND pe.TargetTypeId IN (2, 3) -- monthly, semi annual
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		WHERE pc.EndDate <= @Yesterday
			AND pc.EndDate >= pe.EndDate
			AND epa.[Status] IN (0, 1) -- open/closed
			AND (
				@TeamIds IS NULL
				OR EXISTS (
					SELECT 1
					FROM #tmpTeam AS tmp WITH (NOLOCK)
					WHERE gr.GroupId = tmp.TeamId
					)
				)
			AND (
				@IsLocal IS NULL
				OR emp.IsLocal = emp.IsLocal
				)
			AND (
				emp.FullName LIKE @query
				OR emp.NickName LIKE @query
				)
	END
	ELSE IF @TargetTypeId IN (1, 5) -- probation/promotion
	BEGIN
		INSERT INTO @EPAIds (Id)
		SELECT DISTINCT epa.EPAId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN (
			SELECT DISTINCT EPAId
				,MAX(EndDate) AS EndDate
			FROM dbo.EPAProcess WITH (NOLOCK)
			GROUP BY EPAId
			) AS pc ON epa.EPAId = pc.EPAId
			INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
			AND pe.TargetTypeId IN (1, 5) -- probation/promotion
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		WHERE pc.EndDate <= @Yesterday
			AND epa.[Status] IN (0, 1) -- open/closed
			AND (
				@TeamIds IS NULL
				OR EXISTS (
					SELECT 1
					FROM #tmpTeam AS tmp WITH (NOLOCK)
					WHERE gr.GroupId = tmp.TeamId
					)
				)
			AND (
				@IsLocal IS NULL
				OR emp.IsLocal = emp.IsLocal
				)
			AND (
				emp.FullName LIKE @query
				OR emp.NickName LIKE @query
				)
	END

	SELECT emp.EmployeeId
		,emp.NickName
		,emp.FullName
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN @EPAIds AS epat ON epa.EPAId = epat.Id
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1

	DROP TABLE #tmpTeam
END
