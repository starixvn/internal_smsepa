CREATE TABLE [dbo].[EPA] (
    [EPAId]              INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]         INT            NOT NULL,
    [PeriodId]           INT            NOT NULL,
    [Status]             TINYINT        NULL,
    [DisciplineScore]    FLOAT (53)     NULL,
    [FinalScore]         NVARCHAR (100) NULL,
    [Grade]              NVARCHAR (100) NULL,
    [ModifiedBy]         INT            NULL,
    [ModifiedDate]       DATETIME       NULL,
    [ScoreRatingTypeId]  INT            NULL,
    [RatingTypeId]       INT            NULL,
    [HasMonthlyEPA]      BIT            NULL,
    [OldDisciplineScore] FLOAT (53)     NULL,
    [OldFinalScore]      NVARCHAR (100) NULL,
    [OldGrade]           NVARCHAR (100) NULL,
    CONSTRAINT [PK_EPA_1] PRIMARY KEY CLUSTERED ([EPAId] ASC)
);







