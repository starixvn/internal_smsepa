CREATE TABLE [dbo].[TargetType] (
    [TargetTypeId]     TINYINT        IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (200) NOT NULL,
    [IsActive]         BIT            NOT NULL,
    [DisplayOrder]     TINYINT        NULL,
    [ScoreTypeId]      TINYINT        NULL,
    [ProcessGroupId]   INT            NULL,
    [TargetTemplateId] INT            NULL,
    [CreatedBy]        INT            NULL,
    [CreatedDate]      DATETIME       NULL,
    [ModifiedBy]       INT            NULL,
    [ModifiedDate]     DATETIME       NULL,
    [AllowMultiple]    BIT            NULL,
    CONSTRAINT [PK_TargetType] PRIMARY KEY CLUSTERED ([TargetTypeId] ASC)
);

