CREATE TABLE [dbo].[EPARecommendation] (
    [EPAId]               INT      NOT NULL,
    [ProcessId]           TINYINT  NOT NULL,
    [ProbationStatus]     TINYINT  NULL,
    [Promoted]            BIT      NULL,
    [PayIncreased]        BIT      NULL,
    [JobLevelIncreased]   BIT      NULL,
    [JobLevelId]          INT      NULL,
    [ModifiedBy]          INT      NULL,
    [ModifiedDate]        DATETIME NULL,
    [PromotedBy]          INT      NULL,
    [PayIncreasedBy]      INT      NULL,
    [JobLevelIncreasedBy] INT      NULL,
    CONSTRAINT [PK_EPARecommendation] PRIMARY KEY CLUSTERED ([EPAId] ASC, [ProcessId] ASC)
);





