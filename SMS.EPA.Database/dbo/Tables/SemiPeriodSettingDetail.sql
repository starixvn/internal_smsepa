CREATE TABLE [dbo].[SemiPeriodSettingDetail] (
    [SemiPeriodSettingId] INT NOT NULL,
    [Month]               INT NOT NULL,
    [IsLastYear]          BIT NOT NULL,
    [IsActive]            BIT NOT NULL,
    CONSTRAINT [PK_SemiPeriodSettingDetail] PRIMARY KEY CLUSTERED ([SemiPeriodSettingId] ASC, [Month] ASC)
);

