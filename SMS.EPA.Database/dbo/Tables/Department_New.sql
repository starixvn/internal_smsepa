CREATE TABLE [dbo].[Department_New] (
    [DepartmentId] INT            NOT NULL,
    [Code]         VARCHAR (50)   NOT NULL,
    [Name]         NVARCHAR (200) NOT NULL,
    [UnitId]       INT            NULL,
    [IsActive]     BIT            NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [SyncDate]     DATETIME       NULL,
    CONSTRAINT [PK_Department_New] PRIMARY KEY CLUSTERED ([DepartmentId] ASC)
);

