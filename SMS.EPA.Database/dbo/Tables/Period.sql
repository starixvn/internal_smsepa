CREATE TABLE [dbo].[Period] (
    [PeriodId]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (200) NOT NULL,
    [TargetTypeId]        INT            NOT NULL,
    [StartDate]           DATETIME       NULL,
    [EndDate]             DATETIME       NULL,
    [AttendanceStartDate] DATETIME       NULL,
    [AttendanceEndDate]   DATETIME       NULL,
    [IsActive]            BIT            NOT NULL,
    [Description]         NVARCHAR (MAX) NULL,
    [EvaluationTimes]     INT            NULL,
    [CreatedBy]           INT            NULL,
    [CreatedDate]         DATETIME       NULL,
    [ModifiedBy]          INT            NULL,
    [ModifiedDate]        DATETIME       NULL,
    CONSTRAINT [PK_EvaluationPeriod] PRIMARY KEY CLUSTERED ([PeriodId] ASC)
);



