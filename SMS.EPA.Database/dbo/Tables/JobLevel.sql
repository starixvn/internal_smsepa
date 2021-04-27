CREATE TABLE [dbo].[JobLevel] (
    [JobLevelId]   INT            NOT NULL,
    [Name]         NVARCHAR (150) NOT NULL,
    [IsActive]     BIT            CONSTRAINT [DF_JobLevel_IsActive] DEFAULT ((1)) NOT NULL,
    [ModifiedDate] DATETIME       NULL,
    [SyncDate]     DATETIME       NULL,
    CONSTRAINT [PK_JobLevel] PRIMARY KEY CLUSTERED ([JobLevelId] ASC)
);

