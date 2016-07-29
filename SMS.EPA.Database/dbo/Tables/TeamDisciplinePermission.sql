CREATE TABLE [dbo].[TeamDisciplinePermission]
(
	[TeamId] INT NOT NULL
	,[OwnerId] INT NOT NULL
	,[IsDeleted] BIT NULL
	,[ModifiedDate] DATETIME NULL
	,[SyncDate] DATETIME NULL
	,CONSTRAINT [PK_TeamDiscipline] PRIMARY KEY CLUSTERED ([TeamId] ASC, [OwnerId] ASC)
)
