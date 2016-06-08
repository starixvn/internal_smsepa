CREATE TABLE [dbo].[AttendanceDetailIgnored] (
    [WorkDate]     DATETIME NOT NULL,
    [EmployeeId]   INT      NOT NULL,
    [ViolationId]  INT      NOT NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_AttendanceDetailIgnored] PRIMARY KEY CLUSTERED ([WorkDate] ASC, [EmployeeId] ASC, [ViolationId] ASC)
);

