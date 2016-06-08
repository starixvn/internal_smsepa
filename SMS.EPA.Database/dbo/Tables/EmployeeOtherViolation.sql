CREATE TABLE [dbo].[EmployeeOtherViolation] (
    [WorkDate]     DATETIME       NOT NULL,
    [EmpId]        INT            NOT NULL,
    [Name]         NVARCHAR (50)  NOT NULL,
    [Score]        FLOAT (53)     NOT NULL,
    [Comment]      NVARCHAR (500) NULL,
    [CreatedBy]    INT            NULL,
    [CreatedDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_EmployeeOtherViolation] PRIMARY KEY CLUSTERED ([WorkDate] ASC, [EmpId] ASC, [Name] ASC)
);

