CREATE TABLE [dbo].[HRMEmployeeSupervisor] (
    [OwnerId]      INT          NOT NULL,
    [EmpId]        INT          NOT NULL,
    [ActionType]   TINYINT      NOT NULL,
    [RequestType]  TINYINT      NOT NULL,
    [ModifiedBy]   INT          NULL,
    [ModifiedDate] DATETIME     NULL,
    [OrderNo]      SMALLINT     NULL,
    [OrderCode]    VARCHAR (10) NULL,
    [SyncDate]     DATETIME     NULL,
    CONSTRAINT [PK_HRMEmployeeSupervisor] PRIMARY KEY CLUSTERED ([OwnerId] ASC, [EmpId] ASC, [ActionType] ASC, [RequestType] ASC)
);





