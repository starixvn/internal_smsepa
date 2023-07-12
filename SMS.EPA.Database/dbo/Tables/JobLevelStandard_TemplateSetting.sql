CREATE TABLE [dbo].[JobLevelStandard_TemplateSetting] (
    [TemplateId]   INT      NOT NULL,
    [ObjectTypeId] TINYINT  NOT NULL,
    [ObjectId]     INT      NOT NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_JobLevelStandard_TemplateSetting] PRIMARY KEY CLUSTERED ([TemplateId] ASC, [ObjectTypeId] ASC, [ObjectId] ASC)
);

