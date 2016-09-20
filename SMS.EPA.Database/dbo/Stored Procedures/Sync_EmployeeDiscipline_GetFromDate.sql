
CREATE PROCEDURE [dbo].[Sync_EmployeeDiscipline_GetFromDate]
AS
/*
Created: 20151109@Quinn.Vo
Task: Get from date to sync attendance violation.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
		,@CurrentDate DATETIME

	SELECT @CurrentDate = GETDATE()

	IF (
			NOT EXISTS (
				SELECT 1
				FROM dbo.Period WITH (NOLOCK)
				WHERE TargetTypeId = 3
					AND @CurrentDate BETWEEN StartDate
						AND EndDate
				)
			)
	BEGIN
		SELECT @FromDate = MAX(AttendanceEndDate)
		FROM dbo.Period WITH (NOLOCK)
		WHERE TargetTypeId = 3
			AND @CurrentDate > EndDate

		SELECT @FromDate = DATEADD(DD, 1, @FromDate)
	END
	ELSE
	BEGIN
		SELECT @FromDate = MIN(AttendanceStartDate)
		FROM dbo.Period WITH (NOLOCK)
		WHERE TargetTypeId = 3
			AND @CurrentDate BETWEEN StartDate
				AND EndDate
	END

	SELECT ISNULL(@FromDate, '2015-01-01') AS FromDate
END