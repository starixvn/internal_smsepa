
CREATE PROCEDURE [dbo].[EpaTargetTemplateEmployee_Assign] @TargetTemplateId INT
	,@EmployeeIds VARCHAR(MAX)
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Assign target template to employee.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
	@Employees = '1,2,3'
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #TargetEmployee (
		[TargetTemplateId] INT
		,[EmployeeId] INT
		)

	IF ISNULL(@EmployeeIds, '') <> ''
	BEGIN
		DECLARE @TargetTypeId INT
			,@AllowMultiple BIT

		SELECT @TargetTypeId = tarTemp.[TargetTypeId]
		FROM [dbo].[TargetTemplate] AS tarTemp WITH (NOLOCK)
			INNER JOIN [dbo].[TargetType] AS tar WITH (NOLOCK) ON tarTemp.[TargetTypeId] = tar.[TargetTypeId]
		WHERE tarTemp.[TargetTemplateId] = @TargetTemplateId

		SELECT @AllowMultiple = tar.[AllowMultiple]
		FROM [dbo].[TargetTemplate] AS tarTemp WITH (NOLOCK)
			INNER JOIN [dbo].[TargetType] AS tar WITH (NOLOCK) ON tarTemp.[TargetTypeId] = tar.[TargetTypeId]
		WHERE tarTemp.[TargetTemplateId] = @TargetTemplateId

		INSERT INTO #TargetEmployee (
			[TargetTemplateId]
			,[EmployeeId]
			)
		SELECT DISTINCT @TargetTemplateId
			,val
		FROM fnc_strSplit(@EmployeeIds, ',')

		DECLARE @OldData NVARCHAR(MAX)
			,@EmployeeId INT
			,@EmpCursor CURSOR
			,@EmpLogDetail NVARCHAR(MAX)

		SET @OldData = '<r>'
		SET @EmpCursor = CURSOR
		FOR

		SELECT [EmployeeId]
		FROM #TargetEmployee

		OPEN @EmpCursor

		FETCH NEXT
		FROM @EmpCursor
		INTO @EmployeeId

		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @EmpLogDetail = ''

			EXEC @EmpLogDetail = [dbo].[fncGetDataForWriteLog_TargetTemplateSetting_Employee] @EmployeeId = @EmployeeId

			SET @OldData = @OldData + @EMpLogDetail

			FETCH NEXT
			FROM @EmpCursor
			INTO @EmployeeId
		END

		CLOSE @EmpCursor

		DEALLOCATE @EmpCursor

		SET @OldData = @OldData + '</r>'

		BEGIN TRY
			BEGIN TRANSACTION

			IF @AllowMultiple = 1
			BEGIN
				DELETE
				FROM [dbo].[TargetTemplateEmployee]
				WHERE [EmployeeId] IN (
						SELECT DISTINCT [EmployeeId]
						FROM #TargetEmployee WITH (NOLOCK)
						)
					AND [TargetTemplateId] = @TargetTemplateId
			END

			IF ISNULL(@AllowMultiple, '') = ''
			BEGIN
				DELETE
				FROM [dbo].[TargetTemplateEmployee]
				WHERE [EmployeeId] IN (
						SELECT DISTINCT [EmployeeId]
						FROM #TargetEmployee WITH (NOLOCK)
						)
					AND [TargetTemplateId] IN (
						SELECT tarTemp.[TargetTemplateId]
						FROM [dbo].[TargetTemplate] AS tarTemp
							INNER JOIN [dbo].[TargetType] AS tar WITH (NOLOCK) ON tarTemp.[TargetTypeId] = tar.[TargetTypeId]
						WHERE tar.[TargetTypeId] = @TargetTypeId
						)
			END

			INSERT INTO [dbo].[TargetTemplateEmployee] (
				[TargetTemplateId]
				,[EmployeeId]
				,[ModifiedBy]
				,[ModifiedDate]
				)
			SELECT [TargetTemplateId]
				,[EmployeeId]
				,@ModifiedBy
				,GETDATE()
			FROM #TargetEmployee

			DECLARE @Data NVARCHAR(MAX)

			SET @Data = '<r>'
			SET @EmpCursor = CURSOR
			FOR

			SELECT [EmployeeId]
			FROM #TargetEmployee

			OPEN @EmpCursor

			FETCH NEXT
			FROM @EmpCursor
			INTO @EmployeeId

			WHILE @@FETCH_STATUS = 0
			BEGIN
				SET @EmpLogDetail = ''

				EXEC @EmpLogDetail = [dbo].[fncGetDataForWriteLog_TargetTemplateSetting_Employee] @EmployeeId = @EmployeeId

				SET @Data = @Data + @EMpLogDetail

				FETCH NEXT
				FROM @EmpCursor
				INTO @EmployeeId
			END

			CLOSE @EmpCursor

			DEALLOCATE @EmpCursor

			SET @Data = @Data + '</r>'

			EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
				,@SpName = 'EpaTargetTemplateEmployee_Assign'
				,@ObjectId = NULL
				,@ObjectTypeId = 7
				,@Data = @Data
				,@OldData = @OldData
				,@ObjectIds = NULL

			COMMIT TRANSACTION
		END TRY

		BEGIN CATCH
			ROLLBACK TRANSACTION

			SELECT @ErrCode = 1
				,@ErrMsg = N'Set target template to employee unsuccessfully.'
		END CATCH
	END

	DROP TABLE #TargetEmployee
END