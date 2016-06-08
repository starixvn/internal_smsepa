CREATE TABLE [dbo].[FlowTemplate] (
    [FlowTemplateId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (200) NOT NULL,
    [TargetTypeId]   INT            NOT NULL,
    [Description]    NVARCHAR (500) NULL,
    [IsActive]       BIT            NULL,
    [CreatedBy]      INT            NULL,
    [CreatedDate]    DATETIME       NULL,
    [ModifiedBy]     INT            NULL,
    [ModifiedDate]   DATETIME       NULL,
    CONSTRAINT [PK_Flow] PRIMARY KEY CLUSTERED ([FlowTemplateId] ASC)
);

