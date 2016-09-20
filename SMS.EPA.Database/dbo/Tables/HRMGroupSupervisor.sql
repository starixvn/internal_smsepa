CREATE TABLE [dbo].[HRMGroupSupervisor] (
    [OwnerId]      INT        NOT NULL,
    [GroupId]      INT        NOT NULL,
    [ActionType]   TINYINT    NOT NULL,
    [RequestType]  TINYINT    NOT NULL,
    [ModifiedBy]   INT        NULL,
    [ModifiedDate] DATETIME   NULL,
    [SyncDate]     NCHAR (10) NULL,
    [IsDeleted]    BIT        NULL,
    CONSTRAINT [PK_HRMGroupSupervisor] PRIMARY KEY CLUSTERED ([OwnerId] ASC, [GroupId] ASC, [ActionType] ASC, [RequestType] ASC)
);



