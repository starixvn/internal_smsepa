CREATE TABLE [dbo].[DisciplineSummary] (
    [EmployeeId]          INT            NOT NULL,
    [WorkDate]            DATE           NOT NULL,
    [ChangeShift]         DECIMAL (5, 2) NULL,
    [MissingCheckInOut]   DECIMAL (5, 2) NULL,
    [LateIn]              DECIMAL (5, 2) NULL,
    [EarlyOut]            DECIMAL (5, 2) NULL,
    [Absent]              DECIMAL (5, 2) NULL,
    [UnpaidLeave]         DECIMAL (5, 2) NULL,
    [WorkingUndertime]    DECIMAL (5, 2) NULL,
    [UnpaidLeaveAbsent]   DECIMAL (5, 2) NULL,
    [UnplannedLeave]      DECIMAL (5, 2) NULL,
    [WeekendLeave]        DECIMAL (5, 2) NULL,
    [OtherDisciplines]    DECIMAL (5, 2) NULL,
    [SystemSubtractScore] DECIMAL (5, 2) NULL,
    [ModifiedDate]        DATETIME       NULL,
    [IsDeleted]           BIT            NULL,
    [SyncDate]            DATETIME       NULL,
    [SevenDaysLeave]      DECIMAL (5, 2) NULL,
    CONSTRAINT [PK_DisciplineSummary] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [WorkDate] ASC)
);



