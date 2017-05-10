CREATE TABLE [dbo].[StandardGrade] (
    [GradeId]    INT           NOT NULL,
    [Type]       SMALLINT      NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    [Code]       NCHAR (10)    NOT NULL,
    [LeftScore]  FLOAT (53)    NOT NULL,
    [RightScore] FLOAT (53)    NOT NULL,
    CONSTRAINT [PK_StandardGrade_tim] PRIMARY KEY CLUSTERED ([GradeId] ASC, [Type] ASC)
);



