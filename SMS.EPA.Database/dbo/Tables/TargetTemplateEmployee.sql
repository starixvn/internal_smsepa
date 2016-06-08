CREATE TABLE [dbo].[TargetTemplateEmployee] (
    [TargetTemplateId] INT      NOT NULL,
    [EmployeeId]       INT      NOT NULL,
    [ModifiedBy]       INT      NULL,
    [ModifiedDate]     DATETIME NULL,
    CONSTRAINT [PK_TargetTemplateEmployee] PRIMARY KEY CLUSTERED ([TargetTemplateId] ASC, [EmployeeId] ASC)
);

