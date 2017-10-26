CREATE TABLE [dbo].[PeriodFlowSetting] (
    [PeriodId]     INT      NOT NULL,
    [EmployeeId]   INT      NOT NULL,
    [ProcessId]    INT      NOT NULL,
    [EvaluatorId]  INT      NOT NULL,
    [Priority]     INT      NULL,
    [Duration]     INT      NULL,
    [StartDate]    DATE     NULL,
    [EndDate]      DATE     NULL,
    [HasSendMail]  BIT      NULL,
    [RemindDay]    INT      NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_PeriodFlowSetting] PRIMARY KEY CLUSTERED ([PeriodId] ASC, [EmployeeId] ASC, [ProcessId] ASC, [EvaluatorId] ASC)
);



