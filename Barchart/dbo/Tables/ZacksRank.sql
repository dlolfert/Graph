CREATE TABLE [dbo].[ZacksRank] (
    [Symbol]   VARCHAR (50)   NOT NULL,
    [Rank]     INT            NULL,
    [Date]     DATETIME       NULL,
    [Momentum] CHAR (1)       NULL,
    [DayHigh]  DECIMAL (9, 2) NULL,
    [Open]     DECIMAL (9, 2) NULL,
    [Close]    DECIMAL (9, 2) NULL,
    [DayLow]   DECIMAL (9, 2) NULL,
    [Volume]   BIGINT         NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [NonClusteredIndex-20200324-134409]
    ON [dbo].[ZacksRank]([Symbol] ASC, [Date] ASC);

