CREATE TABLE [dbo].[KPIGroupHistory] (
    [KPIGroupId]   INT            NOT NULL,
    [LogNo]        VARCHAR (20)   NOT NULL,
    [Name]         NVARCHAR (500) NULL,
    [IsActive]     BIT            NULL,
    [Description]  NVARCHAR (MAX) NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_KPIGroupHistory] PRIMARY KEY CLUSTERED ([KPIGroupId] ASC, [LogNo] ASC)
);

