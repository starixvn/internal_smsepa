CREATE TABLE [dbo].[JobLevelStandard_Template] (
    [TemplateId]   INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (200) NULL,
    [IsActive]     BIT            NULL,
    [CreatedBy]    INT            NULL,
    [CreatedDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_JobLevelStandard_Template] PRIMARY KEY CLUSTERED ([TemplateId] ASC)
);

