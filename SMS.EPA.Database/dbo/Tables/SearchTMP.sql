CREATE TABLE [dbo].[SearchTMP] (
    [GradeId]    INT           IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    [Code]       NCHAR (10)    NOT NULL,
    [LeftScore]  FLOAT (53)    NOT NULL,
    [RightScore] FLOAT (53)    NOT NULL
);

