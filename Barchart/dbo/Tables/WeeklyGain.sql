CREATE TABLE [dbo].[WeeklyGain] (
    [Symbol]        NVARCHAR (100)  NOT NULL,
    [Monday]        DATE            NOT NULL,
    [MondayOpen]    DECIMAL (11, 2) NULL,
    [WeekHigh]      DECIMAL (11, 2) NULL,
    [Percent]       DECIMAL (11, 2) NULL,
    [FridayClose]   DECIMAL (11, 2) NULL,
    [WeeklyPercent] DECIMAL (11, 2) NULL
);

