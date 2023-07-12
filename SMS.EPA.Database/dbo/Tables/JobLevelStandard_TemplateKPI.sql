CREATE TABLE [dbo].[JobLevelStandard_TemplateKPI] (
    [TemplateId]   INT            NOT NULL,
    [KPIId]        INT            NOT NULL,
    [Weight]       DECIMAL (7, 4) NULL,
    [DisplayOrder] TINYINT        NULL,
    CONSTRAINT [PK_JobLevelStandard_TemplateKPI] PRIMARY KEY CLUSTERED ([TemplateId] ASC, [KPIId] ASC)
);



