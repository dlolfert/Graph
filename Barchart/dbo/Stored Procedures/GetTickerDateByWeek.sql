
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetTickerDateByWeek]
	@Symbol NVARCHAR(5) = 'META'
AS
BEGIN

Declare @FDate AS DATETIME

DROP TABLE IF EXISTS #FirstDate;
Select Top 1 [Date] Into #FirstDate From dbo.Yahoo Where Symbol = @Symbol Order By Date ASC

Select @FDate = [Date] From #FirstDate

--SET @FDate = DATEADD(MONTH, -24, GETDATE())

DROP TABLE IF EXISTS #Output
CREATE TABLE #Output
(
    Symbol NVARCHAR(5),
    Monday DATETIME,
    Friday DATETIME,
    [Open] Decimal(9,2),
    [High] Decimal(9,2),
    [Close] Decimal(9,2)
)

While @FDate <= DATEADD(DAY, 7, GETDATE())
BEGIN

Declare @Monday AS DATETIME = [dbo].[GetMonday] (@FDate)
Declare @Friday AS DATETIME = [dbo].[GetFriday] (@FDate)
Declare @Open AS DECIMAL(9,2) = 0.0
Declare @High AS DECIMAL(9,2) = 0.0
Declare @Close AS DECIMAL(9,2) = 0.0

Select Top 1 @Open = [Open] FROM [dbo].[Yahoo] Where [Date] >= @Monday And [Date] <= @Friday And Symbol = @Symbol Order By [Date] Asc
Select Top 1 @High = DayHigh FROM [dbo].[Yahoo] Where [Date] >= @Monday And [Date] <= @Friday And Symbol = @Symbol Order By DayHigh DESC
Select Top 1 @Close = [Close] FROM [dbo].[Yahoo] Where [Date] >= @Monday And [Date] <= @Friday And Symbol = @Symbol Order By [Date] DESC
PRINT CAST(@Open AS NVARCHAR(MAX)) + ' ' + CAST(@High AS NVARCHAR(MAX)) + ' ' + CAST(@Close AS NVARCHAR(MAX))

IF (@Open > 0.0)
BEGIN
    Insert Into #Output (Symbol, Monday, Friday, [Open], [High], [Close]) Values (@Symbol, @Monday, @Friday, @Open, @High, @Close)
END

SET @FDate = DATEADD(DAY, 7, @FDate)
END

Select *, ([Close] - [Open]) AS WeekEndValue, ([High] - [Open]) AS MaxValue From #Output Order By Friday DESC
END