CREATE TABLE [dbo].[TargetTemplate] (
    [TargetTemplateId]  INT            IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (200) NOT NULL,
    [ScoreRatingTypeId] INT            NOT NULL,
    [Description]       NVARCHAR (MAX) NULL,
    [TargetTypeId]      TINYINT        NULL,
    [IsActive]          BIT            NULL,
    [CreatedBy]         INT            NULL,
    [CreatedDate]       DATETIME       NULL,
    [ModifiedBy]        INT            NULL,
    [ModifiedDate]      DATETIME       NULL,
    [RatingTypeId]      INT            NULL,
    CONSTRAINT [PK_TargetTemplate] PRIMARY KEY CLUSTERED ([TargetTemplateId] ASC)
);

