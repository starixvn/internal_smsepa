
CREATE FUNCTION [dbo].[fncGetDataForWriteLog] (
	@ObjectId INT
	,@ObjectType INT
	)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)

	IF (@ObjectType = 1) -- KPI
	BEGIN
		DECLARE @KpiGroupIds VARCHAR(500)

		SET @KpiGroupIds = ''

		SELECT @KpiGroupIds = STUFF((
					SELECT ',' + CAST([KPIGroupId] AS VARCHAR(20))
					FROM [dbo].[KPIDetail]
					WHERE [KPIId] = @ObjectId
					FOR XML PATH('')
						,TYPE
					).value('.[1]', 'nvarchar(500)'), 1, 1, '')

		SELECT @Data = CONCAT ('<r><id>', @ObjectId, '</id><n>', [Name], '</n><a>', [IsActive], '</a><re>', [IsRequiredEvaluate], '</re><rc>', [IsRequiredComment], '</rc><c>', [HasComment], '</c><d>', [Description], '</d><ln>', [LogNo], '</ln><grs>', @KpiGroupIds, '</grs></r>')
		FROM [dbo].[KPI] WITH (NOLOCK)
		WHERE [KPIId] = @ObjectId
	END
	ELSE IF (@ObjectType = 2) -- KPI Group
	BEGIN
		SELECT @Data = CONCAT ('<r><id>', @ObjectId, '</id><n>', [Name], '</n><a>', [IsActive], '</a><d>', [Description], '</d><ln>', [LogNo], '</ln></r>')
		FROM [dbo].[KPIGroup] WITH (NOLOCK)
		WHERE [KPIGroupId] = @ObjectId
	END
	ELSE IF (@ObjectType = 3) -- Target Template
	BEGIN
		SELECT @Data = CONCAT ('<r><id>', @ObjectId, '</id><n>', [Name], '</n><a>', [IsActive], '</a><d>', [Description], '</d><scrt>', [ScoreRatingTypeId], '</scrt><rt>', [RatingTypeId], '</rt><tt>', [TargetTypeId], '</tt></r>')
		FROM [dbo].[TargetTemplate] WITH (NOLOCK)
		WHERE [TargetTemplateId] = @ObjectId
	END
	ELSE IF (@ObjectType = 4) -- User Management
	BEGIN
		SELECT @Data = CONCAT ('<r><id>', @ObjectId, '</id><ni>', [NickName], '</ni><fu>', [FullName], '</fu><un>', [DomainUsername], '</un><pa>', [Password], '</pa><se>', [GroupId], '</se><<ca>', [IsLocal], '</ca><em>', [Email], '</em>jt>', [JobTitle], '</jt><en>', [Enabled], '</en></r>')
		FROM [dbo].[Employee] WITH (NOLOCK)
		WHERE [EmployeeId] = @ObjectId
	END
	ELSE IF (@ObjectType = 6) -- Flow Template
	BEGIN
		SELECT @Data = CONCAT ('<r><id>', @ObjectId, '</id><n>', [Name], '</n><a>', [IsActive], '</a><d>', [Description], '</d><tt>', [TargetTypeId], '</tt></r>')
		FROM [dbo].[FlowTemplate] WITH (NOLOCK)
		WHERE [FlowTemplateId] = @ObjectId
	END
	ELSE IF (@ObjectType = 8) -- Period
	BEGIN
		SELECT @Data = CONCAT ('<r><id>', @ObjectId, '</id><n>', [Name], '</n><a>', [IsActive], '</a><d>', [Description], '</d><tt>', [TargetTypeId], '</tt><sd>', [StartDate], '</sd><ed>', [EndDate], '</ed><asd>', [AttendanceStartDate], '</asd><aed>', [AttendanceEndDate], '</aed></r>')
		FROM [dbo].[Period] WITH (NOLOCK)
		WHERE [PeriodId] = @ObjectId
	END

	RETURN @Data
END