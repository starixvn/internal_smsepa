CREATE TABLE [dbo].[JobLevelStandard_PeriodJobLevelSetting] (
    [PeriodId]     INT            NOT NULL,
    [SectionId]    INT            NOT NULL,
    [JobLevelId]   INT            NOT NULL,
    [MinWeight]    DECIMAL (5, 2) NULL,
    [MaxWeight]    DECIMAL (5, 2) NULL,
    [CreatedBy]    INT            NULL,
    [CreatedDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_JobLevelStandard_PeriodJobLevelSetting] PRIMARY KEY CLUSTERED ([PeriodId] ASC, [SectionId] ASC, [JobLevelId] ASC)
);

