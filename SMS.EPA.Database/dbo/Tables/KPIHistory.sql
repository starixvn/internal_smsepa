CREATE TABLE [dbo].[KPIHistory] (
    [KPIId]              INT            NOT NULL,
    [LogNo]              VARCHAR (20)   NOT NULL,
    [Name]               NVARCHAR (500) NULL,
    [Description]        NVARCHAR (MAX) NULL,
    [IsActive]           BIT            NULL,
    [HasComment]         BIT            NULL,
    [IsRequiredComment]  BIT            NULL,
    [IsRequiredEvaluate] BIT            NULL,
    [ModifiedBy]         INT            NULL,
    [ModifiedDate]       DATETIME       NULL,
    CONSTRAINT [PK_KPIHistory] PRIMARY KEY CLUSTERED ([KPIId] ASC, [LogNo] ASC)
);

