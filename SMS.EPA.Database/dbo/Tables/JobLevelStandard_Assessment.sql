CREATE TABLE [dbo].[JobLevelStandard_Assessment] (
    [AssessmentId]      INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]        INT            NOT NULL,
    [PeriodId]          INT            NOT NULL,
    [TotalWeight]       DECIMAL (7, 2) NULL,
    [CurrentJobLevelId] INT            NULL,
    [NewJobLevelId]     INT            NULL,
    [Remark]            NVARCHAR (200) NULL,
    [StatusId]          TINYINT        NULL,
    [CreatedBy]         INT            NULL,
    [CreatedDate]       DATETIME       NULL,
    [ModifiedBy]        INT            NULL,
    [ModifiedDate]      DATETIME       NULL,
    CONSTRAINT [PK_JobLevelStandard_Assessment] PRIMARY KEY CLUSTERED ([AssessmentId] ASC)
);



