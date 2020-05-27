CREATE TABLE [dbo].[FeedbackKPI] (
    [FeedbackId]         BIGINT         NOT NULL,
    [KPIGroupId]         INT            NOT NULL,
    [KPIId]              INT            NOT NULL,
    [LogNo]              VARCHAR (20)   NULL,
    [HasComment]         BIT            CONSTRAINT [DF_FeedbackKPI_HasComment] DEFAULT ((0)) NOT NULL,
    [IsRequiredComment]  BIT            CONSTRAINT [DF_FeedbackKPI_IsRequiredComment] DEFAULT ((0)) NOT NULL,
    [IsRequiredEvaluate] BIT            CONSTRAINT [DF_FeedbackKPI_IsRequiredEvaluate] DEFAULT ((0)) NOT NULL,
    [Weight]             FLOAT (53)     CONSTRAINT [DF_FeedbackKPI_Weight] DEFAULT ((0)) NOT NULL,
    [Score]              NVARCHAR (100) NULL,
    [Rate]               NVARCHAR (100) NULL,
    [Comment]            NVARCHAR (MAX) NULL,
    [DisplayOrder]       SMALLINT       CONSTRAINT [DF_FeedbackKPI_DisplayOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_FeedbackKPI] PRIMARY KEY CLUSTERED ([FeedbackId] ASC, [KPIGroupId] ASC, [KPIId] ASC)
);



