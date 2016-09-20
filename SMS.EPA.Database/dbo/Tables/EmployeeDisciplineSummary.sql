CREATE TABLE [dbo].[EmployeeDisciplineSummary] (
    [EmployeeId]          INT            NOT NULL,
    [Period]              DATE           NOT NULL,
    [ChangeShift]         DECIMAL (5, 2) NULL,
    [MissingCheckInOut]   DECIMAL (5, 2) NULL,
    [LateIn]              DECIMAL (5, 2) NULL,
    [EarlyOut]            DECIMAL (5, 2) NULL,
    [Absent]              DECIMAL (5, 2) NULL,
    [UnpaidLeave]         DECIMAL (5, 2) NULL,
    [WorkingUndertime]    DECIMAL (5, 2) NULL,
    [UnpaidLeaveAbsent]   DECIMAL (5, 2) NULL,
    [OtherDisciplines]    DECIMAL (5, 2) NULL,
    [SystemSubtractScore] DECIMAL (5, 2) NULL,
    [ModifiedDate]        DATETIME       NULL,
    [SyncDate]            DATETIME       NULL,
    CONSTRAINT [PK_EmployeeDisciplineSummary] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [Period] ASC)
);

