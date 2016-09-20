
CREATE FUNCTION [dbo].[fncGetDataForWriteLog_UpdateStatus] (
	@ObjectId INT
	,@ObjectType INT
	)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)

	IF (@ObjectType = 1) -- KPI
	BEGIN
		SELECT @Data = CONCAT ('<r><id>', @ObjectId, '</id><a>', [IsActive], '</a><ln>', [LogNo], '</ln></r>')
		FROM [dbo].[KPI] WITH (NOLOCK)
		WHERE [KPIId] = @ObjectId
	END
	ELSE IF (@ObjectType = 2) -- KPI Group
	BEGIN
		SELECT @Data = CONCAT ('<r><id>', @ObjectId, '</id><a>', [IsActive], '</a><ln>', [LogNo], '</ln></r>')
		FROM [dbo].[KPIGroup] WITH (NOLOCK)
		WHERE [KPIGroupId] = @ObjectId
	END
	ELSE IF (@ObjectType = 3) -- Target Template
	BEGIN
		SELECT @Data = CONCAT ('<r><id>', @ObjectId, '</id><a>', [IsActive], '</a></r>')
		FROM [dbo].[TargetTemplate] WITH (NOLOCK)
		WHERE [TargetTemplateId] = @ObjectId
	END
	ELSE IF (@ObjectType = 6) -- Flow Template
	BEGIN
		SELECT @Data = CONCAT ('<r><id>', @ObjectId, '</id><a>', [IsActive], '</a></r>')
		FROM [dbo].[FlowTemplate] WITH (NOLOCK)
		WHERE [FlowTemplateId] = @ObjectId
	END
	ELSE IF (@ObjectType = 8) -- Period
	BEGIN
		SELECT @Data = CONCAT ('<r><id>', @ObjectId, '</id><a>', [IsActive], '</a></r>')
		FROM [dbo].[Period] WITH (NOLOCK)
		WHERE [PeriodId] = @ObjectId
	END

	RETURN @Data
END