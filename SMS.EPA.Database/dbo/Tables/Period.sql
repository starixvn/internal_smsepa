CREATE TABLE [dbo].[Period] (
    [PeriodId]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (200) NOT NULL,
    [TargetTypeId]        INT            NOT NULL,
    [StartDate]           DATE           NULL,
    [EndDate]             DATE           NULL,
    [AttendanceStartDate] DATE           NULL,
    [AttendanceEndDate]   DATE           NULL,
    [IsActive]            BIT            NOT NULL,
    [Description]         NVARCHAR (MAX) NULL,
    [CreatedBy]           INT            NULL,
    [CreatedDate]         DATETIME       NULL,
    [ModifiedBy]          INT            NULL,
    [ModifiedDate]        DATETIME       NULL,
    [EvaluationTimes]     INT            NULL,
    CONSTRAINT [PK_EvaluationPeriod] PRIMARY KEY CLUSTERED ([PeriodId] ASC)
);







