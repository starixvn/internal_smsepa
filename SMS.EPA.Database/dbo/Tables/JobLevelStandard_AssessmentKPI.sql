CREATE TABLE [dbo].[JobLevelStandard_AssessmentKPI] (
    [AssessmentId] INT            NOT NULL,
    [KPIId]        INT            NOT NULL,
    [Name]         NVARCHAR (200) NULL,
    [Level]        SMALLINT       NULL,
    [Weight]       DECIMAL (5, 2) NULL,
    [DisplayOrder] TINYINT        NULL,
    [CreatedBy]    INT            NULL,
    [CreatedDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_JobLevelStandard_AssessmentKPI] PRIMARY KEY CLUSTERED ([AssessmentId] ASC, [KPIId] ASC)
);

