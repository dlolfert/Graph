CREATE TABLE [dbo].[Top100] (
    [Symbol]   NVARCHAR (50)   NULL,
    [Name]     NVARCHAR (1000) NULL,
    [WtdAlpha] DECIMAL (18, 2) NULL,
    [Last]     DECIMAL (18, 2) NULL,
    [Change]   DECIMAL (18, 2) NULL,
    [%Chg]     DECIMAL (18, 2) NULL,
    [52WHigh]  DECIMAL (18, 2) NULL,
    [52WLow]   DECIMAL (18, 2) NULL,
    [52W%Chg]  DECIMAL (18, 2) NULL,
    [Date]     DATETIME        NULL
);

