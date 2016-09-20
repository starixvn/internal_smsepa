
CREATE PROCEDURE [dbo].[FlowTemplate_Get] @TargetTypeId TINYINT = NULL
	,@IsActive BIT = NULL
AS
/*
Created: 20150804@Tim.Trinh
Task: Get all Flow Template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	SELECT ft.FlowTemplateId
		,ft.[Name]
		,ft.[Description]
		,ft.IsActive
		,tt.TargetTypeId
		,tt.[Name]
	FROM dbo.FlowTemplate AS ft WITH (NOLOCK)
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON ft.TargetTypeId = tt.TargetTypeId
	WHERE (
			@TargetTypeId IS NULL
			OR ft.TargetTypeId = @TargetTypeId
			)
		AND (
			@IsActive IS NULL
			OR ft.IsActive = @IsActive
			)
END