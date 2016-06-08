CREATE TABLE [dbo].[Group] (
    [GroupId]           INT            NOT NULL,
    [Name]              NVARCHAR (150) NOT NULL,
    [Code]              VARCHAR (20)   NULL,
    [DepartmentId]      INT            NULL,
    [LeaderId]          INT            NULL,
    [AssistantLeaderId] INT            NULL,
    [Rank]              INT            NULL,
    [IsDeleted]         BIT            NOT NULL,
    [lft]               INT            NOT NULL,
    [rgt]               INT            NOT NULL,
    [ModifiedDate]      DATETIME       NULL,
    [SyncDate]          DATETIME       NULL,
    CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED ([GroupId] ASC)
);

