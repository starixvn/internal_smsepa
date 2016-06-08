CREATE TABLE [dbo].[EPAProcess] (
    [EPAId]         INT            NOT NULL,
    [ProcessId]     INT            NOT NULL,
    [EvaluatorId]   INT            NOT NULL,
    [Priority]      TINYINT        NOT NULL,
    [Comment]       NVARCHAR (MAX) NULL,
    [StartDate]     DATETIME       NULL,
    [EndDate]       DATETIME       NULL,
    [IsEndProcess]  BIT            NOT NULL,
    [KPICount]      INT            NULL,
    [TotalScore]    NVARCHAR (100) NULL,
    [AdjustScore]   NVARCHAR (100) NULL,
    [NoticeAtBegin] BIT            NULL,
    [RemindDate]    DATETIME       NULL,
    [Status]        TINYINT        NULL,
    [EvaluateDate]  DATETIME       NULL,
    [ModifiedBy]    INT            NULL,
    [ModifiedDate]  DATETIME       NULL,
    CONSTRAINT [PK_EPAProcess] PRIMARY KEY CLUSTERED ([EPAId] ASC, [ProcessId] ASC, [EvaluatorId] ASC)
);

