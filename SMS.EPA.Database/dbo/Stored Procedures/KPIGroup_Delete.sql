
CREATE PROCEDURE [dbo].[KPIGroup_Delete] @KPIGroupId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150710@Tim.Trinh
Task: Delete KPI Group.
DB: SMSEPA
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.KPIGroup WITH (NOLOCK)
			WHERE KPIGroupId = @KPIGroupId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The KPI Group does not exists.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM [dbo].[TargetTemplateKPIGroup] WITH (NOLOCK)
			WHERE KPIGroupId = @KPIGroupId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete KPI Group unsuccessfully. It has been already existed in target template.'

		RETURN
	END

	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_UpdateStatus] @ObjectId = @KPIGroupId
		,@ObjectType = 2

	BEGIN TRY
		BEGIN TRANSACTION;

		DECLARE @LogNo VARCHAR(20)

		SET @LogNo = CONVERT(VARCHAR(8), GETDATE(), 112) + REPLACE(CONVERT(VARCHAR(12), GETDATE(), 114), ':', '')

		INSERT INTO dbo.KPIGroupHistory (
			[KPIGroupId]
			,[LogNo]
			,[Name]
			,[Description]
			,[IsActive]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT @KPIGroupId
			,@LogNo
			,[Name]
			,[Description]
			,[IsActive]
			,@ModifiedBy
			,GETDATE()
		FROM dbo.KPIGroup WITH (NOLOCK)
		WHERE KPIGroupId = @KPIGroupId

		DELETE
		FROM dbo.KPIGroup
		WHERE KPIGroupId = @KPIGroupId

		DELETE
		FROM dbo.KPIDetail
		WHERE KPIGroupId = @KPIGroupId

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'KPIGroup_Delete'
			,@ObjectId = @KPIGroupId
			,@ObjectTypeId = 2
			,@Data = NULL
			,@OldData = @OldData
			,@ObjectIds = NULL

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete KPI Group unsuccessfully.'
	END CATCH
END