CREATE TABLE [dbo].[JobLevelStandard_KPILog] (
    [KPIId]    INT            NOT NULL,
    [LogNo]    VARCHAR (20)   NOT NULL,
    [Name]     NVARCHAR (200) NULL,
    [IsActive] BIT            NULL,
    CONSTRAINT [PK_JobLevelStandard_KPILog] PRIMARY KEY CLUSTERED ([KPIId] ASC, [LogNo] ASC)
);

