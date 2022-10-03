CREATE TABLE [dbo].[JobLevelStandard_PeriodProcessSetting] (
    [PeriodId]     INT      NOT NULL,
    [ProcessId]    TINYINT  NOT NULL,
    [EmployeeId]   INT      NOT NULL,
    [EvaluatorId]  INT      NOT NULL,
    [FromDate]     DATE     NULL,
    [ToDate]       DATE     NULL,
    [CreatedBy]    INT      NULL,
    [CreatedDate]  DATETIME NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_JobLevelStandard_PeriodProcessSetting] PRIMARY KEY CLUSTERED ([PeriodId] ASC, [ProcessId] ASC, [EmployeeId] ASC, [EvaluatorId] ASC)
);

