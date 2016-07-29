CREATE TABLE [dbo].[EmployeeDiscipline]
(
	[EmpId] INT NOT NULL
	,[WorkDate] DATE NOT NULL
	,[DisciplineTypeId] INT NOT NULL
	,[ShiftName] VARCHAR(50) NULL
	,[CheckIn] DATETIME NULL
	,[CheckOut] DATETIME NULL
	,[DisciplineScore] DECIMAL(5, 2) NULL
	,[TotalHours] DECIMAL(5, 2) NULL
	,[LateIn] INT NULL
	,[EarlyOut] INT NULL
	,[IsChangeShift] BIT NULL
	,[IsIgnoredChangeShift] BIT NULL
	,[IsMissingCheckInOut] BIT NULL
	,[IsIgnoredMissingCheckInOut] BIT NULL
	,[IsLateIn] BIT NULL
	,[IsIgnoredLateIn] BIT NULL
	,[IsEarlyOut] BIT NULL
	,[IsIgnoredEarlyOut] BIT NULL
	,[IsAbsent] BIT NULL
	,[IsIgnoredAbsent] BIT NULL
	,[IsUnpaidLeave] BIT NULL
	,[IsIgnoredUnpaidLeave] BIT NULL
	,[IsWorkingUnderTime] BIT NULL
	,[IsIgnoredWorkingUnderTime] BIT NULL
	,[IsUnpaidLeaveAbsent] BIT NULL
	,[IsIgnoredUnpaidLeaveAbsent] BIT NULL
	,[EmployeeComment] NVARCHAR(500) NULL
	,[ApproverComment] NVARCHAR(500) NULL
	,[ModifiedDate] DATETIME NULL
	,[IsDeleted] BIT NULL
	,[SyncDate] DATETIME NULL
	,CONSTRAINT [PK__EmployeeDiscipline] PRIMARY KEY CLUSTERED ([EmpId] ASC, [WorkDate] ASC, [DisciplineTypeId] ASC)
)
