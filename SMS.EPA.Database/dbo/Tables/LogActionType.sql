CREATE TABLE [dbo].[LogActionType] (
    [Id]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (50) NULL,
    CONSTRAINT [PK_LogActionType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

