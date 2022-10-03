CREATE TABLE [dbo].[JobLevelStandard_JobLevelSetting] (
    [SectionId]  INT            NOT NULL,
    [JobLevelId] INT            NOT NULL,
    [MinWeight]  DECIMAL (5, 2) NULL,
    [MaxWeight]  DECIMAL (5, 2) NULL,
    CONSTRAINT [PK_JobLevelStandard_DefaultWeight] PRIMARY KEY CLUSTERED ([SectionId] ASC, [JobLevelId] ASC)
);

