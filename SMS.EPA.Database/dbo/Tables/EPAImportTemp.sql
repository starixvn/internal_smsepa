CREATE TABLE [dbo].[EPAImportTemp] (
    [Code]             VARCHAR (20)   NULL,
    [NickName]         NVARCHAR (20)  NULL,
    [FullName]         NVARCHAR (50)  NULL,
    [PeriodName]       NVARCHAR (200) NULL,
    [PeriodDate]       DATETIME       NULL,
    [FinalScore]       NVARCHAR (100) NULL,
    [OriginFinalScore] FLOAT (53)     NULL,
    [PeriodId]         INT            NULL,
    [Grade]            NVARCHAR (100) NULL
);

