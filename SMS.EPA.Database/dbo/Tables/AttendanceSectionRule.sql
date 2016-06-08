CREATE TABLE [dbo].[AttendanceSectionRule] (
    [GroupId]     INT NOT NULL,
    [ViolationId] INT NOT NULL,
    [IsLocal]     BIT NOT NULL,
    [Value]       BIT NOT NULL,
    CONSTRAINT [PK_AttendanceSectionRule] PRIMARY KEY CLUSTERED ([GroupId] ASC, [ViolationId] ASC, [IsLocal] ASC)
);

