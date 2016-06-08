CREATE TABLE [dbo].[PeriodExcludedEmployee] (
    [PeriodId]     INT            NOT NULL,
    [EmployeeId]   INT            NOT NULL,
    [Description]  NVARCHAR (MAX) NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_PeriodExcludedEmployee] PRIMARY KEY CLUSTERED ([PeriodId] ASC, [EmployeeId] ASC)
);

