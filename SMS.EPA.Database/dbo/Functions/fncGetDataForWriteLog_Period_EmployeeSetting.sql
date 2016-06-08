/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_EmployeeSetting    Script Date: 5/16/2016 2:20:06 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_EmployeeSetting    Script Date: 5/16/2016 2:15:28 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_EmployeeSetting    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Period_EmployeeSetting    Script Date: 4/6/2016 11:27:50 AM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Period_EmployeeSetting] (
			@PeriodId INT
			,@EmployeeId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)	
	DECLARE @PeriodProcesses NVARCHAR(MAX)
	DECLARE @EPAProcesses NVARCHAR(MAX)
	DECLARE @EPADetails NVARCHAR(MAX)
	DECLARE @EPA NVARCHAR(MAX)

	SET @PeriodProcesses = ''

	SELECT @PeriodProcesses = @PeriodProcesses 
							+ '<pp><pid>' + CAST([ProcessId] AS NVARCHAR(10))
							+ '</pid><eva>' + STUFF((
									SELECT ',' + CAST([EvaluatorId] AS varchar(20))
									FROM [dbo].[PeriodFlowSetting] WITH (NOLOCK)
									WHERE [PeriodId] = pef.[PeriodId]
										AND [EmployeeId] = pef.[EmployeeId]
										AND [ProcessId] = pef.[ProcessId]
									FOR XML PATH('')
										,TYPE
									).value('.[1]', 'nvarchar(50)'), 1, 1, '')
							+ '</eva></pp>'
	FROM [dbo].[PeriodFlowSetting] AS pef WITH (NOLOCK)
	WHERE [PeriodId] = @PeriodId
		AND [EmployeeId] = @EmployeeId

	SET @EPAProcesses = ''

	SELECT @EPAProcesses = @EPAProcesses 
							+ '<ep><pid>' + CAST([ProcessId] AS NVARCHAR(10))
							+ '</pid><eva>' + STUFF((
									SELECT ',' + CAST([EvaluatorId] AS varchar(20))
									FROM [dbo].[EPAProcess] WITH (NOLOCK)
									WHERE [EPAId] = epap.[EPAId]
										AND [ProcessId] = epap.[ProcessId]
									FOR XML PATH('')
										,TYPE
									).value('.[1]', 'nvarchar(50)'), 1, 1, '')
							+ '</eva><pri>' + CAST([Priority] AS NVARCHAR)
							+ '</pri><sd>' + CAST([StartDate] AS NVARCHAR(20))
							+ '</sd><ed>' + CAST([EndDate] AS NVARCHAR(20))
							+ '</ed><iep>' + CAST([IsEndProcess] AS NVARCHAR)
							+ '</iep><nab>' + CAST([NoticeAtBegin] AS NVARCHAR)
							+ '</nab><rd>' + CAST([RemindDate] AS NVARCHAR(20))
							+ '</rd><ts>' + CAST([TotalScore] AS NVARCHAR(100)) 
							+ '</ts><as>' + CAST([AdjustScore] AS NVARCHAR(100))
							+ '</as><ed>' + CAST([EvaluateDate] AS NVARCHAR(100))
							+ '</ed></ep>'
	FROM [dbo].[EPAProcess] AS epap WITH (NOLOCK)
	WHERE EXISTS (
		SELECT 1
		FROM [dbo].[EPA] WITH (NOLOCK)
		WHERE [PeriodId] = @PeriodId
			AND [EmployeeId] = @EmployeeId
	)

	SET @EPADetails = ''

	SELECT @EPADetails = @EPADetails 
						+ '<ed><pid>' + CAST([ProcessId] AS NVARCHAR(10))
						+ '</pid><eva>' + CAST([EvaluatorId] AS NVARCHAR(10))
						+ '</eva><kid>' + CAST([KPIId] AS NVARCHAR(10))
						+ '</kid><gid>' + CAST([KPIGroupId] AS NVARCHAR(10))
						+ '</gid><sc>' + [Score]
						+ '</sc><ra>' + [Rate]
						+ '</ra><hc>' + CAST([HasComment] AS NVARCHAR(10))
						+ '</hc><rc>' + CAST([IsRequiredComment] AS NVARCHAR(10))
						+ '</rc><cm>' + [Comment]
						+ '</cm><re>' + CAST([IsRequiredEvaluate] AS NVARCHAR(10))
						+ '</re><kln>' + [KPILogNo]
						+ '</kln><gln>' + [KPIGroupLogNo]
						+ '</gln><isk>' + CAST([IsSpreadKPI] AS NVARCHAR(10))
						+ '</isk><gw>' + CAST([KPIGroupWeight] AS NVARCHAR(10))
						+ '</gw><kw>' + CAST([KPIWeight] AS NVARCHAR(10))
						+ '</kw><ims>' + CAST([IsMaxScore] AS NVARCHAR(10))
						+ '</ims></ed>'
	FROM [dbo].[EPADetail] AS epad WITH (NOLOCK)
	WHERE EXISTS (
		SELECT 1
		FROM [dbo].[EPA] WITH (NOLOCK)
		WHERE [PeriodId] = @PeriodId
			AND [EmployeeId] = @EmployeeId
			AND [EPAId] = epad.[EPAId]
	)

	SET @EPA = ''
	SELECT @EPA = @EPA 
				+ '<epa><eid>' + CAST([EPAId] AS NVARCHAR(10))
				+ '</eid><tid>' + CAST((
						SELECT [TargetTemplateId]
						FROM [dbo].[PeriodTargetSetting] WITH (NOLOCK)
						WHERE [PeriodId] = epa.[PeriodId]
							AND [EmployeeId] = epa.[EmployeeId]
					) AS NVARCHAR(10))
				+ '</tid><s>' + CAST([Status] AS NVARCHAR)
				+ '</s></epa>'		
	FROM [dbo].[EPA] AS epa WITH (NOLOCK)
	WHERE [PeriodId] = @PeriodId
		AND [EmployeeId] = @EmployeeId

	SELECT @Data = CONCAT(
				'<emp><id>', @EmployeeId
				,'</id><pid>', @PeriodId
				,'</pid>', @EPA
				,'<eps>', @EPAProcesses
				,'</eps><eds>', @EPADetails
				,'</eds><pps>', @PeriodProcesses
				,'</pps></emp>')

	RETURN @Data
END