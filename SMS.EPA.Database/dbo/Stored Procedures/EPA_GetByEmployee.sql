CREATE PROCEDURE [dbo].[EPA_GetByEmployee] @EmployeeId INT
AS
/*
Created: 20150710@Tim.Trinh
Task: Get EPAs by employeeId.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EPAIds IntId
		,@Today DATE = GETDATE()

	INSERT INTO @EPAIds (Id)
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pcSelf WITH (NOLOCK) ON epa.EPAId = pcSelf.EPAId
		AND pcSelf.ProcessId = 2 -- self
		AND pcSelf.EvaluatorId = @EmployeeId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
	WHERE ISNULL(epa.[Status], 0) = 0 -- inprogress
		AND (
			tt.TargetTypeId IN (2, 3) -- monthly, semi
			AND @Today >= pcSelf.StartDate
			AND (
				@Today < period.EndDate
				OR (
					@Today <= pc.EndDate
					AND pc.ProcessId > 1 -- HR
					)
				)
			)
		OR (
			tt.TargetTypeId NOT IN (2, 3)
			AND @Today BETWEEN pc.StartDate
				AND pc.EndDate
			AND pc.ProcessId > 1 -- HR
			)

	-- General Info
	EXECUTE dbo.EPA_GetGeneralInfo @EPAIds

	-- Process
	EXECUTE dbo.EPA_GetProcess @EPAIds
END