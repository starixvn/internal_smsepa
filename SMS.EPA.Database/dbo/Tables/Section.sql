CREATE TABLE [dbo].[Section] (
    [SectionId]    INT            NOT NULL,
    [Code]         VARCHAR (50)   NOT NULL,
    [Name]         NVARCHAR (200) NOT NULL,
    [DepartmentId] INT            NULL,
    [IsActive]     BIT            NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [SyncDate]     DATETIME       NULL,
    CONSTRAINT [PK_Section] PRIMARY KEY CLUSTERED ([SectionId] ASC)
);

