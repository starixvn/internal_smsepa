CREATE TABLE [dbo].[SettingEmployeeFlow] (
    [EmployeeId]     INT      NOT NULL,
    [ProcessId]      INT      NOT NULL,
    [EvaluatorId]    INT      NOT NULL,
    [FlowTemplateId] INT      NOT NULL,
    [Priority]       TINYINT  NULL,
    [Duration]       INT      NULL,
    [HasSendMail]    BIT      NULL,
    [RemindDay]      INT      NULL,
    [ModifiedBy]     INT      NULL,
    [ModifiedDate]   DATETIME NULL,
    CONSTRAINT [PK_SettingEmployeeFlow_1] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [ProcessId] ASC, [EvaluatorId] ASC, [FlowTemplateId] ASC)
);

