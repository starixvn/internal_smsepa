CREATE TABLE [dbo].[JobDescription_GroupItem] (
    [JobDescription_GroupItemId] INT            IDENTITY (1, 1) NOT NULL,
    [GroupId]                    INT            NOT NULL,
    [Name]                       NVARCHAR (500) NOT NULL,
    [IsActive]                   BIT            CONSTRAINT [DF_JobDesciption_KPIGroup_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_JobDescription_GroupItem] PRIMARY KEY CLUSTERED ([JobDescription_GroupItemId] ASC)
);

