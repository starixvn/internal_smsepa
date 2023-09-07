CREATE TABLE [dbo].[Unit] (
    [UnitId]       INT            NOT NULL,
    [Code]         VARCHAR (50)   NOT NULL,
    [Name]         NVARCHAR (200) NOT NULL,
    [IsActive]     BIT            NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [SyncDate]     DATETIME       NULL,
    CONSTRAINT [PK_Unit] PRIMARY KEY CLUSTERED ([UnitId] ASC)
);

