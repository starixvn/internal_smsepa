CREATE TABLE [dbo].[StandardGrade] (
    [GradeId]    INT           NOT NULL,
    [Type]       SMALLINT      NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    [Code]       NVARCHAR (10) NOT NULL,
    [LeftScore]  FLOAT (53)    NOT NULL,
    [RightScore] FLOAT (53)    NOT NULL,
    CONSTRAINT [PK_StandardGrade] PRIMARY KEY CLUSTERED ([GradeId] ASC, [Type] ASC)
);





