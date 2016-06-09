CREATE PROCEDURE [dbo].[EPA_GetEmployeeToUpdateFinalScore] @PeriodId INT
	,@GroupId INT = NULL
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
		,@Yesterday DATE
		,@TargetTypeId INT

	SET @Yesterday = DATEADD(DAY, - 1, GETDATE())

	SELECT @TargetTypeId = TargetTypeId
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId
		AND IsActive = 1

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
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		WHERE CAST(pe.EndDate AS DATE) <= CAST(GETDATE() AS DATE)
			AND CAST(pc.EndDate AS DATE) < CAST(pe.EndDate AS DATE)
			AND pe.TargetTypeId IN (2, 3) -- monthly, semi annual
			AND epa.[Status] IN (0, 1) -- open/closed
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			AND (
				ISNULL(@GroupId, '') = ''
				OR gr.GroupId = @GroupId
				)
			AND (
				ISNULL(@Query, '') = ''
				OR emp.FullName LIKE '%' + @Query + '%'
				OR emp.NickName LIKE '%' + @Query + '%'
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
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		WHERE CAST(pc.EndDate AS DATE) <= @Yesterday
			AND CAST(pc.EndDate AS DATE) >= CAST(pe.EndDate AS DATE)
			AND pe.TargetTypeId IN (2, 3) -- monthly, semi annul
			AND epa.[Status] IN (0, 1) -- open/closed
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			AND (
				ISNULL(@GroupId, '') = ''
				OR gr.GroupId = @GroupId
				)
			AND (
				ISNULL(@Query, '') = ''
				OR emp.FullName LIKE '%' + @Query + '%'
				OR emp.NickName LIKE '%' + @Query + '%'
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
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		WHERE CAST(pc.EndDate AS DATE) <= @Yesterday
			AND pe.TargetTypeId IN (1, 5) -- probation/promotion
			AND epa.[Status] IN (0, 1) -- open/closed
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			AND (
				ISNULL(@GroupId, '') = ''
				OR gr.GroupId = @GroupId
				)
			AND (
				ISNULL(@Query, '') = ''
				OR emp.FullName LIKE '%' + @Query + '%'
				OR emp.NickName LIKE '%' + @Query + '%'
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
END