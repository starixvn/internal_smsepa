CREATE TABLE [dbo].[SemiPeriodSetting] (
    [SemiPeriodSettingId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (50) NOT NULL,
    [FromDate]            NVARCHAR (10) NOT NULL,
    [ToDate]              NVARCHAR (10) NOT NULL,
    [EffectiveDate] DATE NOT NULL, 
    CONSTRAINT [PK_SemiPeriodSetting] PRIMARY KEY CLUSTERED ([SemiPeriodSettingId] ASC)
);

