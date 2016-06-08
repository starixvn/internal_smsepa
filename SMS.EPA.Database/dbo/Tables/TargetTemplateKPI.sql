CREATE TABLE [dbo].[TargetTemplateKPI] (
    [TargetTemplateId]   INT        NOT NULL,
    [KPIGroupId]         INT        NOT NULL,
    [KPIId]              INT        NOT NULL,
    [Weight]             FLOAT (53) NOT NULL,
    [DisplayOrder]       INT        NULL,
    [AddedBy]            INT        NULL,
    [AddedDate]          DATETIME   NULL,
    [ModifiedBy]         INT        NULL,
    [ModifiedDate]       DATETIME   NULL,
    [HasComment]         BIT        NULL,
    [IsRequiredComment]  BIT        NULL,
    [IsRequiredEvaluate] BIT        NULL,
    CONSTRAINT [PK_TargetTemplateKPI] PRIMARY KEY CLUSTERED ([TargetTemplateId] ASC, [KPIGroupId] ASC, [KPIId] ASC)
);

