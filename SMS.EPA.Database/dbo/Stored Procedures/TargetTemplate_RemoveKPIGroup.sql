
CREATE PROCEDURE [dbo].[TargetTemplate_RemoveKPIGroup] @TargetTemplateId INT
	,@KPIGroupId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140404@Quinn.Vo
Task: Remove KPI group in target template.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_TargetTemplate_UpdateKPI] @ObjectId = @TargetTemplateId

	DELETE
	FROM dbo.TargetTemplateKPI
	WHERE TargetTemplateId = @TargetTemplateId
		AND KPIGroupId = @KPIGroupId

	DELETE
	FROM dbo.TargetTemplateKPIGroup
	WHERE TargetTemplateId = @TargetTemplateId
		AND KPIGroupId = @KPIGroupId

	UPDATE dbo.TargetTemplate WITH (ROWLOCK, UPDLOCK)
	SET ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE TargetTemplateId = @TargetTemplateId

	EXEC @Data = [dbo].[fncGetDataForWriteLog_TargetTemplate_UpdateKPI] @ObjectId = @TargetTemplateId

	EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
		,@SpName = 'TargetTemplate_RemoveKPIGroup'
		,@ObjectId = @TargetTemplateId
		,@ObjectTypeId = 3
		,@Data = @Data
		,@OldData = @OldData
		,@ObjectIds = NULL
END