CREATE TABLE [dbo].[Yahoo] (
    [Symbol]  VARCHAR (50)   NOT NULL,
    [Date]    DATETIME       NULL,
    [DayHigh] DECIMAL (9, 2) NULL,
    [Open]    DECIMAL (9, 2) NULL,
    [Close]   DECIMAL (9, 2) NULL,
    [DayLow]  DECIMAL (9, 2) NULL,
    [Volume]  BIGINT         NULL
);

