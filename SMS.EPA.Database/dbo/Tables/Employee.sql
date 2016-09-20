CREATE TABLE [dbo].[Employee] (
    [EmployeeId]     INT            NOT NULL,
    [DomainUsername] NVARCHAR (50)  NOT NULL,
    [Code]           NVARCHAR (20)  NOT NULL,
    [NickName]       NVARCHAR (20)  NOT NULL,
    [FullName]       NVARCHAR (50)  NOT NULL,
    [GroupId]        INT            NULL,
    [Phone]          VARCHAR (50)   NULL,
    [CardNumber]     VARCHAR (20)   NULL,
    [Enabled]        BIT            NULL,
    [IsDeleted]      BIT            NULL,
    [IsSpecialUser]  BIT            NULL,
    [Password]       NVARCHAR (100) NULL,
    [JoinedDate]     DATETIME       NULL,
    [JobTitle]       INT            NULL,
    [ModifiedBy]     INT            NULL,
    [ModifiedDate]   DATETIME       NULL,
    [CreatedBy]      INT            NULL,
    [CreatedDate]    DATETIME       NULL,
    [Email]          VARCHAR (50)   NULL,
    [SyncDate]       DATETIME       NULL,
    [IsLocal]        BIT            NULL,
    [HasMonthlyEPA]  BIT            NULL,
    [ResignedDate]   DATETIME       NULL,
    [ConfirmedDate]  DATETIME       NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);



