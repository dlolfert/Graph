



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[WeeklyHigh] 
	-- Add the parameters for the stored procedure here
	@Symbol NVARCHAR(MAX),
	@DaysBack INT = -365
AS
BEGIN

 DECLARE @Start AS DATETIME
 DECLARE @Monday AS DATETIME
 DECLARE @Tuesday AS DATETIME
 DECLARE @Friday AS DATETIME

 DECLARE @MondayOpen AS DECIMAL(11,2)
 DECLARE @WeekHigh AS DECIMAL(11,2)
 DECLARE @FridayClose AS DECIMAL(11,2)

 Select @Start = DATEADD(DAY, @DaysBack, GETDATE())
 
 WHILE(DATENAME(dw, @Start) != 'Monday')
 BEGIN
	SET @Start = DATEADD(DAY, 1, @Start)
 END

 WHILE(@Start < GETDATE())
 BEGIN

	 SET @Monday = @Start
	 SET @Monday = CAST(CAST(DATEPART(YEAR, @Monday) AS NVARCHAR(4)) + '-' + CAST(DATEPART(MONTH, @Monday) AS NVARCHAR(2)) + '-' + CAST(DATEPART(DAY, @Monday) AS NVARCHAR(2)) AS DATETIME)

	 SET @Tuesday = DATEADD(DAY, 1, @Start)
	 SET @Tuesday = CAST(CAST(DATEPART(YEAR, @Tuesday) AS NVARCHAR(4)) + '-' + CAST(DATEPART(MONTH, @Tuesday) AS NVARCHAR(2)) + '-' + CAST(DATEPART(DAY, @Tuesday) AS NVARCHAR(2)) AS DATETIME)

	 SET @Friday = DATEADD(DAY, 4, @Start)
	 SET @Friday = CAST(CAST(DATEPART(YEAR, @Friday) AS NVARCHAR(4)) + '-' + CAST(DATEPART(MONTH, @Friday) AS NVARCHAR(2)) + '-' + CAST(DATEPART(DAY, @Friday) AS NVARCHAR(2)) AS DATETIME)
  
	 SET @MondayOpen = 0
	 SET @WeekHigh = 0
	 SET @FridayClose = 0

	 Select @MondayOpen = [Open] From [Barchart].[dbo].[Yahoo] Where [Date] = @Monday And Symbol = @Symbol
	 IF(@MondayOpen = 0.00)
	 BEGIN
		Select @MondayOpen = [Open] From [Barchart].[dbo].[Yahoo] Where [Date] = DATEADD(DAY, 1, @Monday) And Symbol = @Symbol
	 END
	 
	 Select TOP 1 @WeekHigh = [DayHigh] From [Barchart].[dbo].[Yahoo] Where [Date] >= @Monday And [Date] <= @Friday And Symbol = @Symbol Order By DayHigh DESC
	 
	 Select @FridayClose = [Close] From [Barchart].[dbo].[Yahoo] Where [Date] = @Friday And Symbol = @Symbol
	 IF(@FridayClose = 0.00)
	 BEGIN
		Select @FridayClose = [Close] From [Barchart].[dbo].[Yahoo] Where [Date] = DATEADD(DAY, -1, @Friday) And Symbol = @Symbol
	 END
  
	 IF ((Select Count(*) From [Barchart].[dbo].[WeeklyGain] Where Symbol = @Symbol and Monday = @Monday) < 1)
	 BEGIN
		Insert Into [Barchart].[dbo].[WeeklyGain] Values(
			@Symbol, 
			@Monday, 
			@MondayOpen, 
			@WeekHigh, 
			CAST((((@MondayOpen * 100) / @WeekHigh) - 100) * -1 AS DECIMAL(11, 2)),
			@FridayClose,
			CAST((((@MondayOpen * 100) / @FridayClose) - 100) * -1 AS DECIMAL(11, 2)))
	 END

	 SET @Start = DATEADD(DAY, 7, @Monday)

 END
 
END