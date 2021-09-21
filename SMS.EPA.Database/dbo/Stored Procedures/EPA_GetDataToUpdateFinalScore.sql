

CREATE PROCEDURE [dbo].[EPA_GetDataToUpdateFinalScore] @PeriodId INT
	,@EmployeeIds VARCHAR(MAX) = NULL
AS
/*
Created: 2021-09-15@Tim.Trinh
Task: get data to update final score
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@TblEPAId IntId
		,@EPAIds VARCHAR(MAX)

	CREATE TABLE #tblEmployee (EmployeeId INT)

	CREATE TABLE #tblDisciplineScore (
		EPAId INT
		,SystemDisciplineScore FLOAT
		,DisciplineScore FLOAT
		,OtherDisciplines FLOAT
		)

	IF ISNULL(@EmployeeIds, '') <> ''
	BEGIN
		INSERT INTO #tblEmployee (EmployeeId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@EmployeeIds, ',')
	END

	INSERT INTO @TblEPAId (Id)
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS process WITH (NOLOCK) ON epa.EPAId = process.EPAId
		INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON epa.PeriodId = pe.PeriodId
		LEFT JOIN #tblEmployee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
	WHERE pe.PeriodId = @PeriodId
		AND process.ProcessId IN (
			SELECT MAX(process1.ProcessId)
			FROM dbo.EPAProcess AS process1 WITH (NOLOCK)
			WHERE process1.EPAId = process.EPAId
			)
		AND (
			(
				pe.TargetTypeId IN (2, 3) -- Monthly, Semi
				AND pe.EndDate < @Today
				AND process.EndDate < @Today
				)
			OR (
				pe.TargetTypeId NOT IN (2, 3) -- Monthly, Semi
				AND process.EndDate < @Today
				)
			)
		AND (
			ISNULL(@EmployeeIds, '') = ''
			OR emp.EmployeeId IS NOT NULL
			)
		AND ISNULL(epa.[Status], 0) <> 2 -- rejected
		AND pe.IsActive = 1

	SELECT @EPAIds = COALESCE(@EPAIds + ',', '') + CAST(Id AS NVARCHAR(20))
	FROM @TblEPAId

	INSERT INTO #tblDisciplineScore (
		EPAId
		,SystemDisciplineScore
		,DisciplineScore
		,OtherDisciplines
		)
	EXECUTE dbo.DisciplineSummary_GetByEPA @EPAIds

	-- General Info
	SELECT DISTINCT epa.EPAId
		,(CASE srt.ScoreRatingTypeId
			WHEN 1 THEN 1
			ELSE 0 END
			) AS HasDisciplineScore
		,(CASE 
			WHEN ds.DisciplineScore IS NULL
				OR ds.DisciplineScore < 0 THEN 0
			ELSE ds.DisciplineScore END
			) AS DisciplineScore
		,epa.[Status] AS StatusId
		,emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.Email
		,period.PeriodId
		,period.[Name]
		,tt.TargetTypeId
		,tt.[Name] AS TargetTypeName
	FROM @TblEPAId AS tbl
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.Id = epa.EPAId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
		LEFT JOIN #tblDisciplineScore AS ds WITH (NOLOCK) ON epa.EPAId = ds.EPAId
	ORDER BY epa.EPAId ASC

	-- Process
	EXECUTE dbo.EPA_GetProcess @TblEPAId

	DROP TABLE #tblDisciplineScore

	DROP TABLE #tblEmployee
END