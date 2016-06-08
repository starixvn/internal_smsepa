CREATE TABLE [dbo].[AttendanceViolation] (
    [Id]    INT        IDENTITY (1, 1) NOT NULL,
    [Name]  NCHAR (50) NOT NULL,
    [Score] FLOAT (53) NOT NULL,
    CONSTRAINT [PK_AttendanceViolation] PRIMARY KEY CLUSTERED ([Id] ASC)
);

