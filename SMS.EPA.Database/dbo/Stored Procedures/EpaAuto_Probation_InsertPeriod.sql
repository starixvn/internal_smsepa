
CREATE PROCEDURE [dbo].[EpaAuto_Probation_InsertPeriod] @EvaluationTimes INT
AS
/*
Created: 20161031@Tim.Trinh
Task: Insert probation period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @PeriodId INT
		,@Name NVARCHAR(200)

	SELECT @Name = Ordinal + ' Probation'
	FROM dbo.OrdinalNumbers
	WHERE Number = @EvaluationTimes

	IF EXISTS (
			SELECT 1
			FROM dbo.Period WITH (NOLOCK)
			WHERE (
					[Name] = @Name
					OR EvaluationTimes = @EvaluationTimes
					)
				AND TargetTypeId = 1 -- probation
			)
	BEGIN
		RETURN
	END

	INSERT INTO dbo.Period (
		[Name]
		,TargetTypeId
		,IsActive
		,EvaluationTimes
		,CreatedDate
		)
	VALUES (
		@Name
		,1 -- probation
		,1
		,@EvaluationTimes
		,GETDATE()
		)

	SET @PeriodId = SCOPE_IDENTITY()

	INSERT INTO dbo.PeriodProcessSetting (
		PeriodId
		,ProcessId
		,[Duration]
		,NoticeAtBegin
		,RemindDay
		)
	SELECT @PeriodId
		,ProcessId
		,[Duration]
		,NoticeAtBegin
		,RemindDay
	FROM dbo.EvaluationGlobalSetting WITH (NOLOCK)
	WHERE TargetTypeId = 1 -- probation
END