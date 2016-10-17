CREATE TABLE [dbo].[JobGrade] (
    [JobGradeId]   INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (100) NULL,
    [IsActive]     BIT            NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_JobGrade] PRIMARY KEY CLUSTERED ([JobGradeId] ASC)
);

