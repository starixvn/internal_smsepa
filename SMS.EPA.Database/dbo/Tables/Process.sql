CREATE TABLE [dbo].[Process] (
    [ProcessId]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (50)  NOT NULL,
    [Description]         NVARCHAR (500) NULL,
    [ProcessGroupId]      INT            NULL,
    [ProcessDays]         INT            NULL,
    [Priority]            INT            NULL,
    [CreatedBy]           INT            NULL,
    [CreatedDate]         DATETIME       NULL,
    [ModifiedBy]          INT            NULL,
    [ModifiedDate]        DATETIME       NULL,
    [IsEvaluatedByLeader] BIT            NULL,
    [UpperLevel]          TINYINT        NULL,
    [IsSelfEvaluated]     BIT            NULL,
    CONSTRAINT [PK_Process] PRIMARY KEY CLUSTERED ([ProcessId] ASC)
);

