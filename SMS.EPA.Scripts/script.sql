ALTER TABLE dbo.Period
ALTER COLUMN StartDate DATE NULL
GO
ALTER TABLE dbo.Period
ALTER COLUMN EndDate DATE NULL
GO
ALTER TABLE dbo.Period
ALTER COLUMN AttendanceStartDate DATE NULL
GO
ALTER TABLE dbo.Period
ALTER COLUMN AttendanceEndDate DATE NULL
GO
ALTER TABLE dbo.PeriodFlowSetting
ALTER COLUMN StartDate DATE NULL
GO
ALTER TABLE dbo.PeriodFlowSetting
ALTER COLUMN EndDate DATE NULL
GO
ALTER TABLE dbo.EPAProcess
ALTER COLUMN StartDate DATE NULL
GO
ALTER TABLE dbo.EPAProcess
ALTER COLUMN EndDate DATE NULL
GO
ALTER TABLE dbo.EPAProcess
ALTER COLUMN RemindDate DATE NULL
GO
------update data------
UPDATE dbo.Menu WITH (ROWLOCK, UPDLOCK)
SET Url = '/DisciplineSummary/Index'
WHERE [Name] = 'Discipline Summary By Period'
GO
UPDATE dbo.Menu WITH (ROWLOCK, UPDLOCK)
SET IsActive = 0
WHERE [Name] = 'Discipline Detail'
GO
------drop sp------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EPA_GetNumberOfRow]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EPA_GetNumberOfRow]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EPA_GetOtherViolations]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EPA_GetOtherViolations]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OtherViolationType_Get]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[OtherViolationType_Get]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeDisciplineSummary_GetByEPA]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EmployeeDisciplineSummary_GetByEPA]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeDisciplineSummary_GetByPeriod]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EmployeeDisciplineSummary_GetByPeriod]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EpaAttendanceSummary_GetOtherViolations]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EpaAttendanceSummary_GetOtherViolations]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EpaAttendanceSummary_GetViolationType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EpaAttendanceSummary_GetViolationType]
GO
------drop table------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Process]') AND type in (N'U'))
DROP TABLE [dbo].[Process]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrdinalNumbers]') AND type in (N'U'))
DROP TABLE [dbo].[OrdinalNumbers]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Lookup]') AND type in (N'U'))
DROP TABLE [dbo].[Lookup]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LookupData]') AND type in (N'U'))
DROP TABLE [dbo].[LookupData]
GO
-----------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefResource]') AND type in (N'U'))
DROP TABLE [dbo].[RefResource]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RefResource](
	[RefResourceId] [int] IDENTITY(1,1) NOT NULL,
	[RefName] [varchar](100) NOT NULL,
	[ResourceId] [smallint] NOT NULL,
	[Value] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_RefResource] PRIMARY KEY CLUSTERED 
(
	[RefResourceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[RefResource] ON
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (1, N'SystemRole', 1, N'SA')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (2, N'SystemRole', 2, N'HR')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (3, N'SystemRole', 3, N'MGT')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (4, N'SystemRole', 4, N'Reviewer')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (5, N'SystemRole', 5, N'Evaluator')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (6, N'SystemRole', 6, N'Employee')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (7, N'SystemRole', 7, N'Approver')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (8, N'SystemRole', 8, N'Section Admin')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (9, N'ProbationSettingDay', 1, N'15')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (10, N'SendMailToAll', 1, N'0')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (11, N'RatingType', 1, N'Star')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (12, N'RatingType', 2, N'Score')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (13, N'RatingType', 3, N'Half Star')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (14, N'EmployeeCategory', 0, N'Expat')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (15, N'EmployeeCategory', 1, N'Local')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (16, N'ProcessStatus', 0, N'None')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (17, N'ProcessStatus', 1, N'Open')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (18, N'ProcessStatus', 2, N'In Progress')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (19, N'ProcessStatus', 3, N'Closed')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (20, N'LogObjectType', 1, N'KPI')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (21, N'LogObjectType', 2, N'KPI Group')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (22, N'LogObjectType', 3, N'Target Template')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (23, N'LogObjectType', 4, N'User Management')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (24, N'LogObjectType', 5, N'Role Mangement')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (25, N'LogObjectType', 6, N'Flow Template')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (26, N'LogObjectType', 7, N'Target Employee Setting')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (27, N'LogObjectType', 8, N'Period')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (28, N'LogObjectType', 9, N'Discipline')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (29, N'LogObjectType', 10, N'EPA')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (30, N'EPAStatus', 0, N'Open')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (31, N'EPAStatus', 1, N'Closed')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (32, N'EPAStatus', 2, N'Rejected')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (33, N'EvaluationTimes', 1, N'1st')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (34, N'EvaluationTimes', 2, N'2nd')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (35, N'EvaluationTimes', 3, N'3rd')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (36, N'EvaluationTimes', 4, N'4th')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (37, N'EvaluationTimes', 5, N'5th')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (38, N'EvaluationTimes', 6, N'6th')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (39, N'EvaluationTimes', 7, N'7th')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (40, N'EvaluationTimes', 8, N'8th')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (41, N'EvaluationTimes', 9, N'9th')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (42, N'EvaluationTimes', 10, N'10th')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (43, N'RequiredCommentOnMaxScore', 1, N'1')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (44, N'EvaluationPhase', 2, N'Self')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (45, N'EvaluationPhase', 3, N'Evaluator')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (46, N'EvaluationPhase', 4, N'Reviewer')
INSERT [dbo].[RefResource] ([RefResourceId], [RefName], [ResourceId], [Value]) VALUES (47, N'EvaluationPhase', 5, N'Management')
SET IDENTITY_INSERT [dbo].[RefResource] OFF
GO
-----------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PeriodDisciplineSetting]') AND type in (N'U'))
DROP TABLE [dbo].[PeriodDisciplineSetting]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PeriodDisciplineSetting](
	[PeriodId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
 CONSTRAINT [PK_PeriodDisciplineSetting] PRIMARY KEY CLUSTERED 
(
	[PeriodId] ASC,
	[EmployeeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
-----------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DisciplineSummary]') AND type in (N'U'))
DROP TABLE [dbo].[DisciplineSummary]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DisciplineSummary](
	[EmployeeId] [int] NOT NULL,
	[WorkDate] [date] NOT NULL,
	[ChangeShift] [decimal](5, 2) NULL,
	[MissingCheckInOut] [decimal](5, 2) NULL,
	[LateIn] [decimal](5, 2) NULL,
	[EarlyOut] [decimal](5, 2) NULL,
	[Absent] [decimal](5, 2) NULL,
	[UnpaidLeave] [decimal](5, 2) NULL,
	[WorkingUndertime] [decimal](5, 2) NULL,
	[UnpaidLeaveAbsent] [decimal](5, 2) NULL,
	[UnplannedLeave] [decimal](5, 2) NULL,
	[WeekendLeave] [decimal](5, 2) NULL,
	[OtherDisciplines] [decimal](5, 2) NULL,
	[SystemSubtractScore] [decimal](5, 2) NULL,
	[ModifiedDate] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[SyncDate] [datetime] NULL,
 CONSTRAINT [PK_DisciplineSummary] PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC,
	[WorkDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO