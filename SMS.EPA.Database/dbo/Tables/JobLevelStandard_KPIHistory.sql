CREATE TABLE [dbo].[JobLevelStandard_KPIHistory] (
    [KPIId]              INT            NOT NULL,
    [LogNo]              VARCHAR (20)   NOT NULL,
    [Name]               NVARCHAR (500) NULL,
    [IsActive]           BIT            NULL,
    [ModifiedBy]         INT            NULL,
    [ModifiedDate]       DATETIME       NULL,
    CONSTRAINT [PK_JobLevelStandard_KPIHistory] PRIMARY KEY CLUSTERED ([KPIId] ASC, [LogNo] ASC)
);

