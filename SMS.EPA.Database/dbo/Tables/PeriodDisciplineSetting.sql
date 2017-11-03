CREATE TABLE [dbo].[PeriodDisciplineSetting] (
    [PeriodId]   INT  NOT NULL,
    [EmployeeId] INT  NOT NULL,
    [StartDate]  DATE NOT NULL,
    [EndDate]    DATE NOT NULL,
    CONSTRAINT [PK_PeriodDisciplineSetting] PRIMARY KEY CLUSTERED ([PeriodId] ASC, [EmployeeId] ASC)
);

