/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_AddEmployee    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_AddEmployee    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_AddEmployee    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_AddEmployee    Script Date: 4/6/2016 11:27:50 AM ******/

CREATE FUNCTION dbo.fncGetDataForWriteLog_Period_AddEmployee (@PeriodId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
		,@EmployeeIds NVARCHAR(MAX)

	SELECT @EmployeeIds = COALESCE(@EmployeeIds + ',', '') + CAST(pe.EmployeeId AS NVARCHAR)
	FROM (
		SELECT DISTINCT PeriodId
			,EmployeeId
		FROM dbo.PeriodFlowSetting WITH (NOLOCK)
		
		UNION
		SELECT peT.PeriodId
			,peT.EmployeeId
		FROM dbo.PeriodTargetSetting AS peT WITH (NOLOCK)
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.PeriodFlowSetting AS peF WITH (NOLOCK)
				WHERE peF.EmployeeId = peT.EmployeeId
					AND peF.PeriodId = peT.PeriodId
				)
		) AS pe
	WHERE pe.PeriodId = @PeriodId
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodExcludedEmployee AS peEx WITH (NOLOCK)
			WHERE peEx.PeriodId = pe.PeriodId
				AND peEx.EmployeeId = pe.EmployeeId
			)

	SELECT @EmployeeIds = STUFF(@EmployeeIds, 1, 1, '')

	SELECT @Data = CONCAT ('<r><pid>', @PeriodId, '</pid><eids>', @EmployeeIds, '</eids></r>')

	RETURN @Data
END