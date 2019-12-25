CREATE TABLE [dbo].[ProbationExtendedQuestion]
(
	[EPAId] INT NOT NULL , 
    [SubQuestionId] SMALLINT NOT NULL, 
    [IsAgree] BIT NULL, 
    CONSTRAINT [PK_ProbationExtendedQuestion] PRIMARY KEY ([EPAId] ASC, [SubQuestionId] ASC)
)
