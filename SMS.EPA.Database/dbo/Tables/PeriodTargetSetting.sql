CREATE TABLE [dbo].[PeriodTargetSetting] (
    [PeriodId]         INT      NOT NULL,
    [EmployeeId]       INT      NOT NULL,
    [TargetTemplateId] INT      NOT NULL,
    [ModifiedBy]       INT      NULL,
    [ModifiedDate]     DATETIME NULL,
    CONSTRAINT [PK_PeriodTargetSetting] PRIMARY KEY CLUSTERED ([PeriodId] ASC, [EmployeeId] ASC, [TargetTemplateId] ASC)
);

