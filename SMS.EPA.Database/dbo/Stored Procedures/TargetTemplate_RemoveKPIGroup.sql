/****** Object:  Stored Procedure dbo.TargetTemplate_RemoveKPIGroup    Script Date: 5/16/2016 2:20:13 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_RemoveKPIGroup    Script Date: 5/16/2016 2:15:34 PM ******/
/****** Object:  Stored Procedure dbo.TargetTemplate_RemoveKPIGroup    Script Date: 5/16/2016 1:59:44 PM ******/

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
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

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

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Remove KPI group failed.'
	END
	ELSE
	BEGIN
		EXEC @Data = [dbo].[fncGetDataForWriteLog_TargetTemplate_UpdateKPI] @ObjectId = @TargetTemplateId

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'TargetTemplate_RemoveKPIGroup'
			,@ObjectId = @TargetTemplateId
			,@ObjectTypeId = 3
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END