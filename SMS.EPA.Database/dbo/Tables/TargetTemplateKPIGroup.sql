CREATE TABLE [dbo].[TargetTemplateKPIGroup] (
    [TargetTemplateId] INT        NOT NULL,
    [KPIGroupId]       INT        NOT NULL,
    [Weight]           FLOAT (53) NOT NULL,
    [DisplayOrder]     INT        NULL,
    [IsSpreadKPI]      BIT        NULL,
    [AddedBy]          INT        NULL,
    [AddedDate]        DATETIME   NULL,
    [ModifiedBy]       INT        NULL,
    [ModifiedDate]     DATETIME   NULL,
    CONSTRAINT [PK_TargetTemplateKPIGroup] PRIMARY KEY CLUSTERED ([TargetTemplateId] ASC, [KPIGroupId] ASC)
);

