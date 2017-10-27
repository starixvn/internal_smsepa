
CREATE PROCEDURE [dbo].[EpaEmployee_UpdateHasMonthlyEPA] @EmployeeId INT
	,@HasMonthlyEPA BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160129@Tim.Trinh
Task: Update employee.
DB: SMS EPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	CREATE TABLE #tblPeriod (
		PeriodId INT
		,EndDate DATE
		)

	INSERT INTO #tblPeriod (
		PeriodId
		,EndDate
		)
	SELECT pFS.PeriodId
		,(CASE 
			WHEN period.EndDate > pFS.EndDate THEN period.EndDate
			ELSE pFS.EndDate END
			) AS EndDate
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN (
		SELECT PeriodId
			,EmployeeId
			,MAX(EndDate) AS EndDate
		FROM dbo.PeriodFlowSetting WITH (NOLOCK)
		WHERE EmployeeId = @EmployeeId
		GROUP BY PeriodId
			,EmployeeId
		) AS pFS ON period.PeriodId = pFS.PeriodId
	WHERE period.TargetTypeId IN (2, 3) -- monthly, semi

	IF @HasMonthlyEPA = 0
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM #tblPeriod AS period WITH (NOLOCK)
					LEFT JOIN dbo.EPA AS epa WITH (NOLOCK) ON period.PeriodId = epa.PeriodId
					AND epa.EmployeeId = @EmployeeId
				WHERE period.EndDate >= @Today
					AND NOT EXISTS (
						SELECT 1
						FROM dbo.PeriodExcludedEmployee AS pEx WITH (NOLOCK)
						WHERE pEx.EmployeeId = @EmployeeId
							AND pEx.PeriodId = period.PeriodId
						)
					AND (
						epa.[Status] IS NULL
						OR epa.[Status] = 0
						)
				)
		BEGIN
			DECLARE @NickName NVARCHAR(20)

			SELECT @NickName = NickName
			FROM dbo.Employee WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId

			SELECT @ErrCode = 1
				,@ErrMsg = 'Cannot unset monthly evaluation for <b>' + @NickName + '</b> because <b>' + @NickName + '</b> has been included in Monthly or Semi period.'

			RETURN
		END
	END

	UPDATE [dbo].[Employee] WITH (ROWLOCK, UPDLOCK)
	SET HasMonthlyEPA = @HasMonthlyEPA
		,[ModifiedBy] = @ModifiedBy
		,[ModifiedDate] = GETDATE()
	WHERE [EmployeeId] = @EmployeeId

	DROP TABLE #tblPeriod
END