CREATE TABLE [dbo].[JobLevelStandard_PeriodSetting] (
    [PeriodId]  INT     NOT NULL,
    [ProcessId] TINYINT NOT NULL,
    [FromDate]  DATE    NULL,
    [ToDate]    DATE    NULL,
    CONSTRAINT [PK_JobLevelStandard_PeriodSetting] PRIMARY KEY CLUSTERED ([PeriodId] ASC, [ProcessId] ASC)
);

