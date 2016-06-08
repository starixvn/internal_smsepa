CREATE TABLE [dbo].[EPASummaryImport] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Code]             VARCHAR (20)   NOT NULL,
    [NickName]         NVARCHAR (20)  NOT NULL,
    [FullName]         NVARCHAR (50)  NOT NULL,
    [PeriodName]       NVARCHAR (200) NULL,
    [PeriodDate]       DATETIME       NULL,
    [FinalScore]       NVARCHAR (100) NULL,
    [OriginFinalScore] FLOAT (53)     NULL,
    [PeriodId]         INT            NULL,
    [Grade]            NVARCHAR (100) NULL,
    CONSTRAINT [PK_EPASummaryImport] PRIMARY KEY CLUSTERED ([Id] ASC)
);

