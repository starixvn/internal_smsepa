/****** Object:  Stored Procedure dbo.FlowTemplate_GetById    Script Date: 5/16/2016 2:20:09 PM ******/
/****** Object:  Stored Procedure dbo.FlowTemplate_GetById    Script Date: 5/16/2016 2:15:31 PM ******/
/****** Object:  Stored Procedure dbo.FlowTemplate_GetById    Script Date: 5/16/2016 1:59:44 PM ******/
/****** Object:  Stored Procedure dbo.FlowTemplate_GetById    Script Date: 4/6/2016 11:27:46 AM ******/

CREATE PROCEDURE [dbo].[FlowTemplate_GetById]
    @FlowTemplateId INT
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
    SET NOCOUNT ON;
    
    SELECT ft.FlowTemplateId
        ,ft.Name
        ,ft.[Description]
        ,ft.IsActive
        ,tt.TargetTypeId
        ,tt.Name
    FROM dbo.FlowTemplate AS ft WITH (NOLOCK)
    INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) 
        ON ft.TargetTypeId = tt.TargetTypeId
	WHERE ft.FlowTemplateId = @FlowTemplateId
END