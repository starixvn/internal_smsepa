CREATE TABLE [dbo].[PeriodProcessSetting] (
    [PeriodId]      INT      NOT NULL,
    [ProcessId]     INT      NOT NULL,
    [Duration]      INT      NOT NULL,
    [NoticeAtBegin] BIT      NOT NULL,
    [RemindDay]     INT      NOT NULL,
    [ModifiedBy]    INT      NULL,
    [ModifiedDate]  DATETIME NULL,
    CONSTRAINT [PK_PeriodProcessSetting] PRIMARY KEY CLUSTERED ([PeriodId] ASC, [ProcessId] ASC)
);

