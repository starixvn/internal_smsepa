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
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EPA_GetNumberOfRow]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EPA_GetNumberOfRow]
GO
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
-----------------------------------