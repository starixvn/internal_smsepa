CREATE TABLE [dbo].[Department] (
    [DepartmentId]    INT            NOT NULL,
    [Code]            VARCHAR (50)   NOT NULL,
    [Name]            NVARCHAR (200) NOT NULL,
    [ParentId]        INT            NULL,
    [ManagerId]       INT            NULL,
    [DepartmentLevel] INT            NULL,
    [IsActive]        BIT            NULL,
    [CreatedBy]       INT            NULL,
    [CreatedDate]     DATETIME       NULL,
    [ModifiedBy]      INT            NULL,
    [ModifiedDate]    DATETIME       NULL,
    [lft]             INT            NULL,
    [rgt]             INT            NULL,
    [SyncDate]        DATETIME       NULL,
    CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED ([DepartmentId] ASC)
);

