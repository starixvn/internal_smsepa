CREATE TABLE [dbo].[JobLevel] (
    [JobLevelId]   INT            NOT NULL,
    [Name]         NVARCHAR (150) NOT NULL,
    [IsDeleted]    BIT            NOT NULL,
    [DisplayOrder] SMALLINT       NULL,
    [ModifiedDate] DATETIME       NULL,
    [SyncDate]     DATETIME       NULL,
    CONSTRAINT [PK_JobLevel] PRIMARY KEY CLUSTERED ([JobLevelId] ASC)
);





