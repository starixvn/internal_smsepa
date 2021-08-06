CREATE TABLE [dbo].[JobDescription_EmployeeSetting] (
    [EmployeeId]            INT      NOT NULL,
    [JobLevelId]            INT      NOT NULL,
    [JobDescription_ItemId] INT      NOT NULL,
    [DisplayOrder]          SMALLINT CONSTRAINT [DF_JobDescription_EmployeeSetting_DisplayOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_JobDescription_EmployeeSetting] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [JobLevelId] ASC, [JobDescription_ItemId] ASC)
);

