CREATE TABLE [dbo].[KPI] (
    [KPIId]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (500) NOT NULL,
    [IsActive]           BIT            NULL,
    [IsRequiredComment]  BIT            NULL,
    [HasComment]         BIT            NULL,
    [IsRequiredEvaluate] BIT            NULL,
    [Description]        NVARCHAR (MAX) NULL,
    [LogNo]              VARCHAR (20)   NULL,
    [CreatedBy]          INT            NULL,
    [CreatedDate]        DATETIME       NULL,
    [ModifiedBy]         INT            NULL,
    [ModifiedDate]       DATETIME       NULL,
    CONSTRAINT [PK_KPI] PRIMARY KEY CLUSTERED ([KPIId] ASC)
);

