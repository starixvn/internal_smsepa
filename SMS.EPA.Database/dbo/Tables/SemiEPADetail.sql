CREATE TABLE [dbo].[SemiEPADetail] (
    [EPAId] INT NOT NULL,
    [Month] INT NOT NULL,
    [Year]  INT NOT NULL,
    CONSTRAINT [PK_EPASemiMonthly_1] PRIMARY KEY CLUSTERED ([EPAId] ASC, [Month] ASC)
);

