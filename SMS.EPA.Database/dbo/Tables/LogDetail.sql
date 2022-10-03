CREATE TABLE [dbo].[LogDetail] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [ActorId]      INT            NULL,
    [SpName]       VARCHAR (100)  NULL,
    [LogDate]      DATETIME       NULL,
    [ObjectId]     INT            NULL,
    [ObjectTypeId] INT            NULL,
    [ActionId]     INT            NULL,
    [Data]         NVARCHAR (MAX) NULL,
    [OldData]      NVARCHAR (MAX) NULL,
    [ObjectIds]    VARCHAR (MAX)  NULL,
    CONSTRAINT [PK_LogDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);



