CREATE TABLE [dbo].[SettingDepartmentFlow] (
    [DepartmentId]   INT      NOT NULL,
    [ProcessId]      INT      NOT NULL,
    [EvaluatorId]    INT      NOT NULL,
    [IsGroup]        BIT      NOT NULL,
    [FlowTemplateId] INT      NOT NULL,
    [Priority]       TINYINT  NULL,
    [Duration]       INT      NULL,
    [HasSendMail]    BIT      NULL,
    [RemindDay]      INT      NULL,
    [ModifiedBy]     INT      NULL,
    [ModifiedDate]   DATETIME NULL,
    CONSTRAINT [PK_SettingDepartmentFlow_1] PRIMARY KEY CLUSTERED ([DepartmentId] ASC, [ProcessId] ASC, [EvaluatorId] ASC, [IsGroup] ASC, [FlowTemplateId] ASC)
);

