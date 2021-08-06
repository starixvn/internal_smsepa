CREATE TABLE [dbo].[JobDescription_GroupSetting] (
    [GroupId]               INT      NOT NULL,
    [JobLevelId]            INT      NOT NULL,
    [JobDescription_ItemId] INT      NOT NULL,
    [DisplayOrder]          SMALLINT CONSTRAINT [DF_JobDescription_GroupSetting_DisplayOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_JobDescription_GroupSetting] PRIMARY KEY CLUSTERED ([GroupId] ASC, [JobLevelId] ASC, [JobDescription_ItemId] ASC)
);

