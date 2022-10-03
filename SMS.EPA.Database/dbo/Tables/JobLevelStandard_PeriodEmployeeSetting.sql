CREATE TABLE [dbo].[JobLevelStandard_PeriodEmployeeSetting] (
    [PeriodId]     INT      NOT NULL,
    [EmployeeId]   INT      NOT NULL,
    [TemplateId]   INT      NULL,
    [IsExcluded]   BIT      NULL,
    [CreatedBy]    INT      NULL,
    [CreatedDate]  DATETIME NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATE     NULL,
    CONSTRAINT [PK_JobLevelStandard_PeriodEmployeeSetting] PRIMARY KEY CLUSTERED ([PeriodId] ASC, [EmployeeId] ASC)
);

