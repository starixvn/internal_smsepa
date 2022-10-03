CREATE TABLE [dbo].[JobLevelStandard_KPI] (
    [KPIId]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (200) NULL,
    [IsActive]     BIT            NULL,
    [LogNo]        VARCHAR (20)   NULL,
    [CreatedBy]    INT            NULL,
    [CreatedDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_JobLevelStandard_KPI] PRIMARY KEY CLUSTERED ([KPIId] ASC)
);

