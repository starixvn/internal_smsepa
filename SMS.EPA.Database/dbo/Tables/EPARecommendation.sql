CREATE TABLE [dbo].[EPARecommendation] (
    [EPAId]             INT      NOT NULL,
    [ProbationStatus]   TINYINT  NULL,
    [Promoted]          BIT      NULL,
    [PayIncreased]      BIT      NULL,
    [JobGradeIncreased] BIT      NULL,
    [JobGradeId]        INT      NULL,
    [ModifiedBy]        INT      NULL,
    [ModifiedDate]      DATETIME NULL,
    CONSTRAINT [PK_EPARecommendation] PRIMARY KEY CLUSTERED ([EPAId] ASC)
);

