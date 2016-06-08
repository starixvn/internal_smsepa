CREATE TABLE [dbo].[HRMJobTitle] (
    [Id]       INT            NOT NULL,
    [LookupId] INT            NOT NULL,
    [Code]     VARCHAR (20)   NULL,
    [Name]     NVARCHAR (100) NULL,
    [Value]    VARCHAR (500)  NULL,
    [SyncDate] DATETIME       NULL,
    CONSTRAINT [PK_JobTitle] PRIMARY KEY CLUSTERED ([Id] ASC)
);

