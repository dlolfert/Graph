





CREATE PROCEDURE [dbo].[GetHeaderInfo] 
	@Symbol AS NVARCHAR(MAX) = 'SPXL'
AS
BEGIN

-- OUTPUT
DECLARE @NAME AS NVARCHAR(MAX)
DECLARE @AVG as DECIMAL(9,2)
DECLARE @DAYS_ABOVE_AVG INT
DECLARE @TOTAL_USING_ACTUAL AS DECIMAL(9,2)
DECLARE @TOTAL_USING_ADJACTUAL AS DECIMAL(9,2)
DECLARE @DAYS_CLOSE_ABOVE_OPEN AS INT
DECLARE @DAYS_HIGH_ABOVE_OPEN AS INT
DECLARE @STDEV AS DECIMAL(9,2)
DECLARE @RecCount AS INT
DECLARE @LastClose AS DECIMAL(9, 2)
DECLARE @AvgVolume AS BIGINT
--

SELECT TOP 500
      @AVG = AVG(DayHigh - [Open]),
	  @STDEV = CAST(STDEV(DayHigh - [Open]) AS DECIMAL(9,2)),
	  @RecCount = COUNT(*),
	  @AvgVolume = AVG(Volume)
	  FROM [Barchart].[dbo].[Yahoo]
	  Where Symbol = @Symbol And [DATE] >= DATEADD(DAY, -500, GETDATE())

SELECT TOP 1 @LastClose = [Close] FROM [Barchart].[dbo].[Yahoo] Where Symbol = @Symbol Order By [Date] DESC

--SELECT TOP 100 
--	  @STDEV = CAST(STDEV(DayHigh - [Open]) AS DECIMAL(6,2))
--	  FROM [Barchart].[dbo].[Yahoo]
--	  Where Symbol = @Symbol And [DATE] >= DATEADD(DAY, -100, GETDATE())

 Select @NAME = CASE WHEN [NAME] IS NULL THEN 'Name Not Found' WHEN [NAME] = '' THEN 'Name Blank' ELSE [NAME] END From Top100 WHERE Symbol = @Symbol

DECLARE @Adjusted table 
   ( 
   Actual Decimal(9,2) NOT NULL,
   
   AdjActual Decimal(9,2) NOT NULL,
   CloseAboveOpen bit,
   DayHighAboveOpen bit)
   
   INSERT INTO @Adjusted SELECT TOP 500
      (DayHigh - [Open]), 
	  CASE WHEN (DayHigh - [Open]) >= @AVG THEN @AVG ELSE (DayHigh - [Open]) END AS AdjActual,
	  CASE WHEN ([Close] > [Open]) THEN 1 ELSE 0 END,
	  CASE WHEN (DayHigh > [OPEN]) THEN 1 ELSE 0 END
  FROM [Barchart].[dbo].[Yahoo]
  Where Symbol = @Symbol And [DATE] >= DATEADD(DAY, -500, GETDATE())
  

Select @TOTAL_USING_ACTUAL = SUM(Actual), @TOTAL_USING_ADJACTUAL = SUM(AdjActual) From @Adjusted

Select @DAYS_ABOVE_AVG = COUNT(*) FROM @Adjusted Where Actual >= @AVG

Select @DAYS_CLOSE_ABOVE_OPEN = COUNT(*) FROM @Adjusted Where CloseAboveOpen = 1
SELECT @DAYS_HIGH_ABOVE_OPEN = COUNT(*) FROM @Adjusted WHERE DayHighAboveOpen = 1


SELECT
@SYMBOL AS Symbol,
@NAME AS Name,
@AVG AS [Average],
@DAYS_ABOVE_AVG AS DaysAboveAvg,
@TOTAL_USING_ACTUAL AS Total,
@TOTAL_USING_ADJACTUAL AS AdjustedTotal,
@DAYS_CLOSE_ABOVE_OPEN AS DaysCloseAboveOpen,
@DAYS_HIGH_ABOVE_OPEN AS DaysHighAboveOpen,
CAST((CAST(@DAYS_HIGH_ABOVE_OPEN AS DECIMAL(6,2)) / CAST(@RecCount AS DECIMAL(9,2)) * 100) AS DECIMAL(6,0)) AS '% Day High Above Open',
CAST((CAST(@DAYS_ABOVE_AVG AS DECIMAL(6,2)) / CAST(@RecCount AS DECIMAL(9,2)) * 100) AS DECIMAL(6,0)) AS '% Days Above Average',
CAST((CAST(@DAYS_CLOSE_ABOVE_OPEN AS DECIMAL(6,2)) / CAST(@RecCount AS DECIMAL(9,2)) * 100) AS DECIMAL(6,0)) AS '% Day Close Above Open',
@STDEV AS StdDev,
@RecCount AS Records,
@LastClose AS LastClose,
@AvgVolume AS AvgVolume
END