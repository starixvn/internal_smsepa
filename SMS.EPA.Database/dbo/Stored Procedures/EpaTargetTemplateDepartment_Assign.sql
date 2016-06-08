/****** Object:  Stored Procedure dbo.EpaTargetTemplateDepartment_Assign    Script Date: 5/16/2016 2:20:09 PM ******/
/****** Object:  Stored Procedure dbo.EpaTargetTemplateDepartment_Assign    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.EpaTargetTemplateDepartment_Assign    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.EpaTargetTemplateDepartment_Assign    Script Date: 4/6/2016 11:27:46 AM ******/

CREATE PROCEDURE [dbo].[EpaTargetTemplateDepartment_Assign] @TargetTemplateId INT
	,@Departments VARCHAR(MAX)
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Assign target template to department and group.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
	@Departments = <ROOT>
					<Department DepartmentId='1' IsGroup='1'></Department>
					<Department DepartmentId='1' IsGroup='0'></Department>
					</ROOT>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TargetTypeId INT
		,@AllowMultiple BIT
		,@DocHandler INT

	SET @TargetTypeId = (
			SELECT tarTemp.[TargetTypeId]
			FROM [dbo].[TargetTemplate] AS tarTemp WITH (NOLOCK)
				INNER JOIN [dbo].[TargetType] AS tar WITH (NOLOCK) ON tarTemp.[TargetTypeId] = tar.[TargetTypeId]
			WHERE tarTemp.[TargetTemplateId] = @TargetTemplateId
			)
	SET @AllowMultiple = (
			SELECT tar.[AllowMultiple]
			FROM [dbo].[TargetTemplate] AS tarTemp WITH (NOLOCK)
				INNER JOIN [dbo].[TargetType] AS tar WITH (NOLOCK) ON tarTemp.[TargetTypeId] = tar.[TargetTypeId]
			WHERE tarTemp.[TargetTemplateId] = @TargetTemplateId
			)

	CREATE TABLE #DepartmentSetting (
		[DepartmentId] INT
		,[IsGroup] BIT
		,[TargetTemplateId] INT
		)

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@Departments

	INSERT INTO #DepartmentSetting (
		[DepartmentId]
		,[IsGroup]
		,[TargetTemplateId]
		)
	SELECT [DepartmentId]
		,[IsGroup]
		,@TargetTemplateId
	FROM OPENXML(@DocHandler, '/ROOT/Department', 1) WITH (DepartmentId INT, IsGroup BIT) AS Data

	EXEC sp_xml_removedocument @DocHandler

	DECLARE @OldData NVARCHAR(MAX)
		,@DepartmentId INT
		,@IsGroup BIT
		,@Dept CURSOR
		,@DeptLogDetail NVARCHAR(MAX)

	SET @OldData = '<r>'
	SET @Dept = CURSOR
	FOR

	SELECT [DepartmentId]
		,[IsGroup]
	FROM #DepartmentSetting

	OPEN @Dept

	FETCH NEXT
	FROM @Dept
	INTO @DepartmentId
		,@IsGroup

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @DeptLogDetail = ''

		EXEC @DeptLogDetail = [dbo].[fncGetDataForWriteLog_TargetTemplateSetting_Department] @DepartmentId = @DepartmentId
			,@IsGroup = @IsGroup

		SET @OldData = @OldData + @DeptLogDetail

		FETCH NEXT
		FROM @Dept
		INTO @DepartmentId
			,@IsGroup
	END

	CLOSE @Dept

	DEALLOCATE @Dept

	SET @OldData = @OldData + '</r>'

	BEGIN TRY
		BEGIN TRANSACTION

		SELECT @AllowMultiple

		IF @AllowMultiple = 1
		BEGIN
			DELETE
			FROM [dbo].[TargetTemplateDepartment]
			WHERE EXISTS (
					SELECT TOP 1 1
					FROM #DepartmentSetting AS deptSet
					WHERE [dbo].[TargetTemplateDepartment].[DepartmentId] = deptSet.DepartmentId
						AND [dbo].[TargetTemplateDepartment].[IsGroup] = deptSet.IsGroup
						AND [dbo].[TargetTemplateDepartment].TargetTemplateId = @TargetTemplateId
					)
		END

		IF ISNULL(@AllowMultiple, '') = ''
		BEGIN
			DELETE
			FROM [dbo].[TargetTemplateDepartment]
			WHERE EXISTS (
					SELECT TOP 1 1
					FROM #DepartmentSetting AS deptSet
					WHERE [dbo].[TargetTemplateDepartment].[DepartmentId] = deptSet.[DepartmentId]
						AND [dbo].[TargetTemplateDepartment].[IsGroup] = deptSet.[IsGroup]
						AND [dbo].[TargetTemplateDepartment].[TargetTemplateId] IN (
							SELECT [TargetTemplateId]
							FROM [dbo].[TargetTemplate] WITH (NOLOCK)
							WHERE [TargetTypeId] = @TargetTypeId
							)
					)
		END

		INSERT INTO [dbo].[TargetTemplateDepartment] (
			[TargetTemplateId]
			,[DepartmentId]
			,[IsGroup]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT @TargetTemplateId
			,[DepartmentId]
			,[IsGroup]
			,@ModifiedBy
			,GETDATE()
		FROM #DepartmentSetting

		COMMIT TRANSACTION

		DECLARE @Data NVARCHAR(MAX)

		SET @Data = '<r>'
		SET @Dept = CURSOR
		FOR

		SELECT [DepartmentId]
			,[IsGroup]
		FROM #DepartmentSetting

		OPEN @Dept

		FETCH NEXT
		FROM @Dept
		INTO @DepartmentId
			,@IsGroup

		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @DeptLogDetail = ''

			EXEC @DeptLogDetail = [dbo].[fncGetDataForWriteLog_TargetTemplateSetting_Department] @DepartmentId = @DepartmentId
				,@IsGroup = @IsGroup

			SET @Data = @Data + @DeptLogDetail

			FETCH NEXT
			FROM @Dept
			INTO @DepartmentId
				,@IsGroup
		END

		CLOSE @Dept

		DEALLOCATE @Dept

		SET @Data = @Data + '</r>'

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy, @SpName = 'EpaTargetTemplateDepartment_Assign', @ObjectId = NULL, @ObjectTypeId = 7, @Data = @Data, @OldData = @OldData, @ObjectIds = NULL

		DROP TABLE #DepartmentSetting
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Setting target template to department unsuccessfully.'
	END CATCH
END