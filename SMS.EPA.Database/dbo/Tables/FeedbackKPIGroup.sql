CREATE TABLE [dbo].[FeedbackKPIGroup] (
    [FeedbackId]   BIGINT       NOT NULL,
    [KPIGroupId]   INT          NOT NULL,
    [LogNo]        VARCHAR (20) NULL,
    [IsSpreadKPI]  BIT          CONSTRAINT [DF_FeedbackKPIGroup_IsSpreadKPI] DEFAULT ((0)) NOT NULL,
    [Weight]       FLOAT (53)   CONSTRAINT [DF_FeedbackKPIGroup_Weight] DEFAULT ((0)) NOT NULL,
    [DisplayOrder] SMALLINT     CONSTRAINT [DF_FeedbackKPIGroup_DisplayOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_FeedbackKPIGroup] PRIMARY KEY CLUSTERED ([FeedbackId] ASC, [KPIGroupId] ASC)
);



