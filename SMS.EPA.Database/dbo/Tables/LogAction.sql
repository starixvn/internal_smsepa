CREATE TABLE [dbo].[LogAction] (
    [Id]           INT          IDENTITY (1, 1) NOT NULL,
    [ActionName]   VARCHAR (50) NULL,
    [ActionTypeId] INT          NULL,
    [SpName]       VARCHAR (50) NULL,
    CONSTRAINT [PK_LogAction] PRIMARY KEY CLUSTERED ([Id] ASC)
);

