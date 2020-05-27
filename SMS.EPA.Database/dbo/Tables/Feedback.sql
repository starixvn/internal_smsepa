CREATE TABLE [dbo].[Feedback] (
    [FeedbackId]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [EmployeeId]       INT            NOT NULL,
    [EvaluatorId]      INT            NOT NULL,
    [PeriodId]         INT            NOT NULL,
    [TargetTemplateId] INT            NOT NULL,
    [FromDate]         DATE           NOT NULL,
    [ToDate]           DATE           NOT NULL,
    [FinalScore]       NVARCHAR (100) NULL,
    [Comment]          NVARCHAR (MAX) NULL,
    [EvaluatedDate]    DATETIME       NULL,
    CONSTRAINT [PK_Feedback] PRIMARY KEY CLUSTERED ([FeedbackId] ASC)
);



