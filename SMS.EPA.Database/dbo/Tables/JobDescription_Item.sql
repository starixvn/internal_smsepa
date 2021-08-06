CREATE TABLE [dbo].[JobDescription_Item] (
    [JobDescription_ItemId]      INT            IDENTITY (1, 1) NOT NULL,
    [JobDescription_GroupItemId] INT            NOT NULL,
    [Name]                       NVARCHAR (500) NOT NULL,
    [IsActive]                   BIT            CONSTRAINT [DF_JobDescription_Item_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_JobDescription_Item] PRIMARY KEY CLUSTERED ([JobDescription_ItemId] ASC)
);

