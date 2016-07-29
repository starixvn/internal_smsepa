CREATE TABLE [dbo].[EmployeeDisciplinePermission]
(
	[EmpId] INT NOT NULL
	,[OwnerId] INT NOT NULL
	,[IsDeleted] BIT NULL
	,[ModifiedDate] DATETIME NULL
	,[SyncDate] DATETIME NULL
	,CONSTRAINT [PK_EmployeeDisciplinePermission] PRIMARY KEY CLUSTERED ([EmpId] ASC, [OwnerId] ASC)
)
