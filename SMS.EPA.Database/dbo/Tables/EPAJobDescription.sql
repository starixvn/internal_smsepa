CREATE TABLE [dbo].[EPAJobDescription] (
    [EPAId]         INT            NOT NULL,
    [GroupItemId]   INT            NOT NULL,
    [GroupItemName] NVARCHAR (500) NULL,
    [ItemId]        INT            NOT NULL,
    [Name]          NVARCHAR (500) NULL,
    [DisplayOrder]  SMALLINT       NULL,
    CONSTRAINT [PK_EPAJobDescription] PRIMARY KEY CLUSTERED ([EPAId] ASC, [GroupItemId] ASC, [ItemId] ASC)
);

